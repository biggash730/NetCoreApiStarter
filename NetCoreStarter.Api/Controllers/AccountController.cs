using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreStarter.Shared.Classes;
using NetCoreStarter.Utils;
using NetCoreStarter.Utils.Helpers;
using NetCoreStarter.Web.Models;

namespace NetCoreStarter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("createuser")]
        public async Task<ApiResponse> CreateUser(User model)
        {
            ApiResponse res;
            try
            {
                //model.CreatedAt = DateTime.Now.ToUniversalTime();
                var result = await userManager.CreateAsync(model, model.Password).ConfigureAwait(true);

                if (result.Succeeded)
                {
                    var user = userManager.FindByNameAsync(model.UserName).Result;
                    var rslt = await userManager.AddToRoleAsync(user, model.Role.Name);
                }
                else throw new Exception(result.Errors.ToString());
                res = new ApiResponse(System.Net.HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                res = WebHelpers.ProcessException(ex);
            }
            return res;
        }
    

        //// GET: api/TodoItems
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        //{
        //    return await _context.TodoItems.ToListAsync();
        //}

        //#region snippet_GetByID
        //// GET: api/TodoItems/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        //{
        //    var todoItem = await _context.TodoItems.FindAsync(id);

        //    if (todoItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return todoItem;
        //}
        //#endregion

        //#region snippet_Update
        //// PUT: api/TodoItems/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        //{
        //    if (id != todoItem.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(todoItem).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TodoItemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        //#endregion

        //#region snippet_Create
        //// POST: api/TodoItems
        //[HttpPost]
        //public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        //{
        //    _context.TodoItems.Add(todoItem);
        //    await _context.SaveChangesAsync();

        //    //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        //    return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        //}
        //#endregion

        //#region snippet_Delete
        //// DELETE: api/TodoItems/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        //{
        //    var todoItem = await _context.TodoItems.FindAsync(id);
        //    if (todoItem == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TodoItems.Remove(todoItem);
        //    await _context.SaveChangesAsync();

        //    return todoItem;
        //}
        //#endregion

        //private bool TodoItemExists(long id)
        //{
        //    return _context.TodoItems.Any(e => e.Id == id);
        //}
    }

    
}