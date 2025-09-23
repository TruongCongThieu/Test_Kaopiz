using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:""<>?|])[A-Za-z\d!@#$%^&*()_+:""<>?|]{8,}$")]
        public string Password { get; set; } = default!;

        public short? MembershipPackageId { get; set; }

        public string? CompanyName { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? LinkWebsite { get; set; }

        [MaxLength(500)]
        public string? AccountZalo { get; set; }

        [MaxLength(500)]
        public string? AccountFb { get; set; }

        public string? ReasonJoin { get; set; }
        public string? ReferralSource { get; set; }
        public string? JoiningGoal { get; set; }
    }
}