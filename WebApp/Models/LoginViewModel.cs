using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:""<>?|])[A-Za-z\d!@#$%^&*()_+:""<>?|]{8,}$")]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }
    }
}
