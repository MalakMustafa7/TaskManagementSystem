using TeamTaskManagement.Api.Helper;
using TeamTaskManagement.Core.Repositories;
using TeamTaskManagement.Core.Service;
using TeamTaskManagement.Repository;
using TeamTaskManagement.Service;

namespace TeamTaskManagement.Api.Extentions
{
    public static class ApplicationServicesExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<ITeamService,TeamService>();
            services.AddScoped<ITaskService,TaskService>();
            return services;
        }
    }
}
