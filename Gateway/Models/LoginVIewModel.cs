using System.ComponentModel.DataAnnotations;

namespace ITElectiveSSO.Models
{
    public class LoginViewModel
    {
        // Ensure form is accessible (labels, aria attributes) - FACTOR
        // code here

        // Ensure form is accessible (labels, aria attributes) - FACTOR
        // code here

        public string? ReturnUrl { get; set; }

        // Display app name if provided in returnUrl - LLORENTE
        public string? AppName { get; set; }


        // Style error messages clearly - CABARDO
        public string? ErrorMessage { get; set; }
    }
}
