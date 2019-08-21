using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreStarter.Shared.Classes;
using System.Security.Claims;

namespace NetCoreStarter.Web.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            
        
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<Role>(
                    new RoleStore<Role>(context),
                    null,
                    null,
                    null,
                    null);
                var userManager = new UserManager<User>(new UserStore<User>(context), null, null, null, null, null, null, null, null);

                #region Roles
                var adminRole = new Role("Administrator");
                var existingRole = context.Roles.FindAsync("Administrator").Result;
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