using Microsoft.AspNetCore.Mvc;

namespace DataLifecycleManager.Presentation.Extensions
{
    /// <summary>
    /// Presentation layer service collection extensions
    /// Contains only configurations specific to the MVC Web layer
    /// </summary>
    public static class PresentationServiceCollectionExtensions
    {
        /// <summary>
        /// Add MVC presentation layer services and configurations
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPresentation(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add MVC Controllers with Views
            services.AddControllersWithViews(options =>
            {
                // Add global filters if needed
                // options.Filters.Add<YourGlobalFilter>();
            })
            .AddJsonOptions(options =>
            {
                // Configure JSON serialization for AJAX responses
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase for .NET conventions
                options.JsonSerializerOptions.WriteIndented = true;
            });

            // Add Razor Pages (required for Identity UI)
            services.AddRazorPages();

            // Configure routing options
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false; // Keep query strings case-sensitive
            });

            // Add Response Compression for better performance
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Add Session support if needed
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}