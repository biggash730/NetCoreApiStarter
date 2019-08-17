using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStarter.Data.Classes
{
    public class User : IdentityUser
    {
        [MaxLength(128), Required]
        public string Name { get; set; }
        public virtual Profile Profile { get; set; }
        public long ProfileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool Locked { get; set; }
        public bool Hidden { get; set; }
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        //{
        //    var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
        //    return userIdentity;
        //}
    }
}
