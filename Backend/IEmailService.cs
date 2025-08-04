namespace thomasmack.dev.Backend
{
    public interface IEmailService
    {
        Task SendAsync(string name, string from, string subject, string htmlBody);
    }

}
