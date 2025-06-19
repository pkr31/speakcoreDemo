using System.ComponentModel.DataAnnotations;

namespace SpeakCoreApp.Server.DTOs
{
    public class LoginRequestViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
