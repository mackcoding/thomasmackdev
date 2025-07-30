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
            if (string.IsNullOrWhiteSpace(from))
                throw new ArgumentException("Sender address is required.", nameof(from));

            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(new MailboxAddress(_settings.SendTo, _settings.SendTo));

            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }
                              .ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Hostname, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

