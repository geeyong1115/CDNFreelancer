using CDNFreelancer.Data;
using CDNFreelancer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace CDNFreelancer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersAPIController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<User>>> GetUsers([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync(); // Get the total count of users
                var users = await _context.Users
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

                var result = new PagedResult<User>
                {
                    Results = users,
                    TotalCount = totalUsers
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Find(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Find), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the user: {ex.Message}");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

}
