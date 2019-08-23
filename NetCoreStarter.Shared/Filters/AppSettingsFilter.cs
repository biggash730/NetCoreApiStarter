using NetCoreStarter.Shared.Classes;
using System.Linq;

namespace NetCoreStarter.Shared.Filters
{
    public class AppSettingsFilter : Filter<AppSetting>
    {
        public long Id;
        public string Name;

        public override IQueryable<AppSetting> BuildQuery(IQueryable<AppSetting> query)
        {
            if (Id > 0) query = query.Where(q => q.Id == Id);
            if (!string.IsNullOrEmpty(Name)) query = query.Where(q => q.Name.ToLower().Contains(Name.ToLower()));

            query = query.Where(q => !q.IsDeleted);
            return query;
        }
    }
}
