using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow_desktop.Models
{
    public class PlayerState
    {
        public Song? LoadedSong { get; set; }
        public bool IsPlaying { get; set; }
        public int CurrentPositionMs { get; set; }
        public int DurationMs { get; set; }

        public float PlayProgress => DurationMs > 0
            ? (float)CurrentPositionMs / DurationMs
            : 0f;
    }
}
