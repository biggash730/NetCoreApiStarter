using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var adminClaims = new List<string>
                        {
                            Privileges.CanViewDashboard,
                            Privileges.CanViewAdministration,
                            Privileges.CanViewSettings,
                            Privileges.CanViewReports,
                            Privileges.CanViewRoles,
                            Privileges.CanCreateRoles,
                            Privileges.CanUpdateRoles,
                            Privileges.CanDeleteRoles,
                            Privileges.CanViewUsers,
                            Privileges.CanCreateUsers,
                            Privileges.CanUpdateUsers,
                            Privileges.CanDeleteUsers,
                            Privileges.CanCreateSettings,
                            Privileges.CanUpdateSettings,
                            Privileges.CanDeleteSettings
                };
                var adminRole = new Role { Name = "Administrator", NormalizedName = "Administrator", Locked = true};
                var existingRole = context.Roles.FirstOrDefault(x => x.Name == adminRole.Name);
                if (existingRole == null)
                {
                    var res = roleManager.CreateAsync(adminRole);
                    if (res.Result.Succeeded)
                    {
                        adminClaims.Distinct().ToList().ForEach(r => roleManager.AddClaimAsync(adminRole,
                        new Claim(GenericProperties.Privilege, r)).Wait());
                    }
                }
                else
                {
                    foreach (var r in adminClaims)
                    {
                        var exst = context.RoleClaims.FirstOrDefault(x => x.RoleId == existingRole.Id && r == x.ClaimValue);
                        if (exst == null)
                        {
                            var newClaim = new RoleClaim { RoleId = existingRole.Id, ClaimValue = r, ClaimType = GenericProperties.Privilege };
                            context.RoleClaims.Add(newClaim);
                            context.SaveChanges();
                            var a = newClaim;
                        }
                    }
                }
                #endregion

                #region Users
                var adminUser = new User
                {
                    Name = "System Administrator",
                    UserName = "Admin"
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
