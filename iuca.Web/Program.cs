using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;

namespace iuca.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    ApplicationUserManager<ApplicationUser> userManager = services.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
                    if (!userManager.Users.Any()) 
                    {
                        IUserInfoService userInfoService = services.GetRequiredService<IUserInfoService>();
                        IUserBasicInfoService userBasicInfoService = services.GetRequiredService<IUserBasicInfoService>();
                        INationalityService nationalityService = services.GetRequiredService<INationalityService>();
                        ICountryService countryService = services.GetRequiredService<ICountryService>();
                        IUserRolesService userRolesService = services.GetRequiredService<IUserRolesService>();
                        RoleManager<ApplicationRole> roleManager = services.GetService<RoleManager<ApplicationRole>>();
                        IOrganizationService organizationService = services.GetService<IOrganizationService>();
                        IClaimService claimService = services.GetRequiredService<IClaimService>();
                        IUserTypeOrganizationService userTypeOrganizationService = services.GetRequiredService<IUserTypeOrganizationService>();

                        Seeds.DefaultRoles.SeedAsync(roleManager, organizationService).GetAwaiter().GetResult();
                        Seeds.DefaultUsers.SeedDefaultUsersAsync(userInfoService, userBasicInfoService, nationalityService,
                            countryService, userManager, userRolesService, organizationService, userTypeOrganizationService)
                            .GetAwaiter().GetResult();
                        Seeds.DefaultPermissions.SeedDefaultRolePermissions(organizationService, claimService);
                    }
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging((hostingContext, logging) => 
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                    logging.AddNLog();
                });


       
    }
}
