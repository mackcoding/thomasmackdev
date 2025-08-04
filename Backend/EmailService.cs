using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;


namespace thomasmack.dev.Backend
{
    public class EmailService(IOptions<SmtpSettings> opts) : IEmailService
    {
        private readonly SmtpSettings _settings = opts.Value;

        public async Task SendAsync(string name, string from, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(from))
                throw new ArgumentException("Sender address is required.", nameof(from));

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(name, from));
            message.To.Clear();
            message.To.Add(new MailboxAddress(_settings.Name, _settings.SendTo));

            message.Subject = subject;
            message.Body = new BodyBuilder { TextBody = body }
                              .ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _settings.Hostname,
                _settings.Port,
                MailKit.Security.SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

