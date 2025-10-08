using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Entities.Security;


namespace SGMCJ.Persistence.Context
{
    public class SGMCJDbContext : DbContext
    {
        public SGMCJDbContext(DbContextOptions<SGMCJDbContext> options) : base(options) { }

        public DbSet<Cita> Citas { get; set; } = null!;
        public DbSet<Paciente> Pacientes { get; set; } = null!;
        public DbSet<Medico> Medicos { get; set; } = null!;
        public DbSet<Disponibilidad> Disponibilidades { get; set; } = null!;
        public DbSet<Recordatorio> Recordatorios { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<UsuarioRol> UsuarioRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCita(modelBuilder);
            ConfigureMedico(modelBuilder);
            ConfigurePaciente(modelBuilder);
            ConfigureDisponibilidad(modelBuilder);
            ConfigureRecordatorio(modelBuilder);

            ConfigureUsuario(modelBuilder);
            ConfigureRol(modelBuilder);
            ConfigureUsuarioRol(modelBuilder);

            SeedData(modelBuilder);
        }

        private void ConfigureCita(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaHora).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
                entity.Property(e => e.Motivo).HasMaxLength(500);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.Citas)
                      .HasForeignKey(e => e.PacienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Medico)
                      .WithMany(m => m.Citas)
                      .HasForeignKey(e => e.MedicoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureMedico(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroLicencia).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Especialidad).IsRequired();
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.Sexo).IsRequired().HasMaxLength(10);

                entity.HasIndex(e => e.NumeroLicencia).IsUnique();
                entity.HasIndex(e => e.Cedula).IsUnique();
            });
        }

        private void ConfigurePaciente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.ContactoEmergencia).HasMaxLength(100);
                entity.Property(e => e.TelefonoEmergencia).HasMaxLength(20);

                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.Sexo).IsRequired().HasMaxLength(10);

                entity.HasIndex(e => e.Cedula).IsUnique();
            });
        }

        private void ConfigureDisponibilidad(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Disponibilidad>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DiaSemana).IsRequired();
                entity.Property(e => e.HoraInicio).IsRequired();
                entity.Property(e => e.HoraFin).IsRequired();
                entity.Property(e => e.EsActivo).IsRequired();

                entity.HasOne(e => e.Medico)
                      .WithMany(m => m.Disponibilidades)
                      .HasForeignKey(e => e.MedicoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureRecordatorio(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recordatorio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaEnvio).IsRequired();
                entity.Property(e => e.TipoNotificacion).IsRequired();
                entity.Property(e => e.Mensaje).HasMaxLength(500);
                entity.Property(e => e.FueEnviado).IsRequired();

                entity.HasOne(e => e.Cita)
                      .WithMany(c => c.Recordatorios)
                      .HasForeignKey(e => e.CitaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureUsuario(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EsActivo).IsRequired();

                entity.HasIndex(e => e.NombreUsuario).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }

        private void ConfigureRol(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);

                entity.HasIndex(e => e.Nombre).IsUnique();
            });
        }

        private void ConfigureUsuarioRol(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioRol>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.UsuarioRoles)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.UsuarioRoles)
                      .HasForeignKey(e => e.RolId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UsuarioId, e.RolId }).IsUnique();
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador", Descripcion = "Acceso completo al sistema", FechaCreacion = DateTime.Now },
                new Rol { Id = 2, Nombre = "Medico", Descripcion = "Acceso para medicos", FechaCreacion = DateTime.Now },
                new Rol { Id = 3, Nombre = "Recepcionista", Descripcion = "Acceso para recepcionistas", FechaCreacion = DateTime.Now }
            );
        }
    }
}