using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.System;
using SGMCJ.Infrastructure.Services;
using SGMCJ.Persistence.Repositories.System;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class NotificationDependency
    {
        public static void AddNotificationDependencies(this IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddScoped<INotificationService, MockNotificationService>();

        }
    }
}