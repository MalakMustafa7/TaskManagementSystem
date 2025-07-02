
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using TeamTaskManagement.Api.Errors;
using TeamTaskManagement.Api.Extentions;
using TeamTaskManagement.Api.MiddelWare;
using TeamTaskManagement.Core.Entities.Identity;
using TeamTaskManagement.Repository.Identity;

namespace TeamTaskManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityService(builder.Configuration);

            builder.Services.AddDbContext<AppIdentityContext>(Options =>
              {
                  Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
              });

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToArray();
                    var validationErrorResponse = new ApiValidationResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };

            });

              //builder.Services.AddControllers().AddJsonOptions(options =>
              //{
              //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
              //});
            var app = builder.Build();

            #region UpdateDataBase
             using var Scope = app.Services.CreateScope();
             var Services = Scope.ServiceProvider;
             var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var identitydbContext = Services.GetRequiredService<AppIdentityContext>();
                await identitydbContext.Database.MigrateAsync();
                var userManager = Services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityContextSeed.seedUserAsync(userManager,roleManager);

            }
            catch (Exception ex) {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An Error Occured During Applying Migration");
            }
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
