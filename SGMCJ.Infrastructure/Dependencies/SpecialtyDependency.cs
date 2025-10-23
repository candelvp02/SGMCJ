using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Repositories.Medical;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class SpecialtyDependency
    {
        public static void AddSpecialtyDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddTransient<ISpecialtyService, SpecialtyService>();
        }
    }
}