using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
using NetCoreStarter.Shared.Filters;
using NetCoreStarter.Utils;
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
                        user = _context.Users.Where(x => x.Id == user.Id).Include(x => x.UserRoles).Include(x => x.Claims).First();
                        var role = _context.Roles.Where(x => x.Id == user.UserRoles.First().RoleId).Include(x => x.RoleClaims).First();
                        var roleClaims = role.RoleClaims.Select(x => x.ClaimValue).Distinct().ToList();

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var nowUtc = DateTime.Now.ToUniversalTime();
                        var expires = nowUtc.AddMinutes(double.Parse(configuration["Tokens:ExpiryMinutes"])).ToUniversalTime();

                        var claims = new List<Claim> { new Claim("Id", user.Id), new Claim("UserName", user.UserName) };
                        var token = new JwtSecurityToken(
                        configuration["Tokens:Issuer"],
                        configuration["Tokens:Audience"],
                        claims,
                        expires: expires,
                        signingCredentials: creds);
                        var tokenResponse = new JwtSecurityTokenHandler().WriteToken(token);

                        var data = new
                        {
                            user.Id,
                            Username = user.UserName,
                            user.Name,
                            user.Email,
                            user.PhoneNumber,
                            Role = role.Name,
                            Claims = roleClaims,
                            Token = tokenResponse
                        };

                        return Ok(new
                        {
                            data,
                            message = "Login Successful"
                        });
                    }

                    return BadRequest("Invalid Username or Password");
                }

                return BadRequest("Invalid Username or Password");
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
                return Ok(new { Message = "User Logged Out" });
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
                var roleRepo = new RoleRepository(_context);
                var role = roleRepo.GetByName(model.Role);
                //todo: do validations
                model.Id = Guid.NewGuid().ToString();
                model.Locked = false;
                var result = await userManager.CreateAsync(model, model.Password).ConfigureAwait(true);

                if (result.Succeeded)
                {
                    var user = userManager.FindByNameAsync(model.UserName).Result;
                    //Add to role
                    roleRepo.AddToRole(user.Id, role.Name);
                    //var rslt = userManager.AddToRoleAsync(user, model.Role);
                }
                else
                    return BadRequest(WebHelpers.ProcessException(result));

                return Created("CreateUser", new { model.Id, Message = "User has been created Successfully" });
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

                user.Name = model.Name;
                user.UpdatedAt = DateTime.Now.ToUniversalTime();
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                //user.Locked = false;
                new UserRepository(_context).Update(user);

                //Remove old role
                roleRepo.RemoveFromAllRoles(user.Id);

                //Add to role
                roleRepo.AddToRole(user.Id, role.Name);

                return Created("UpdateUser", new { user.Id, Message = "User has been updated successfully" });
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
                var res = _context.Users.Include(x => x.UserRoles).ToList();
                var data = res.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.UserName,
                    Role = new RoleRepository(_context).GetById(x.UserRoles.FirstOrDefault()?.RoleId)?.Name,
                }).OrderBy(x => x.Name).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPost]
        [Route("QueryUsers")]
        public async Task<ActionResult> QueryUsers(UserFilter filter)
        {
            try
            {
                var res = filter.BuildQuery(_context.Users).Include(x => x.UserRoles).ToList();
                var total = res.Count();
                if (filter.Pager.Page > 0)
                    res = res.Skip(filter.Pager.Skip()).Take(filter.Pager.Size).ToList();
                if (!res.Any()) return NotFound(new { Message = "No User Found" });
                var data = res.Select(x => new
                {
                    x.Id,
                    x.Name,
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

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                new UserRepository(_context).Delete(id);
                return Ok(new { Message = "User Deleted Successfully." });
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
                var data = _context.Roles.Where(x => !x.IsDeleted).Include(x => x.RoleClaims).ToList().Select(x => new Role
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName,
                    Claims = x.RoleClaims.Select(c => c.ClaimValue).ToList()
                }).OrderBy(x => x.Name).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpGet]
        [Route("GetRole/{id}")]
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
                model.NormalizedName = model.Name;
                var claims = model.Claims;
                var existingRole = _context.Roles.FirstOrDefault(x => x.Name == model.Name);
                if (existingRole == null)
                {
                    var res = roleManager.CreateAsync(model);
                    if (res.Result.Succeeded)
                    {
                        //var role = _context.Roles.First(x => x.Name == model.Name);
                        foreach (var c in claims)
                        {
                            roleManager.AddClaimAsync(model,
                        new Claim(GenericProperties.Privilege, c)).Wait();
                        }
                    }
                }
                else
                {
                    return BadRequest("There is an existing role with the same name. Consider updating the role.");
                }
                _context.SaveChanges();
                return Created("CreateRole", new { model.Id, Message = "Role has been created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<ActionResult> UpdateRole(Role model)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var claims = model.Claims;
                var role = _context.Roles.FirstOrDefault(x => x.Id == model.Id);
                if (role != null)
                {
                    var roleClaims = _context.RoleClaims.Where(x => x.RoleId == model.Id);
                    _context.RoleClaims.RemoveRange(roleClaims);
                    _context.SaveChanges();


                    foreach (var c in claims)
                    {
                        _context.RoleClaims.Add(new RoleClaim
                        {
                            ClaimType = GenericProperties.Privilege,
                            ClaimValue = c,
                            RoleId = model.Id
                        });
                    }

                    role.Name = model.Name;
                    role.NormalizedName = model.Name;
                    role.UpdatedAt = DateTime.Now.ToUniversalTime();
                    _context.SaveChanges();
                }
                else
                {
                    return NotFound("Please check the role id.");
                }
                _context.SaveChanges();
                return Created("UpdateRole", new { model.Id, Message = "Role has been updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpDelete]
        [Route("DeleteRole")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var role = _context.Roles.FirstOrDefault(x => x.Id == id);
                if (role != null)
                {
                    var roleClaims = _context.RoleClaims.Where(x => x.RoleId == id);
                    _context.RoleClaims.RemoveRange(roleClaims);

                    _context.Roles.Remove(role);
                }
                else
                {
                    return NotFound("Please check the role id.");
                }
                _context.SaveChanges();
                return Ok(new { Message = "Role Deleted Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateProfile")]
        public async Task<ActionResult> UpdateProfile(User model)
        {
            try
            {
                var uId = User.FindFirst("Id")?.Value;
                var db = _context;
                var user = db.Users.FirstOrDefault(x => x.Id == uId);
                if (user == null) throw new Exception("Could not find user");

                user.Name = model.Name;
                user.UpdatedAt = DateTime.UtcNow;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                db.SaveChanges();

                return Created("UpdateProfile", new { model.Id, Message = "Profile has been updated successfully" });

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
                var uId = User.FindFirst("Id")?.Value;
                var db = _context;
                var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Id == uId);
                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!result.Succeeded) return BadRequest(WebHelpers.ProcessException(result));

                return Ok(new { Message = "Password changed sucessfully." });
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
                return Ok(new { Message = "Password Reset Successful" });
            }
            catch (Exception e)
            {
                return BadRequest(WebHelpers.ProcessException(e));
            }
        }
    }


}