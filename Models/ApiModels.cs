using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow_desktop.Models
{
    public class SongWithUrl
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistStr { get; set; } = string.Empty;
        public int DurationMillis { get; set; }
        public string AlbumArtUrl { get; set; }
        public string SongUrl { get; set; } = string.Empty;

        public Song ToSong() => new Song
        {
            Id = Id,
            Title = Title,
            ArtistStr = ArtistStr,
            DurationMillis = DurationMillis,
            AlbumArtUrl = AlbumArtUrl,
            SongUrl = SongUrl,
        };
    }

    public class SongSearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistStr { get; set; } = string.Empty;
        public string AlbumArtUrl { get; set; } = string.Empty;
    }
}
