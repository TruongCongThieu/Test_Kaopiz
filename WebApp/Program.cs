using BusinessLogic.Extensions;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp.Extensions;
using WebApp.Middleware;
using Microsoft.AspNetCore.Identity;
using BusinessLogic.Entities.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddAuthorization();

// Configuration Cookie
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddCurrentUserService();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddHangFireExtension(builder.Configuration);
builder.Services.AddDistributedMemoryCache();

// Add services
builder.Services.AddServices();
builder.Services.GetApplicationSettings(builder.Configuration);
// Configure Serilog from AppSettings.json file
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Error)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning);
});
var app = builder.Build();
var hangfireBasicAuthSection = builder.Configuration.GetSection("HangfireBasicAuth");
var hangfireUsername = hangfireBasicAuthSection.GetValue<string>("Username");
var hangfirePassword = hangfireBasicAuthSection.GetValue<string>("Password");
app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect = false, // Có https thì bật true
            RequireSsl = false,  // Có https thì bật true
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = hangfireUsername,
                    PasswordClear = hangfirePassword
                }
            }
        })
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/loi-he-thong");

    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = ex.Message;
            string stackTrace = ex.StackTrace ?? "Không có StackTrace";
            string exceptionType = ex.GetType().ToString();
            string requestPath = context.Request.Path;
            string requestQuery = context.Request.QueryString.ToString();
            string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Không xác định";
            string userAgent = context.Request.Headers.ContainsKey("User-Agent") ? context.Request.Headers["User-Agent"].ToString() : "Không xác định";

            Log.Error("🔥 [DATABASE ERROR] Lỗi DB: {ErrorMessage}, Loại: {ExceptionType}, Đường dẫn: {RequestPath}, Query: {RequestQuery}, IP: {ClientIP}, User-Agent: {UserAgent}",
                errorMessage, exceptionType, requestPath, requestQuery, clientIp, userAgent);
            Log.Error("📌 [STACK TRACE] {StackTrace}", stackTrace);

            if (ex.InnerException is SqlException sqlEx)
            {
                Log.Error("❌ [SQL ERROR] Mã lỗi: {ErrorCode}, Server: {Server}, Message: {SqlMessage}",
                    sqlEx.Number, sqlEx.Server, sqlEx.Message);
            }

            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/loi-he-thong");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;
            string stackTrace = ex.StackTrace ?? "Không có StackTrace";
            string exceptionType = ex.GetType().ToString();
            string requestPath = context.Request.Path;
            string requestQuery = context.Request.QueryString.ToString();
            string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Không xác định";
            string userAgent = context.Request.Headers["User-Agent"]!;
            string errorTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss UTC");

            Log.Error("🔥 [LỖI TOÀN CỤC] Thời gian: {ErrorTime}, Lỗi: {ErrorMessage}, Loại: {ExceptionType}, Đường dẫn: {RequestPath}, Query: {RequestQuery}, IP: {ClientIP}, User-Agent: {UserAgent}",
                errorTime, errorMessage, exceptionType, requestPath, requestQuery, clientIp, userAgent);
            Log.Error("📌 [STACK TRACE] {StackTrace}", stackTrace);

            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/loi-he-thong");
            }
        }
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthenticationMiddleware();
app.UseLocalization();
app.UseEndpoints();

// Seed default roles (Member) on startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Seeder");

    try
    {
        const string memberRole = BusinessLogic.Constants.Role.RoleConstants.Member;
        var roleExists = roleManager.RoleExistsAsync(memberRole).GetAwaiter().GetResult();
        if (!roleExists)
        {
            var result = roleManager.CreateAsync(new AppRole(memberRole)).GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                logger.LogInformation("Created role: {Role}", memberRole);
            }
            else
            {
                logger.LogError("Failed to create role {Role}: {Errors}", memberRole, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding roles");
    }
}
app.Run();
