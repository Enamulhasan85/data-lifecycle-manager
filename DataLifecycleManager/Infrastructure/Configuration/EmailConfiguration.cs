using DataLifecycleManager.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Infrastructure.Configuration;

/// <summary>
/// Email configuration and setup
/// </summary>
public static class EmailConfiguration
{
    public static IServiceCollection AddEmailConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure email settings
        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));

        return services;
    }
}
