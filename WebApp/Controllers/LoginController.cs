using BusinessLogic.Constants.Role;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.User;
using BusinessLogic.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LoginController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ApplicationDbContext dbContext,
                                 RoleManager<AppRole> roleManager) : Controller
    {
        [HttpGet("dang-nhap")]
        public async Task<IActionResult> Login()
        {
            var authResult = await HttpContext.AuthenticateAsync(SchemeConstants.Member);

            if (authResult.Succeeded && authResult.Principal != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost("login-post")]
        public async Task<IActionResult> PostLogin([FromBody] LoginViewModel request)
        {
            if (!ModelState.IsValid)
                return Json(new { succeeded = false, messages = "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại sau hoặc liên hệ hỗ trợ." });

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Json(new { succeeded = false, messages = "Tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại." });
            }

            // Validate role
            if (!await userManager.IsInRoleAsync(user, RoleConstants.Member) && !await userManager.IsInRoleAsync(user, RoleConstants.Admin))
            {
                return Json(new { succeeded = false, messages = "Tài khoản không có quyền truy cập." });
            }

            var securityStamp = await userManager.GetSecurityStampAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new("Email", user.Email ?? ""),
                new(ClaimTypes.Email, user.Email!),
                new("AspNet.Identity.SecurityStamp",securityStamp )
            };
            var claimsIdentity = new ClaimsIdentity(claims, SchemeConstants.Member);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = request.RememberMe ? DateTime.Now.AddMonths(1) : DateTime.Now.AddHours(1)
            };

            await HttpContext.SignInAsync(SchemeConstants.Member, claimsPrincipal, authProperties);

            var eventId = Request.Query["eventScheduleId"].ToString();
            if (!string.IsNullOrEmpty(eventId))
            {
                return Json(new { succeeded = true, messages = "", eventId });
            }
            return Json(new { succeeded = true, messages = "" });
        }

        [HttpGet("dang-ky")]
        public async Task<IActionResult> Register()
        {
            var authResult = await HttpContext.AuthenticateAsync(SchemeConstants.Member);

            if (authResult.Succeeded && authResult.Principal != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("~/Views/Login/Register.cshtml");
        }

        [HttpPost("dang-ky-ngay")]
        public async Task<IActionResult> PostRegister([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { succeeded = false, messages = "Dữ liệu không hợp lệ" });
            }

            var existingUser = await dbContext.Users.AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            (EF.Functions.Collate(x.Email!, "SQL_Latin1_General_CP1_CI_AS") == request.Email))
                .Select(x => new { x.Email})
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Email!.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { succeeded = false, messages = "Email đã được sử dụng." });
                }
            }

            // Create data user identity
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName,
                LockoutEnabled = false
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                return Json(new { succeeded = false, messages = string.Join(", ", errorMessages) });
            }

            // Set default type is user in role member
            if (!await roleManager.RoleExistsAsync(RoleConstants.Member))
            {
                await roleManager.CreateAsync(new AppRole(RoleConstants.Member));
            }
            await userManager.AddToRoleAsync(user, RoleConstants.Member);
            return Json(new { succeeded = true });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(SchemeConstants.Member);
            return RedirectToAction("Login", "Login");
        }
    }
}
