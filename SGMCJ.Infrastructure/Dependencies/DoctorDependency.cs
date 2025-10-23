using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Repositories.Users;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class DoctorDependency
    {
        public static void AddDoctorDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddTransient<IDoctorService, DoctorService>();
        }
    }
}