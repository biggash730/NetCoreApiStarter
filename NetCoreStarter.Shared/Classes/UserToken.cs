using Microsoft.AspNetCore.Identity;

namespace NetCoreStarter.Shared.Classes
{
    public class UserToken : IdentityUserToken<string>
    {
        public virtual User User { get; set; }
    }
}
