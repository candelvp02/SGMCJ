using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Entities.Insurance;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Persistence.Context
{
    public partial class HealtSyncContext : DbContext
    {
        public HealtSyncContext(DbContextOptions<HealtSyncContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }

        public virtual DbSet<AvailabilityMode> AvailabilityModes { get; set; }

        public virtual DbSet<Doctor> Doctors { get; set; }

        public virtual DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<InsuranceProvider> InsuranceProviders { get; set; }

        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

        public virtual DbSet<NetworkType> NetworkTypes { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Patient> Patients { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Specialty> Specialties { get; set; }

        public virtual DbSet<Status> Statuses { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new Persistence.Configuration.Appointments.AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Appointments.DoctorAvailabilityConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Insurance.InsuranceProviderConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Insurance.NetworkTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Medical.AvailabilityModeConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Medical.MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Medical.SpecialtyConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.System.NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.System.RoleConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.System.StatusConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Users.DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Users.EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Users.PatientConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Users.PersonConfiguration());
            modelBuilder.ApplyConfiguration(new Domain.Configuration.Users.UserConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}