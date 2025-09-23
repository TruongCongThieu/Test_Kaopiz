using BusinessLogic.Services.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            #region System
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            #endregion System
        }
    }
}