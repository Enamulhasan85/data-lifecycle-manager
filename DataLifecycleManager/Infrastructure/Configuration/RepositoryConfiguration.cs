using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Infrastructure.Data;
using DataLifecycleManager.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataLifecycleManager.Infrastructure.Configuration
{
    /// <summary>
    /// Repository and Unit of Work configuration
    /// </summary>
    public static class RepositoryConfiguration
    {
        public static IServiceCollection AddRepositoryConfiguration(
            this IServiceCollection services)
        {
            // Register generic repository
            services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

            // Register specific repositories


            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
