using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;
using NetCoreStarter.Web.Repositories;

namespace NetCoreStarter.Web.ApiControllers
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
        private readonly IConfiguration configuration;

        public AccountController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            userManager = serviceProvider.GetService<UserManager<User>>();
            roleManager = serviceProvider.GetService<RoleManager<Role>>();
            _context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            signInManager = serviceProvider.GetService<SignInManager<User>>();
            this.configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, model.RememberMe);

                    if (result.Succeeded)
                    {
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var nowUtc = DateTime.Now.ToUniversalTime();
                        var expires = nowUtc.AddMinutes(double.Parse(configuration["Tokens:ExpiryMinutes"])).ToUniversalTime();

                        var token = new JwtSecurityToken(
                        configuration["Tokens:Issuer"],
                        configuration["Tokens:Audience"],
                        null,
                        expires: expires,
                        signingCredentials: creds);

                        var tokenResponse = new JwtSecurityTokenHandler().WriteToken(token);

                        user = _context.Users.Where(x => x.Id == user.Id).Include(x => x.UserRoles).Include(x => x.Claims).First();
                        var role = _context.Roles.Where(x=> x.Id == user.UserRoles.First().RoleId).Include(x=> x.RoleClaims).First();

                        var data = new
                        {
                            user.Id,
                            Username = user.UserName,
                            user.OtherNames,
                            user.Surname,
                            user.Email,
                            user.PhoneNumber,
                            Role = role.Name,
                            Claims = role.RoleClaims.Select(x=> x.ClaimValue).Distinct().ToList(),
                            Token = tokenResponse
                        };

                        return Ok(new
                        {
                            data,
                            message = "Login Successful"
                        });
                    }

                    return BadRequest();
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(WebHelpers.ProcessException(e));
            }
        }


        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult> Logout()
        {            
            try
            {
                await signInManager.SignOutAsync();
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
                var roleRepo = new RoleRepository(_context);
                var user = new UserRepository(_context).Get(model.UserName);
                var role = roleRepo.GetByName(model.Role);

                if (user == null) return NotFound("Updating user not found. Please update an existing user");
                
                user.OtherNames = model.OtherNames;
                user.OtherNames = model.OtherNames;
                user.UpdatedAt = DateTime.Now.ToUniversalTime();
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                new UserRepository(_context).Update(user);

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
        [AllowAnonymous]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var res = _context.Users.Include(x=> x.UserRoles).ToList();
                var data = res.Select(x => new
                    {
                        x.Id,
                        x.OtherNames,
                        x.Surname,
                        x.Email,
                        x.PhoneNumber,
                        x.UserName,
                        Role = new RoleRepository(_context).GetById(x.UserRoles.First().RoleId)?.Name,
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        [HttpPost]
        [Route("CreateRole")]
        public async Task<ActionResult> CreateRole(Role model)
        {
            try
            {
                new RoleRepository(_context).Insert(model);
                return Created("","Role Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<ActionResult> UpdateProfile(User model)
        {
            try
            {
                if (model == null) throw new Exception("Please check the data entered");
                var userId = User.Identity.AsAppUser(_context).Result.Id;
                var db = _context;
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
        
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                new UserRepository(_context).Delete(id);
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
                var user = User.Identity.AsAppUser(_context).Result;
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
                if (us == null) return NotFound("Unknown Username");
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