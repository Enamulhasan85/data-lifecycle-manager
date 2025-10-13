using DataLifecycleManager.Application.Settings;
using DataLifecycleManager.Infrastructure.Data.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Infrastructure.Configuration
{
    /// <summary>
    /// Seeding configuration and setup
    /// </summary>
    public static class SeedConfiguration
    {
        public static IServiceCollection AddSeedConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure seeding options
            services.Configure<DefaultUsersAndRolesOptions>(
                configuration.GetSection("DefaultUsersAndRoles"));

            services.Configure<UserSeedOptions>(
                configuration.GetSection("UserSeedOptions"));

            // Register seeder
            services.AddScoped<DbSeeder>();

            return services;
        }
    }
}
