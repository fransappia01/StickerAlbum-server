using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StickerAlbum.Models
{
    public class Stickers_x_Albums
    {

        public string? Status { get; set; }

        [ForeignKey("Album")]
        public int AlbumID { get; set; }
        public Albums Album { get; set; }

        [ForeignKey("Sticker")]
        public int StickerID { get; set; }
        public Stickers Sticker { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StickerAlbumId { get; set; }
    }
}
