using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class ReportDependency
    {
        public static void AddReportDependencies(this IServiceCollection services)
        {
            services.AddTransient<IReportService, ReportService>();
        }
    }
}