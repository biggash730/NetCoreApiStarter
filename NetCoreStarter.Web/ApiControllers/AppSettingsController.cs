using Microsoft.AspNetCore.Mvc;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Shared.Filters;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStarter.Web.ApiControllers
{
    public class AppSettingsController : BaseController<AppSetting>
    {
        public AppSettingsController(ApplicationDbContext context) : base(context)
        {
        }

        [HttpPost]
        [Route("query")]
        public async Task<ActionResult> Query(AppSettingsFilter filter)
        {
            using (var db = _repository._context)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var query = _repository.Query(filter);
                        var data = query.OrderBy(x => x.Name).ToList();
                        var total = data.Count();
                        if (filter.Pager.Page > 0)
                            data = data.Skip(filter.Pager.Skip()).Take(filter.Pager.Size).ToList();
                        if (!data.Any()) return Ok("No data found");
                        transaction.Commit();
                        return Ok(new
                        {
                            data,
                            total,
                            message = "Loaded Successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(WebHelpers.ProcessException(ex));
                    }
                }
            }
        }
    }
}
