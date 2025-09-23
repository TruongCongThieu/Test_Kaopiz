using BusinessLogic.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities.Identity
{
	public class AppUser : IdentityUser<string>, IAuditableEntity<string>
	{
        public override string? Email { get; set; }
		public string CreatedBy { get; set; } = default!;
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }
		public bool IsDeleted { get; set; }
    }
}