using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StickerAlbum.Models
{
    public class Stickers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StickerID { get; set; }
        public string? Image { get; set; }
        public int? Number { get; set; }
        public string? Country { get; set; }
    }
}
