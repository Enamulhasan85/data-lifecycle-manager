using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace DataLifecycleManager.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for configuring the WebApplication pipeline
    /// </summary>
    public static class InfrastructureWebApplicationExtensions
    {
        /// <summary>
        /// Configures the Infrastructure middleware pipeline
        /// </summary>
        public static async Task<WebApplication> UseInfrastructureAsync(
            this WebApplication app,
            bool runMigrations = true,
            bool seedDatabase = true)
        {
            // Apply database migrations if enabled
            if (runMigrations)
            {
                await app.MigrateDatabaseAsync();
            }

            // Seed default users and roles if enabled
            if (seedDatabase)
            {
                await app.SeedDatabaseAsync();
            }

            return app;
        }

        /// <summary>
        /// Configures the Infrastructure middleware pipeline (synchronous version)
        /// </summary>
        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            // This is a synchronous wrapper for scenarios where async is not needed
            // For database migration/seeding, use UseInfrastructureAsync instead
            return app;
        }
    }
}
