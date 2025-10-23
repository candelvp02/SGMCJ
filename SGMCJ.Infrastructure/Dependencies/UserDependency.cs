using Microsoft.Extensions.DependencyInjection;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Repositories.Users;

namespace SGMCJ.Infrastructure.Dependencies
{
    public static class UserDependency
    {
        public static void AddUserDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}