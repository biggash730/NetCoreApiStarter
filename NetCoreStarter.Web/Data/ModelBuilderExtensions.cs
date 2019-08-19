using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace NetCoreStarter.Web.Data
{
    public static class ModelBuilderExtensions
    {
        
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var roleManager = new RoleManager<Role>();
            var userManager = new UserManager<User>();
            using (var context = new ApplicationDbContext())
            {
                #region Roles
                var adminRole = new Role("Administrator");
                var existingRole = roleManager.FindByNameAsync("Administrator").Result;
                if (existingRole == null)
                {

                    var res = roleManager.CreateAsync(adminRole);
                    if (res.Result.Succeeded)
                    {
                        roleManager.AddClaimAsync(adminRole,
                        new Claim(GenericProperties.Privilege, Privileges.CanViewDashboard));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewReports));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewSettings));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanManageRoles));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewRoles));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanManageUsers));
                        roleManager.AddClaimAsync(adminRole,
                            new Claim(GenericProperties.Privilege, Privileges.CanViewUsers));
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
                        userManager.AddToRoleAsync(user, adminRole.Name);
                    }
                }
                #endregion

                context.SaveChanges();
            }
        }
    }
}