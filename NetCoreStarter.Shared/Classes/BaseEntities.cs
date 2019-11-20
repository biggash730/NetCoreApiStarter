using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreStarter.Shared.Classes
{
    public class HasId
    {
        public long Id { get; set; }
    }

    public class AuditFields : HasId
    {
        //[Required]
        public string CreatedBy { get; set; }
        //[Required]
        public string ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool Locked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }

    public class LookUp : AuditFields
    {
        [MaxLength(512), Required, Index(IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
    }

    public class Role : IdentityRole
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime UpdatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
        [NotMapped]
        public List<string> Claims { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed), NotMapped]
        public bool CanSync => !LastSyncedAt.HasValue || (LastSyncedAt.Value < UpdatedAt);
        public bool Locked { get; set; } = false;
    }

    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual Role Role { get; set; }
    }

    public class User : IdentityUser
    {
        [MaxLength(128), Required]
        public string Name { get; set; }
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

    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual User User { get; set; }
    }

    public class UserLogin : IdentityUserLogin<string>
    {
        public virtual User User { get; set; }
    }

    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class UserToken : IdentityUserToken<string>
    {
        public virtual User User { get; set; }
    }

    public class AppSetting : LookUp
    {
        [MaxLength(512), Required]
        public string Value { get; set; }
    }

}
