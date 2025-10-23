using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Repositories.Users;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class PatientDependency
    {
        public static void AddPatientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddTransient<IPatientService, PatientService>();
        }
    }
}