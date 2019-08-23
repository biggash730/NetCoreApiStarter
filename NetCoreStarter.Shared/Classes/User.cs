using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreStarter.Shared.Classes
{
    public class User : IdentityUser
    {
        [MaxLength(128), Required]
        public string OtherNames { get; set; }
        [MaxLength(128), Required]
        public string Surname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime UpdatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public bool IsDeleted { get; set; } = false;
        public bool Locked { get; set; } = true;
        public bool Hidden { get; set; } = false;
        public virtual ICollection<UserClaim> Claims { get; set; }
        public virtual ICollection<UserLogin> Logins { get; set; }
        public virtual ICollection<UserToken> Tokens { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }

}
