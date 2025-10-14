using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Application.Extensions
{
    /// <summary>
    /// Extension methods for registering Application layer services
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all Application layer services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(ApplicationServiceCollectionExtensions).Assembly);

            // Register generic CRUD service
            services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));

            // Register User Management Service
            services.AddScoped<IUserManagementService, UserManagementService>();

            // Add specific application services here as needed
            // Example:
            // services.AddScoped<IDataArchiveService, DataArchiveService>();
            // services.AddScoped<IRetentionPolicyService, RetentionPolicyService>();

            return services;
        }
    }
}
