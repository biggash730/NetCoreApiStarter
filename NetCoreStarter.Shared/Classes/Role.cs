using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace NetCoreStarter.Shared.Classes
{
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
}
