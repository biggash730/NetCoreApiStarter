using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }

    public class Role : IdentityRole
    {
        private string v;

        public Role(string v)
        {
            this.v = v;
        }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }

    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual User User { get; set; }
    }

    public class UserLogin : IdentityUserLogin<string>
    {
        public virtual User User { get; set; }
    }

    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual Role Role { get; set; }
    }

    public class UserToken : IdentityUserToken<string>
    {
        public virtual User User { get; set; }
    }
}
