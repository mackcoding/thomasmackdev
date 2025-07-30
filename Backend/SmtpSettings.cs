namespace thomasmack.dev.Backend
{
    public class SmtpSettings
    {
        public required string Hostname { get; set; }
        public int Port { get; set; } = 465;
        public bool UseSsl { get; set; } = true;
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string SendTo { get; set; }
    }
}
