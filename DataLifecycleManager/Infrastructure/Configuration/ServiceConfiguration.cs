using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Infrastructure.Configuration
{
    /// <summary>
    /// Infrastructure services configuration
    /// </summary>
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddInfrastructureServicesConfiguration(
            this IServiceCollection services)
        {
            // Register infrastructure services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
