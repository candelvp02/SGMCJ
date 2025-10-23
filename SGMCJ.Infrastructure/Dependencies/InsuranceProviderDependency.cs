using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Insurance;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Repositories.Insurance;
using SGMCJ.Persistence.Repositories.Users;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class InsuranceProviderDependency
    {
        public static void AddInsuranceProviderDependencies(this IServiceCollection services)
        {
            services.AddScoped<IInsuranceProviderRepository, InsuranceProviderRepository>();
            services.AddTransient<IInsuranceProviderService, InsuranceProviderService>();
        }
    }
}