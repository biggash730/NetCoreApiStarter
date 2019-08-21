using Microsoft.AspNetCore.Identity;

namespace NetCoreStarter.Shared.Classes
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual User User { get; set; }
    }
}
