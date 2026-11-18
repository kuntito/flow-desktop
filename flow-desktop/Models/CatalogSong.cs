namespace flow_desktop.Models
{
    public class CatalogSong
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistStr { get; set; } = string.Empty;
        public string AlbumArtUrl { get; set; } = string.Empty;
        public int DurationMillis { get; set; }
        public int ListenCount { get; set; }
        public long Recency { get; set; }
    }
}