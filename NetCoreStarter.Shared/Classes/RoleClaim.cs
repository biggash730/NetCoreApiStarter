using Microsoft.AspNetCore.Identity;

namespace NetCoreStarter.Shared.Classes
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual Role Role { get; set; }
    }
}
