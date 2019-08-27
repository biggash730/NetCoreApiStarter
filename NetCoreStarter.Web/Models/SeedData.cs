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
                var existingRole = context.Roles.FindAsync(adminRole.Name).Result;
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
                            new Claim(GenericProperties.Privilege, Privileges.CanCreateRoles)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewRoles)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanUpdateRoles)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanDeleteRoles)).Wait();
                        
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanCreateUsers)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewUsers)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanUpdateUsers)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanDeleteUsers)).Wait();

                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanCreateSettings)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewSettings)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanUpdateSettings)).Wait();
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanDeleteSettings)).Wait();
                    }
                }
                #endregion

                #region Users
                var adminUser = new User
                {
                    OtherNames = "System",
                    Surname = "Administrator",
                    UserName = "Admin",
                    
                };
                var existingUser = userManager.FindByNameAsync("Admin").Result;
                //Admin User
                if (existingUser == null)
                {
                    var res = userManager.CreateAsync(adminUser, "Admin@app1");
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
