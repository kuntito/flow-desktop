using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace flow_desktop.Data
{
    [Table("local_catalog")]
    public class LocalCatalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SongId { get; set; }

        public string SongTitle { get; set; } = string.Empty;
        public string SongArtistName { get; set; } = string.Empty;
        public string AlbumArtUrl { get; set; } = string.Empty;
        public string NormalizedTitle { get; set; } = string.Empty;
        public string NormalizedArtistName { get; set; } = string.Empty;
        public int? DurationMillis { get; set; }
        public int? ListenCount { get; set; }
        public long? Recency { get; set; }
    }
}