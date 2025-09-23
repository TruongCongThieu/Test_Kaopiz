using System.Security.Claims;
using BusinessLogic.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApp.Helpers
{
    public class CustomClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> options)
        : UserClaimsPrincipalFactory<AppUser, AppRole>(userManager, roleManager, options)
    {
        private readonly UserManager<AppUser> _userManger = userManager;

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            ((ClaimsIdentity)principal.Identity!).AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim("Email",user.Email ?? ""),
                new Claim("PhoneNumber",user.PhoneNumber ?? ""),
                new Claim("Roles",string.Join(";",roles)),
                new Claim("UserId",user.Id),
                new Claim("UserName",user.UserName ?? string.Empty),
            });
            return principal;
        }
    }
}