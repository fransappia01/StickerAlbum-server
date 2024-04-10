using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StickerAlbum.Models;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using StickerAlbum.Context;

namespace StickerAlbum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AlbumDbContext _context;

        public UsersController(AlbumDbContext context)
        {
            _context = context;

        }

        // EndPoint para registrar usuarios
        [HttpPost("Register")]
        public async Task<ActionResult<Users>> Register(Users user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null) 
            {
                return BadRequest("Ya existe un usuario con ese email registrado. Por favor utilice uno válido.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        //EndPoint para buscar usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


    }
}