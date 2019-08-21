using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Web.Repositories;
using System.Security.Principal;
using System.Threading.Tasks;

namespace NetCoreStarter.Web.Models
{
    public static class IdentityExtensions
    {
        public static async Task<User> AsAppUser(this IIdentity identity)
        {
            var user = new UserRepository().Get(identity.Name);
            return await Task.FromResult(user);
        }
    }
}
