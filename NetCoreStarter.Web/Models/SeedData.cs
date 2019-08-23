using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreStarter.Web.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();
            var roleManager = serviceProvider.GetService<RoleManager<Role>>();
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                #region Roles
                var adminRole = new Role { Name = "Administrator", NormalizedName = "Administrator" };
                var existingRole = context.Roles.FindAsync("Administrator").Result;
                if (existingRole == null)
                {
                    var res = roleManager.CreateAsync(adminRole);
                    if (res.Result.Succeeded)
                    {
                        roleManager.AddClaimAsync(adminRole,
                        new Claim(GenericProperties.Privilege, Privileges.CanViewDashboard)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewReports)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewSettings)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanManageRoles)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewRoles)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanManageUsers)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewUsers)).Wait();
                    }
                }
                #endregion

                #region Users
                var adminUser = new User
                {
                    OtherNames = "System",
                    Surname = "Administrator",
                    UserName = "Admin"
                };
                var existingUser = userManager.FindByNameAsync("Admin").Result;
                //Admin User
                if (existingUser == null)
                {
                    var res = userManager.CreateAsync(adminUser, "admin@app");
                    if (res.Result.Succeeded)
                    {
                        var user = userManager.FindByNameAsync("Admin").Result;
                        userManager.AddToRoleAsync(user, adminRole.Name).Wait();
                    }
                }
                #endregion

                context.SaveChanges();
            }
        }

    }
}
