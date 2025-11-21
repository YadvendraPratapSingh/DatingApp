using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")] // localhost:5001/api/Members
    [ApiController]
    public class MembersController(AppDbContext dbContext) : ControllerBase
    {
        
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ApiUser>>> GetMembers()
        {
            var users = await dbContext.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiUser>> GetUser(string id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            return user;

        }
    }
}
