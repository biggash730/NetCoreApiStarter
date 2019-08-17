using NetCoreStarter.Data.Classes;
using System;
using System.Linq;

namespace NetCoreStarter.Data.Filters
{
    public class UserFilter : Filter<User>
    {
        public long ProfileId;
        public string Username;

        public override IQueryable<User> BuildQuery(IQueryable<User> query)
        {
            if (ProfileId > 0) query = query.Where(q => q.Profile.Id == ProfileId);
            if (!string.IsNullOrEmpty(Username)) query = query.Where(q => q.UserName.ToLower().Contains(Username.ToLower()));

            query = query.Where(q => !q.Hidden);
            return query;
        }
    }
}
