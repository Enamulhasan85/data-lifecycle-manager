using DataLifecycleManager.Application.Extensions;
using DataLifecycleManager.Infrastructure.Extensions;
using DataLifecycleManager.Presentation.Extensions;

namespace DataLifecycleManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Application layer services
            builder.Services.AddApplication();

            // Add Infrastructure layer services (Database, Identity, Repositories, etc.)
            builder.Services.AddInfrastructure(builder.Configuration);

            // Add Presentation layer services (MVC, Razor Pages, Response Compression, etc.)
            builder.Services.AddPresentation(builder.Configuration);

            // Add Database Developer Page Exception Filter (for development)
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Build the application
            var app = builder.Build();

            // Apply database migrations and seed default users/roles (both dev and production)
            await app.UseInfrastructureAsync(
                runMigrations: true,  // Set to false if you manage migrations manually
                seedDatabase: true     // Seed default users and roles on startup
            );

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                // Show detailed database errors in development
                app.UseMigrationsEndPoint();
            }
            else
            {
                // Use error handler in production
                app.UseExceptionHandler("/Home/Error");

                // Enable HSTS (HTTP Strict Transport Security)
                app.UseHsts();
            }

            // Middleware pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Map routes
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            // Run the application
            app.Run();
        }
    }
}
