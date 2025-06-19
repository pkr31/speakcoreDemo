using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpeakCoreApp.Server.DTOs
{
    public class RegistrationRequestViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        public string ConfirmEmail { get; set; } = string.Empty;
        public bool Subscribe { get; set; } = false;
    }
}
