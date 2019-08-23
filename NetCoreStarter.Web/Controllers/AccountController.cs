using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;
using NetCoreStarter.Web.Repositories;

namespace NetCoreStarter.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> signInManager;
        private readonly UserRepository _userRepo = new UserRepository();

        public AccountController(IServiceProvider serviceProvider)
        {
            this.userManager = serviceProvider.GetService<UserManager<User>>();
            this.roleManager = serviceProvider.GetService<RoleManager<Role>>();
            this._context = _userRepo._context;
            this.signInManager = serviceProvider.GetService<SignInManager<User>>();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> Login(LoginModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) throw new Exception("Please check the login details");

        //        var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe);
        //        if(result.S)

        //        if (user == null) throw new Exception("Invalid Username or Password");

        //        var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
        //        authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //        var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

        //        var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
        //        var token = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

        //        var data = new
        //        {
        //            user.Id,
        //            Username = user.UserName,
        //            user.Name,
        //            user.Email,
        //            user.PhoneNumber,
        //            Role = new
        //            {
        //                user.Profile.Id,
        //                user.Profile.Name,
        //                Privileges = user.Profile.Privileges.Split(',')
        //            },
        //            token
        //        };

        //        return Ok(new
        //        {
        //            data,
        //            message = "Login Successful"
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(WebHelpers.ProcessException(e));
        //    }
        //}


        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult> Logout()
        {            
            try
            {
                //var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                //authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return Ok("User Logged Out");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPost]
        [Route("createuser")]
        public async Task<ActionResult> CreateUser(User model)
        {
            try
            {
                //todo: do validations
                var result = await userManager.CreateAsync(model, model.Password).ConfigureAwait(true);

                if (result.Succeeded)
                {
                    var user = userManager.FindByNameAsync(model.UserName).Result;
                    var rslt = userManager.AddToRoleAsync(user, model.Role);
                }
                else 
                    return BadRequest(WebHelpers.ProcessException(result));
                
                return Created("","User has been created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPut]
        [Route("updateuser")]
        public async Task<ActionResult> UpdateUser(User model)
        {
            try
            {
                var roleRepo = new RoleRepository();
                var user = _userRepo.Get(model.UserName);
                var role = roleRepo.GetByName(model.Role);

                if (user == null) return NotFound("Updating user not found. Please update an existing user");
                
                user.OtherNames = model.OtherNames;
                user.OtherNames = model.OtherNames;
                user.UpdatedAt = DateTime.Now.ToUniversalTime();
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                _userRepo.Update(user);

                //Remove old role
                roleRepo.RemoveFromAllRoles(user.Id);

                //Add to role
                roleRepo.AddToRole(user.Id, role.Name);

                return Ok("User Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var data = _userRepo.Get()
                    .Select(x => new
                    {
                        x.Id,
                        x.OtherNames,
                        x.Surname,
                        x.Email,
                        x.PhoneNumber,
                        x.UserName,
                        x.UserRoles.First()?.Role,
                    }).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpGet]
        [Route("GetClaims")]
        public async Task<ActionResult> GetClaims()
        {
            try
            {
                var data = _context.RoleClaims.Select(x => x.ClaimValue).Distinct().ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                var data = _context.Roles.Where(x=> !x.IsDeleted).Include(x=> x.RoleClaims).ToList().Select(x=> new Role
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName,
                    Claims = x.RoleClaims.Select(c=> c.ClaimValue).ToList()
                }).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpGet]
        [Route("GetRole")]
        public async Task<ActionResult> GetRole(string id)
        {
            try
            {
                var data = _context.Roles.Where(x => x.Id == id).Include(x => x.RoleClaims).ToList().Select(x => new Role
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName,
                    Claims = x.RoleClaims.Select(c => c.ClaimValue).ToList()
                }).FirstOrDefault();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateRole")]
        public async Task<ActionResult> CreateRole(Role model)
        {
            try
            {
                new RoleRepository().Insert(model);
                return Created("","Role Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<ActionResult> UpdateProfile(User model)
        {
            try
            {
                var userId = User.Identity.AsAppUser().Result.Id;
                var db = _userRepo._context;
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null) throw new Exception("Could not find user");

                user.OtherNames = model.OtherNames;
                user.Surname = model.Surname;
                user.UpdatedAt = DateTime.UtcNow;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                db.SaveChanges();

                return Ok("Profile Updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }
        
        [Authorize]
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                _userRepo.Delete(id);
                return Ok("User Deleted Successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                var user = User.Identity.AsAppUser().Result;
                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!result.Succeeded) return BadRequest(WebHelpers.ProcessException(result));

                return Ok("Password changed sucessfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPost]
        [Route("resetpassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var us = _context.Users.FirstOrDefault(x => x.UserName == model.UserName && !x.Hidden && !x.IsDeleted);
                if (us == null) throw new Exception("System Error");
                var result = await userManager.RemovePasswordAsync(us);
                if (result.Succeeded)
                {
                    var res = await userManager.AddPasswordAsync(us, model.Password);
                    if (!res.Succeeded) return BadRequest(WebHelpers.ProcessException(res));
                }
                else return BadRequest(WebHelpers.ProcessException(result));
                
                        _context.SaveChanges();
                return Ok("Password Reset Successful");
            }
            catch (Exception e)
            {
                return BadRequest(WebHelpers.ProcessException(e));
            }
        }
    }

    
}