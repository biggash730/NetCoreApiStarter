using Microsoft.AspNetCore.Identity;

namespace NetCoreStarter.Shared.Classes
{
    public class UserLogin : IdentityUserLogin<string>
    {
        public virtual User User { get; set; }
    }
}
