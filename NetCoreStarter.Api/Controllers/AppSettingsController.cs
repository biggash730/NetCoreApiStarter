using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace NetCoreStarter.Api.Controllers
{
    public class AppSettingsController: BaseController<AppSetting>
    {
        [HttpPost]
        
        public async Task<ApiResponse> Query()
        {
           ApiResponse res;

            using (var db = new ApplicationDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        res = new ApiResponse(System.Net.HttpStatusCode.OK);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        res = WebHelpers.ProcessException(ex);
                        // TODO: Handle failure
                    }
                }
            }
            return res;
        }
    }
}
