using System;

namespace flow_desktop.Models
{
    public record PnqItem(
        SongSearchItem Song,
        string Key
    )
    {
        public PnqItem(SongSearchItem song)
            : this(
                song,
                Guid.NewGuid().ToString()
            )
        {
            
        }
    }
}