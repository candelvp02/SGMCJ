using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;


namespace SGMCJ.Infrastructure.Dependencies
{
    public static class AvailabilityDependency
    {
        public static void AddAvailabilityDependencies(this IServiceCollection services)
        {
            //services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
            services.AddTransient<IAvailabilityService, AvailabilityService>();
        }
    }
}