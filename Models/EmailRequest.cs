using System.ComponentModel.DataAnnotations;

namespace thomasmack.dev.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string From { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;


        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
