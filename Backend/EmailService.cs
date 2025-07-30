using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;


namespace thomasmack.dev.Backend
{
    public class EmailService(IOptions<SmtpSettings> opts) : IEmailService
    {
        private readonly SmtpSettings _settings = opts.Value;

        public async Task SendAsync(string from, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from, MailboxAddress.Parse(from).ToString()));
            message.To.Add(MailboxAddress.Parse(_settings.Email));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Hostname, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

