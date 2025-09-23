using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:""<>?|])[A-Za-z\d!@#$%^&*()_+:""<>?|]{8,}$")]
        public string NewPassword { get; set; } = default!;
    }
}
