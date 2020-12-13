using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Retro_Gamer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;
using Retro_Gamer.Security;
using Retro_Gamer.Services;
using Amazon.S3;

namespace Retro_Gamer
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddDbContextPool<AppDbContext>(
            options => options.UseSqlServer(config.GetConnectionString("GameDbConnection")));
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddScoped<IGameRepository, SQLGameRepository>();
            services.AddScoped<IMemorieRepository, SQLMemorieRepository>();
            services.AddScoped<IPendingGameRepository, SQLPendingGameRepository>();
            services.AddTransient<IAmazonS3Bucket, AmazonS3Bucket>();
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
                options.SignIn.RequireConfirmedEmail = true;

            })
                .AddDefaultTokenProviders()
                   .AddEntityFrameworkStores<AppDbContext>();
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("SuperAdminPolicy",
                    policy => policy.RequireRole("Admin")
                                    .RequireClaim("Create Role", "true")
                                    .RequireClaim("Edit Role", "true")
                                    .RequireClaim("Delete Role", "true"));
                option.AddPolicy("GameEditorPolicy",
                    policy => policy.AddRequirements(new ManageWhoCanManageGamesRequirement()));
                option.AddPolicy("ForbiddenPolicy",
                policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));


            });
            services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            services.AddSingleton<IAuthorizationHandler, ManageGames>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            //add your google authetication information
            services.AddAuthentication()
                     .AddGoogle(option =>
                     {
                         option.ClientId = "Your client id here";
                         option.ClientSecret = "Your client secret here";
                         option.CorrelationCookie.SameSite = SameSiteMode.Lax;
                     });
            var emailConfig = config
                              .GetSection("EmailConfiguration")
                              .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddTransient<IEmailSender, EmailSender>();

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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(route =>
            {
                route.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
