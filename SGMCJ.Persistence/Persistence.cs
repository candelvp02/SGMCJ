using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Domain.Repositories.Insurance;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Domain.Repositories.System;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Common;
using SGMCJ.Persistence.Context;
using SGMCJ.Persistence.Repositories.Appointments;
using SGMCJ.Persistence.Repositories.Insurance;
using SGMCJ.Persistence.Repositories.Medical;
using SGMCJ.Persistence.Repositories.System;
using SGMCJ.Persistence.Repositories.Users;

namespace SGMCJ.Persistence
{
    public static class Persistence
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration cfg)
        {
            // DbContext
            services.AddDbContext<HealtSyncContext>(options =>
                options.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            // StoredProcedure Executor
            services.AddScoped<StoredProcedureExecutor>();

            // Appointments Repositories
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();

            // Insurance Repositories
            services.AddScoped<IInsuranceProviderRepository, InsuranceProviderRepository>();
            services.AddScoped<INetworkTypeRepository, NetworkTypeRepository>();

            // Medical Repositories
            services.AddScoped<IAvailabilityModeRepository, AvailabilityModeRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

            // System Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();

            // Users Repositories
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}