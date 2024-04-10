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

            // Crear un nuevo álbum para el usuario
            var album = new Albums();
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            // Asignar el ID del álbum al usuario
            user.Album = album;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        //EndPoint para buscar usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUser(int id)
        {
            var user = await _context.Users

            .Include(u => u.Album) 
            .Select(u => new Users 
            {
                UserID = u.UserID,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Password = u.Password,
                Album = new Albums
                {
                    AlbumID = u.Album.AlbumID
                }
            })
            .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        //EndPoint para iniciar sesión
        [HttpPost("Login")]
        public async Task<ActionResult<Users>> Login (string email, string password)
        {
            var user = await _context.Users

            .Include(u => u.Album) // Incluye el álbum del usuario en la consulta
            .Select(u => new Users // Selecciona solo los campos necesarios
            {
                UserID = u.UserID,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Password = u.Password,
                Album = new Albums {
                    AlbumID = u.Album.AlbumID
                } 
            })
            .FirstOrDefaultAsync(u => u.Email == email);

            // Verificar si el usuario existe y si la contraseña coincide
            if (user == null || user.Password != password)
            {
                return NotFound("Correo electrónico o contraseña incorrectos.");
            }

            // Devolver el usuario como respuesta
            return Ok(user);
        }

    }
}