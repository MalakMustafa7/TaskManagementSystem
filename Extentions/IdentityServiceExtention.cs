using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamTaskManagement.Core.Entities.Identity;
using TeamTaskManagement.Core.Service;
using TeamTaskManagement.Repository.Identity;
using TeamTaskManagement.Service;

namespace TeamTaskManagement.Api.Extentions
{
    public static class IdentityServiceExtention
    {

        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityContext>();

            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                     .AddJwtBearer(Options =>
                     {
                         Options.TokenValidationParameters = new TokenValidationParameters()
                         {
                             ValidateIssuer = true,
                             ValidIssuer = configuration["JWT:ValidIssuer"],
                             ValidateAudience = true,
                             ValidAudience = configuration["JWT:ValidAuddiance"],
                             ValidateLifetime = true,
                             ValidateIssuerSigningKey = true,
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))

                         };
                     });
            return services;
        }
    }
}
