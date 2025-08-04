using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using thomasmack.dev.Backend;
using thomasmack.dev.Models;

namespace thomasmack.dev.Pages
{
    public class ContactModel(IEmailService emails,
        IMemoryCache cache,
        ILogger<ContactModel> log,
        IOptions<SmtpSettings> _settings) : PageModel
    {
        [BindProperty]
        public required EmailRequest Req { get; set; }

        [BindProperty(Name = "cf-turnstile-response")]
        public string? TurnstileToken { get; set; }


        public string SiteKey => _settings.Value.CaptchaSiteKey;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var remoteAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent;
            var key = $"{remoteAddr}";

            if (!await VerifyTurnstile())
            {
                ModelState.AddModelError("Captcha", "Please complete the CAPTCHA correctly.");

                log.LogError("Invalid captcha {RemoteAddr} - {UserAgent}", remoteAddr, userAgent);
                return Page();
            }

            if (string.IsNullOrEmpty(Req.Body)
             || string.IsNullOrEmpty(Req.Name)
             || string.IsNullOrEmpty(Req.From)
             || string.IsNullOrEmpty(Req.Subject))
            {
                log.LogError("Postback contained empty data {RemoteAddr} - {UserAgent}", remoteAddr, userAgent);
                return Page();
            }

            if (Req.Body.Length < 50)
            {
                log.LogError("Form data was too short from {RemoteAddr} - {UserAgent}", remoteAddr, userAgent);
                ModelState.AddModelError("Req.Body", "Be sure to include details in your message.");
                return Page();
            }


            if (cache.TryGetValue<bool>(key, out _))
            {
                TempData["status"] = "sent";
                log.LogError("IP already has sent email today {RemoteAddr} - {UserAgent}", remoteAddr, userAgent);
                return RedirectToPage();
            }

            Req.Body = $@"
                {Req.Body}{Environment.NewLine}
                ---Form Details---{Environment.NewLine}
                IP Address: {remoteAddr}{Environment.NewLine}
                UserAgent: {userAgent}{Environment.NewLine}
                Sent: {DateTime.Now}{Environment.NewLine}
            ";

            await emails.SendAsync(Req.Name, Req.From, Req.Subject, Req.Body);
            cache.Set(key, true, TimeSpan.FromHours(6));

            TempData["Status"] = "sent";
            return RedirectToPage();
        }

        private async Task<bool> VerifyTurnstile()
        {
            var secret = _settings.Value.CaptchaSiteSecret;

            using var client = new HttpClient();

            var form = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", TurnstileToken ?? "")
            
            ]);

            var response = await client.PostAsync(
                "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                form);

            if (!response.IsSuccessStatusCode)
                return false;

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync()
            );

            return doc.RootElement
                .GetProperty("success")
                .GetBoolean();

        }

    }
}
