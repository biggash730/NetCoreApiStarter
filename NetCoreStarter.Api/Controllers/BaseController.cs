using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Repositories;

namespace NetCoreStarter.Api.Controllers
{
    [System.Web.Http.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController<T> : ControllerBase where T : class
    {
        protected BaseRepository<T> _repository = new BaseRepository<T>();
        private readonly string _klassName = typeof(T).Name.Humanize(LetterCasing.Title);
        
        #region snippet_Get
        // GET: api/model
        [System.Web.Http.HttpGet]
        public async Task<ApiResponse> Get()
        {
            var msg = "Successful";
            var code = HttpStatusCode.OK;
            var total = 0;
            var res = _repository.Get();
            if (!res.Any()) {
                msg = "No data Found";
            }
            return WebHelpers.BuildResponse(code, res, msg, total);
        }
        #endregion

        #region snippet_GetByID
        // GET: api/model/5
        [System.Web.Http.HttpGet]
        public async Task<ApiResponse> Get(long id)
        {
            var res = _repository.Get(id);
            var msg = $"{_klassName} Loaded Successful";
            var code = HttpStatusCode.OK;
            var total = 0;

            if (res == null)
            {
                msg = $"Could not find any {_klassName}";
                code = HttpStatusCode.NotFound;
            }

            return WebHelpers.BuildResponse(code, res, msg, total);
        }
        #endregion

        #region snippet_Update
        // PUT: api/model/5
        [System.Web.Http.HttpPut]
        public async Task<ApiResponse> Put(T model)
        {
            try
            {
                var msg = $"{_klassName} Updated Successful";
                var code = HttpStatusCode.OK;
                var total = 1;

                _repository.Update(SetAudit(model));
                return WebHelpers.BuildResponse(code, null, msg, total);
            }
            catch (Exception ex)
            {
                return WebHelpers.ProcessException(ex);
            }
        }
        #endregion

        #region snippet_Create
        // POST: api/model
        [System.Web.Http.HttpPost]
        public async Task<ApiResponse> Post(T model)
        {
            try
            {
                var msg = $"{_klassName} Saved Successful";
                var code = HttpStatusCode.OK;
                var total = 1;

                _repository.Insert(SetAudit(model,true));
                return WebHelpers.BuildResponse(code, null, msg, total);
            }
            catch (Exception ex)
            {
                return WebHelpers.ProcessException(ex);
            }
        }
        #endregion

        #region snippet_Delete
        // DELETE: api/model/5
        [System.Web.Http.HttpDelete]
        public async Task<ApiResponse> Delete(long id)
        {
            try
            {
                var msg = $"Deleted Successful";
                var code = HttpStatusCode.OK;
                var total = 1;
                var res = _repository.Get(id);
                if (res == null)
                {
                    msg = $"Could not find the {_klassName}";
                    code = HttpStatusCode.NotFound;
                    return WebHelpers.BuildResponse(code, null, msg, total);
                }

                _repository.Delete(id);
                return WebHelpers.BuildResponse(code, null, msg, total);
            }
            catch (Exception ex)
            {
                return WebHelpers.ProcessException(ex);
            }
        }
        #endregion

        #region Set Audit
        protected T SetAudit(T record, bool isNew = false)
        {
            if (isNew)
            {
                if (typeof(T).GetProperty(GenericProperties.CreatedBy) != null)
                    typeof(T).GetProperty(GenericProperties.CreatedBy).SetValue(record, User.Identity.Name);
            }

            if (typeof(T).GetProperty(GenericProperties.ModifiedBy) != null)
                typeof(T).GetProperty(GenericProperties.ModifiedBy).SetValue(record, User.Identity.Name);

            return record;
        }
        #endregion
    }
}
