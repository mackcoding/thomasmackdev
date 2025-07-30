// Pages/Contact.cshtml.cs
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using thomasmack.dev.Backend;
using thomasmack.dev.Models;

namespace thomasmack.dev.Pages
{
    public class ContactModel(IEmailService emails, IMemoryCache cache, ILogger<ContactModel> log) : PageModel
    {
        [BindProperty]
        public required EmailRequest Req { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var remoteAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent;
            var key = $"{remoteAddr}";

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
                {Req.Body}</p>{Environment.NewLine}</hr>
                ---Form Details---{Environment.NewLine}
                IP Address: {remoteAddr}</p>{Environment.NewLine}
                UserAgent: {userAgent}</p>{Environment.NewLine}
                Sent: {DateTime.Now}{Environment.NewLine}
            ";

            await emails.SendAsync(Req.From, Req.Subject, Req.Body);
            cache.Set(key, true, TimeSpan.FromHours(24));

            TempData["Status"] = "sent";
            return RedirectToPage();
        }



        /*
        public async Task<IActionResult> OnPostAsyncOld()
        {
            if (!ModelState.IsValid)
                return Page();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{CACHE_PREFIX}{ip}:{Req.From.ToLowerInvariant()}";

            if (cache.TryGetValue<bool>(key, out _))
            {
                ModelState.AddModelError(string.Empty, "You've already sent a message in the past 24 hours. Please try again later.");
                return Page();
            }

            if (Req.Body.Length < 200)
            {
                ModelState.AddModelError(nameof(Req.Body), "Please provide a more detailed message (at least 200 characters).");
                return Page();
            }

            var safeBody = HtmlEncoder.Default.Encode(Req.Body);
            var metadata = $"\n\n---\nIP: {ip}\nUA: {Request.Headers["User-Agent"]}\nSent: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
            var fullBody = safeBody + metadata;

            await emails.SendAsync(Req.From, Req.Subject, fullBody);
            cache.Set(key, true, WINDOW);
            TempData["Status"] = "sent";
            return RedirectToPage();
        }*/
    }
}
