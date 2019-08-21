using NetCoreStarter.Shared.Classes;
using System.Linq;

namespace NetCoreStarter.Shared.Filters
{
    public class UserFilter : Filter<User>
    {
        //public string Role;
        public string Username;

        public override IQueryable<User> BuildQuery(IQueryable<User> query)
        {
            //if (!string.IsNullOrEmpty(Role)) query = query.Where(q => q.UserRoles.Select(x=>x.Role.Name).ToList().Include(Role == ProfileId));
            if (!string.IsNullOrEmpty(Username)) query = query.Where(q => q.UserName.ToLower().Contains(Username.ToLower()));

            query = query.Where(q => !q.Hidden);
            return query;
        }
    }
}
