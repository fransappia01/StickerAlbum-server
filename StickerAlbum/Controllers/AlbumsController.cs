using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StickerAlbum.Context;
using StickerAlbum.Models;

namespace StickerAlbum.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly AlbumDbContext _context;

        public AlbumsController(AlbumDbContext context)
        {
            _context = context;
        }

    }
}
