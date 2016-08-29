using System.ComponentModel.DataAnnotations;

namespace AspNetCore_SMTP_MalKit.Models
{
    public class Contact
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
