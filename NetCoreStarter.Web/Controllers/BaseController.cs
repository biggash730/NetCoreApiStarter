using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;
using NetCoreStarter.Web.Repositories;

namespace NetCoreStarter.Web.Controllers
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
        public async Task<ActionResult> Get()
        {
            var res = _repository.Get();
            if (!res.Any()) return Ok("No data found");
            if (res == null) return NotFound($"Could not find any {_klassName}");
            return Ok(res);
        }
        #endregion

        #region snippet_GetByID
        // GET: api/model/5
        [System.Web.Http.HttpGet]
        public async Task<ActionResult> Get(long id)
        {
            var res = _repository.Get(id);
            if (res == null) return NotFound($"Could not find any {_klassName}");
            return Ok(res);
        }
        #endregion

        #region snippet_Update
        // PUT: api/model/5
        [System.Web.Http.HttpPut]
        public async Task<ActionResult> Put(T model)
        {
            try
            {
                _repository.Update(SetAudit(model));
                return Ok($"{_klassName} Updated Successful");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }
        #endregion

        #region snippet_Create
        // POST: api/model
        [System.Web.Http.HttpPost]
        public async Task<ActionResult> Post(T model)
        {
            try
            {
                _repository.Insert(SetAudit(model,true));
                return Created("", $"{_klassName} Saved Successful");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }
        #endregion

        #region snippet_Delete
        // DELETE: api/model/5
        [System.Web.Http.HttpDelete]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var res = _repository.Get(id);
                if (res == null) return BadRequest($"Could not find the {_klassName}");
                _repository.Delete(id);
                return Ok("Deleted Successful");
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
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
