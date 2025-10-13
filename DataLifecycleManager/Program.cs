using DataLifecycleManager.Application.Extensions;
using DataLifecycleManager.Infrastructure.Data;
using DataLifecycleManager.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataLifecycleManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            // Add Application layer services
            builder.Services.AddApplication();

            // Add Infrastructure layer services (Database, Identity, Repositories, etc.)
            builder.Services.AddInfrastructure(builder.Configuration);

            // Add MVC Controllers with Views
            builder.Services.AddControllersWithViews();

            // Add Razor Pages (for Identity UI)
            builder.Services.AddRazorPages();

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
