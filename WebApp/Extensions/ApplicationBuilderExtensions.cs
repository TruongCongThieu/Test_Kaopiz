using BusinessLogic.Services.Common;
using Microsoft.Extensions.Options;

namespace WebApp.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        internal static void UseEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {      
                // Enable attribute routing for controllers
                endpoints.MapControllers();

                // Default route: redirect root to login page
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/dang-nhap");
                    await Task.CompletedTask;
                });

            });
            app.Use(async (context, next) =>
            {
                await next();

                // If no router matches, redirect to Landing Page/Home/Error
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    context.Response.Redirect("/khong-tim-thay-trang");
                }
            });
        }

        internal static void UseLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

        }
    }
}