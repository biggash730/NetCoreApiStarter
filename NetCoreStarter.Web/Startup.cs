using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreStarter.Web.Data;
using NetCoreStarter.Shared.Classes;
using Newtonsoft.Json;
using NetCoreStarter.Utils;
using System.IO;

namespace NetCoreStarter.Web
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password = new PasswordOptions()
                {
                    RequireDigit = AppConfig.Setting.PasswordPolicies.RequireDigit,
                    RequiredLength = AppConfig.Setting.PasswordPolicies.MinimumLength,
                    RequireLowercase = AppConfig.Setting.PasswordPolicies.RequireLowercase,
                    RequireUppercase = AppConfig.Setting.PasswordPolicies.RequireUppercase,
                    RequireNonAlphanumeric = AppConfig.Setting.PasswordPolicies.RequireNonLetterOrDigit,
                    RequiredUniqueChars = AppConfig.Setting.PasswordPolicies.RequiredUniqueChars
                };

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(600);
                options.Lockout.MaxFailedAccessAttempts = 50;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+?!@#$%^&*";
                options.User.RequireUniqueEmail = false;
            });


            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            LoadSettings();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static void LoadSettings()
        {
            AppConfig.Setting = JsonConvert.DeserializeObject<Setting>
                (File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppConfig.json")));
        }
    }
}
