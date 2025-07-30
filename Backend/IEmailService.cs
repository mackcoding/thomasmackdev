namespace thomasmack.dev.Backend
{
    public interface IEmailService
    {
        Task SendAsync(string from, string subject, string htmlBody);
    }

}
