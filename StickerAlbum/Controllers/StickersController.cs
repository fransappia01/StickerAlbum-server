using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StickerAlbum.Context;
using StickerAlbum.Models;

namespace StickerAlbum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StickersController : ControllerBase
    {
        private readonly AlbumDbContext _context;

        public StickersController(AlbumDbContext context)
        {
            _context = context;
        }

        [HttpGet("RandomImage")]
        public async Task<ActionResult<string[]>> GetRandomImages()
        {
            try
            {
                // Obtener cinco registros aleatorios de la tabla Stickers
                var randomStickers = await _context.Stickers
                    .OrderBy(r => Guid.NewGuid())       //Funcion random
                    .Take(5)
                    .ToListAsync();

                // Extraer las URL de las imágenes
                var imageUrls = randomStickers.Select(s => s.Image).ToArray();

                return imageUrls;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Funcion para guardar figuritas que abro de un sobre
        [HttpPost("SaveStickers")]
        public IActionResult AddToAlbum(List<string> photos, int albumId)
        {
            try
            {
                // Verificar si la lista de fotos está vacía
                if (photos == null || photos.Count == 0)
                {
                    return BadRequest("No photos provided");
                }

                var album = _context.Albums.FirstOrDefault(a => a.AlbumID == albumId);

                if (album == null)
                {
                    return NotFound("Album not found");
                }

                foreach (var photo in photos)
                {
                    var sticker = _context.Stickers.FirstOrDefault(s => s.Image == photo);

                    if (sticker == null)
                    {
                        return NotFound($"Sticker with image {photo} not found");
                    }

                    var existingStickerAlbum = _context.Stickers_x_Albums
                        .FirstOrDefault(sa => sa.StickerID == sticker.StickerID && sa.AlbumID == albumId);

                    if (existingStickerAlbum != null)
                    {
                        // El sticker ya está en el álbum, así que creamos una nueva fila con estado "Repetido"
                        var repeatedStickerAlbum = new Stickers_x_Albums
                        {
                            Status = "Repetido",
                            AlbumID = existingStickerAlbum.AlbumID,
                            StickerID = existingStickerAlbum.StickerID
                        };

                        _context.Stickers_x_Albums.Add(repeatedStickerAlbum);
                    }
                    else
                    {
                        // El sticker no está en el álbum, así que lo agregamos con estado "Guardado"
                        var stickerAlbum = new Stickers_x_Albums
                        {
                            Status = "Guardado",
                            AlbumID = albumId,
                            StickerID = sticker.StickerID
                        };

                        _context.Stickers_x_Albums.Add(stickerAlbum);
                    }
                }
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción inesperada aquí
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Funcion para obtener stickers guardados segun albumId
        [HttpGet("GetSaveStickers")]
        public async Task<ActionResult<IEnumerable<Stickers>>> GetSaveStickersByAlbumId(int albumId)
        {
            // Obtener los stickers guardados por albumId
            var stickers = await _context.Stickers_x_Albums
                .Where(sa => sa.AlbumID == albumId && sa.Status == "Guardado") // Filtrar por albumId y estado "Guardado"
                .Select(sa => sa.Sticker) // Seleccionar solo los stickers relacionados
                .ToListAsync();

            if (stickers == null || !stickers.Any())
            {
                return NotFound("No se encontraron stickers guardados para el album especificado.");
            }

            return Ok(stickers);
        }


        // Endpoint para pegar un sticker guardado en el álbum y actualizar su estado a "Pegado"
        [HttpPost("PasteSticker")]
        public async Task<IActionResult> PasteStickerToAlbum(int stickerId, int albumId)
        {
            try
            {
                // Obtener el mapeo existente del sticker en el álbum
                var existingMapping = await _context.Stickers_x_Albums.FirstOrDefaultAsync(sa => sa.AlbumID == albumId && sa.StickerID == stickerId);

                if (existingMapping == null)
                {
                    return NotFound("No se encontró ningún mapeo para el sticker en el álbum.");
                }

                // Verificar si el mapeo ya está marcado como "Pegado"
                if (existingMapping.Status == "Pegado")
                {
                    // Si ya está marcado como "Pegado", retornamos un mensaje indicando que ya está pegado
                    return BadRequest("El sticker ya está pegado en el álbum.");
                }

                // Actualizar el estado del mapeo a "Pegado" y guardar los cambios en la base de datos
                existingMapping.Status = "Pegado";
                await _context.SaveChangesAsync();

                // Obtener la imagen del sticker mediante una consulta a la tabla de stickers
                var stickerImage = await _context.Stickers.Where(s => s.StickerID == stickerId).Select(s => s.Image).FirstOrDefaultAsync();

                return Ok(new { Message = "Sticker pegado en el álbum exitosamente.", StickerImage = stickerImage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Funcion para obtener stickers guardados segun albumId
        [HttpGet("GetPastedStickers")]
        public async Task<ActionResult<IEnumerable<Stickers>>> GetPastedStickers(int albumId)
        {
            // Obtener los stickers guardados por albumId
            var stickers = await _context.Stickers_x_Albums
                .Where(sa => sa.AlbumID == albumId && sa.Status == "Pegado")
                .Select(sa => sa.Sticker) // Seleccionar solo los stickers relacionados
                .ToListAsync();

            if (stickers == null || !stickers.Any())
            {
                return NotFound("No se encontraron stickers guardados para el album especificado.");
            }

            return Ok(stickers);
        }

        // Funcion para obtener stickers repetidos segun albumId
        [HttpGet("GetRepeatedStickers")]
        public async Task<ActionResult<IEnumerable<Stickers>>> GetRepeatedStickers(int albumId)
        {
            // Obtener los stickers guardados por albumId
            var stickers = await _context.Stickers_x_Albums
                .Where(sa => sa.AlbumID == albumId && sa.Status == "Repetido") // Filtrar por albumId y estado "Guardado"
                .Select(sa => sa.Sticker) // Seleccionar solo los stickers relacionados
                .ToListAsync();

            if (stickers == null || !stickers.Any())
            {
                return NotFound("No se encontraron stickers guardados para el album especificado.");
            }

            return Ok(stickers);
        }

        // Funcion para obtener la imagen del sticker segun StickerID
        [HttpGet("GetImageByStickerId")]
        public async Task<ActionResult<string>> GetImageByStickerId(int stickerId)
        {
            try
            {
                // Buscar el sticker por su ID en la base de datos
                var sticker = await _context.Stickers.FindAsync(stickerId);

                if (sticker == null)
                {
                    return NotFound("No se encontró el sticker con el ID especificado.");
                }

                // Devolver la imagen del sticker
                return Ok(sticker.Image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
