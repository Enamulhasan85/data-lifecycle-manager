using DataLifecycleManager.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering all Infrastructure layer services
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all Infrastructure layer services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add HttpContextAccessor (required for CurrentUserService)
            services.AddHttpContextAccessor();

            // Add Database Configuration (DbContext, EF Core)
            services.AddDatabaseConfiguration(configuration);

            // Add Identity Configuration
            services.AddIdentityConfiguration();

            // Add Repository Configuration (Generic Repository, Unit of Work)
            services.AddRepositoryConfiguration();

            // Add Infrastructure Services (CurrentUserService, DateTimeService, etc.)
            services.AddInfrastructureServicesConfiguration();

            // Add Email Configuration
            services.AddEmailConfiguration(configuration);

            // Add Seed Configuration (DbSeeder, default users/roles)
            services.AddSeedConfiguration(configuration);

            // Add Logging Configuration
            services.AddLoggingConfiguration(configuration);

            return services;
        }
    }
}
