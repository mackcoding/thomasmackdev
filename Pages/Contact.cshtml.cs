using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using thomasmack.dev.Backend;
using thomasmack.dev.Models;

namespace thomasmack.dev.Pages
{
    public class ContactModel(IEmailService emails) : PageModel
    {
        private readonly IEmailService _emails = emails;

        [BindProperty]
        public required EmailRequest Req { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _emails.SendAsync(Req.From, Req.Subject, Req.Body);
            TempData["Status"] = "Email sent!";
            return RedirectToPage();
        }
    }
}
