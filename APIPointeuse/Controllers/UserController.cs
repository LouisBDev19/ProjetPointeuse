using APIPointeuse.Data;
using APIPointeuse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminEndPoint()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                return Ok($"Hi, you are an {currentUser.Role}");
            }
            else
            {
                return BadRequest("Unable to determine the current user.");
            }
        }

        private async Task<User> GetCurrentUserAsync()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var username = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    // Recherchez l'utilisateur dans la base de données
                    return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                }
            }

            return null;
        }
    }
}