using BusinessLogic.Constants.Role;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Middleware
{
    /// <summary>
    /// Middleware để kiểm tra authentication cho các trang yêu cầu đăng nhập
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Kiểm tra nếu đây là trang chủ
            if (path != null && path.StartsWith("/trang-chu"))
            {
                // Kiểm tra authentication cho Member scheme
                var authResult = await context.AuthenticateAsync(SchemeConstants.Member);

                if (!authResult.Succeeded || authResult.Principal == null)
                {
                    // Redirect về trang đăng nhập
                    context.Response.Redirect("/dang-nhap");
                    return;
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method để đăng ký AuthenticationMiddleware
    /// </summary>
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
