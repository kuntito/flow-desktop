using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow_desktop.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistStr { get; set; } = string.Empty;
        public int DurationMillis { get; set; }
        public string? AlbumArtUrl { get; set; }
        public string SongUrl { get; set; } = string.Empty;

        public static Song DummySong => new Song
        {
            Id = 0,
            Title = "Sound Helix",
            ArtistStr = "Somebody",
            DurationMillis = 150000,
            AlbumArtUrl = "https://picsum.photos/500",
            SongUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"
        };
    }
}
