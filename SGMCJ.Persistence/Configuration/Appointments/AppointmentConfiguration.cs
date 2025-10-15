using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Persistence.Configuration.Appointments
{
    public partial class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> entity)
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA291B4E736");

            entity.ToTable("Appointments", "appointments");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Appointme__Creat__5165187F")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Appointme__Updat__52593CB8")
                .HasColumnType("datetime");

            entity.HasOne<Doctor>()
                .WithMany(d => d.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctor");

            entity.HasOne<Patient>()
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Patient");

            entity.HasOne<Status>()
                .WithMany(s => s.Appointments)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Status");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Appointment> entity);
    }
}