using NetCoreStarter.Shared.Classes;
using System.Linq;

namespace NetCoreStarter.Shared.Filters
{
    public class UserFilter : Filter<User>
    {
        public string Email;
        public string Username;

        public override IQueryable<User> BuildQuery(IQueryable<User> query)
        {
            if (!string.IsNullOrEmpty(Email)) query = query.Where(q => q.Email.ToLower().Contains(Email.ToLower()));
            if (!string.IsNullOrEmpty(Username)) query = query.Where(q => q.UserName.ToLower().Contains(Username.ToLower()));

            query = query.Where(q => !q.Hidden);
            return query;
        }
    }
}
