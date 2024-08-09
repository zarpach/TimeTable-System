using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using iuca.Web.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Authorization;
using iuca.Infrastructure.Identity.Claims;
using Microsoft.AspNetCore.Http.Features;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace iuca.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            //services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddDbContext<ApplicationDbContext>(options =>
                                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                                    mig =>
                                    mig.MigrationsAssembly("iuca.Infrastructure").EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c => c
            .UseNpgsqlConnection(Configuration.GetConnectionString("DefaultConnection"))));

            services.AddHangfireServer();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opts => {
                opts.User.RequireUniqueEmail = true;
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "";
                options.ClientSecret = "";
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://securetoken.google.com/schedule-419211";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/schedule-419211",
                    ValidateAudience = true,
                    ValidAudience = "schedule-419211",
                    ValidateLifetime = true
                };
            });

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            DependencyInjectionConfiguration.Configure(services);
            services.AddAutoMapper(typeof(AppMappingProfile));

            services.AddSingleton(typeof(IConverter), new BasicConverter(new PdfTools()));

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions 
            {
                Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "admin",
                            PasswordClear =  ""
                        }
                    }
                })}
            });
            RecurringJob.AddOrUpdate<AttendanceParsingJob>("Attendance Parsing Job", x => x.Execute(), Cron.Daily);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                options.SameSite = SameSiteMode.Lax;
            }
        }

    }
}
