using DataLifecycleManager.Infrastructure.Data;
using DataLifecycleManager.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace DataLifecycleManager.Extensions
{
    public static class DatabaseSeederExtensions
    {
        /// <summary>
        /// Migrates AppDbContext databases
        /// </summary>
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>();

            try
            {
                // Migrate ApplicationDbContext (which includes Identity tables)
                var appDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await appDbContext.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the databases");
                throw;
            }

            return app;
        }

        /// <summary>
        /// Seeds the database with default data (users, roles, etc.)
        /// </summary>
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>();

            try
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();
                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }

            return app;
        }
    }
}
