using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Repositories.Medical;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class MedicalRecordDependency
    {
        public static void AddMedicalRecordDependencies(this IServiceCollection services)
        {
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddTransient<IMedicalRecordService, MedicalRecordService>();
        }
    }
}