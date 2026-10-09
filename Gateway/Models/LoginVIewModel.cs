using System.ComponentModel.DataAnnotations;

namespace Gateway.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter your email address."),
         EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password."), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public string? AppName { get; set; }
    }
}