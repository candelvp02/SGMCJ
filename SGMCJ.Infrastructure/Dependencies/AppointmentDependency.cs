using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Persistence.Repositories.Appointments;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class AppointmentDependency
    {
        public static void AddAppointmentDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<IAppointmentService, AppointmentService>();
        }
    }
}