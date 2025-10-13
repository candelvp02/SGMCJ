using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Domain.Interfaces.Repositories;
using SGMCJ.Domain.Repositories;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Domain.Repositories.Security;
using SGMCJ.Persistence.Ado.Medical;
using SGMCJ.Persistence.Ado.Security;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Common;
using SGMCJ.Persistence.Context;
using SGMCJ.Persistence.Repositories.Medical;
using SGMCJ.Persistence.Repositories.Security;

namespace SGMCJ.Persistence
{
    public static class Persistence
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<SGMCJDbContext>(options =>
                options.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICitaRepository, CitaRepository>();
            services.AddScoped<IMedicoRepository, MedicoRepository>();
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            services.AddScoped<StoredProcedureExecutor>();
            services.AddScoped<IUsuarioAdoRepository, UsuarioAdoRepository>();
            services.AddScoped<IMedicoAdoRepository, MedicoAdoRepository>();
            services.AddScoped<IPacienteAdoRepository, PacienteAdoRepository>();
            services.AddScoped<ICitaAdoRepository, CitaAdoRepository>();
            
            return services;
        }
    }
}