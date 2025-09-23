using BusinessLogic.Configurations;
using BusinessLogic.Contexts;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services.Common;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using WebApp.Services;

namespace WebApp.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static AppConfiguration GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            services.Configure<MailConfiguration>(configuration.GetSection(nameof(MailConfiguration)));
            return applicationSettingsConfiguration.Get<AppConfiguration>() ?? throw new InvalidOperationException();
        }

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

        internal static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentity<AppUser, AppRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Lockout settings
                    options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("Session:DefaultLockoutTimeSpan");
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Session:DefaultLockoutTimeSpan"));
                    options.Lockout.AllowedForNewUsers = false;
                    // User settings
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Reset token after
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes(configuration.GetValue<int>("Session:TokenLifespan")));

            // Add session
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromHours(configuration.GetValue<int>("Session:IdleTimeoutHours"));
                o.Cookie.HttpOnly = true;
            });

            // Add config Cookie
            // Setting cookie login theo Scheme Member
            // Member Login: await HttpContext.SignInAsync("MemberAuth", claimsPrincipal);
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "AdminAuth";
                })
                .AddCookie("MemberAuth", options =>
                {
                    options.SlidingExpiration = true;
                    options.LoginPath = "/dang-nhap";
                    options.AccessDeniedPath = "/Member/AccessDenied/Index";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Cookie:ExpireTimeSpan"));
                    options.Cookie.Name = ".AspNetCore.MemberAuth";
                });

            // Add config SecurityStampValidator
            // Update security stamp to invalidate current cookies
            // await _userManager.UpdateSecurityStampAsync(user);
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(5);
            });

            // Add application services.
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            // Add user claim
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();
            // Add IMemoryCache
            services.AddMemoryCache();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                googleOptions.CallbackPath = "/identity/login-google";
            });

            return services;
        }

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<CurrentUserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        /// <summary>
        /// Add HangFire Extension
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configuration"></param>
        public static void AddHangFireExtension(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = false // 🚫 Do not run migration if schema already exists
                });
            });
            service.AddHangfireServer();
        }
    }
}