using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Persistence.Repositories.Appointments;


namespace SGMCJ.Infrastructure.Dependencies
{
    public static class AvailabilityDependency
    {
        public static void AddAvailabilityDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();
            services.AddTransient<IAvailabilityService, AvailabilityService>();
        }
    }
}