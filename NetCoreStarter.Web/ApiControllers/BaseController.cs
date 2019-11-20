using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreStarter.Utils;
using NetCoreStarter.Web.Models;
using NetCoreStarter.Web.Repositories;

namespace NetCoreStarter.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController<T> : ControllerBase where T : class
    {
        public readonly ApplicationDbContext _context;
        protected BaseRepository<T> _repository;
        private readonly string _klassName = typeof(T).Name.Humanize(LetterCasing.Title);

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
            _repository = new BaseRepository<T>(context);
        }

        #region snippet_Get
        // GET: api/model
        [HttpGet]
        public virtual async Task<ActionResult> Get()
        {
            var res = _repository.Get();
            if (!res.Any()) return Ok(new { Message = "No Data Found" });
            if (res == null) return NotFound(new { Message = $"Could not find any {_klassName}" });
            return Ok(res);
        }
        #endregion

        #region snippet_GetByID
        // GET: api/model/5
        [HttpGet("{id}")]
        public virtual async Task<ActionResult> Get(long id)
        {
            var res = _repository.Get(id);
            if (res == null) return NotFound(new { Message = $"Could not find any {_klassName}" });
            return Ok(res);
        }
        #endregion

        #region snippet_Update
        // PUT: api/model
        [HttpPut]
        public virtual async Task<ActionResult> Put(T model)
        {
            try
            {
                _repository.Update(SetAudit(model));
                return Created($"Update{_klassName}", new { Message = $"{_klassName} Updated Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }
        #endregion

        #region snippet_Create
        // POST: api/model
        [HttpPost]
        public virtual async Task<ActionResult> Post(T model)
        {
            try
            {
                _repository.Insert(SetAudit(model, true));
                return Created($"Create{_klassName}", new { Message = $"{_klassName} Saved Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(WebHelpers.ProcessException(ex));
            }
        }
        #endregion

        #region snippet_Delete
        // DELETE: api/model/5
        [HttpDelete]
        public virtual async Task<ActionResult> Delete(long id)
        {
            try
            {
                var res = _repository.Get(id);
                if (res == null) return NotFound($"Could not find the {_klassName}");
                _repository.Delete(id);
                return Ok(new { Message = $"{_klassName} Deleted Successful" });
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
            var uName = User.FindFirst("UserName")?.Value;
            if (isNew)
            {
                if (typeof(T).GetProperty(GenericProperties.CreatedBy) != null)
                    typeof(T).GetProperty(GenericProperties.CreatedBy).SetValue(record, uName);
            }

            if (typeof(T).GetProperty(GenericProperties.ModifiedBy) != null)
                typeof(T).GetProperty(GenericProperties.ModifiedBy).SetValue(record, uName);

            return record;
        }
        #endregion
    }
}
