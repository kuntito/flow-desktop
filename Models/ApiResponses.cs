using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flow_desktop.Models
{
    public class GetNextSongResponse
    {
        public bool Success { get; set; }
        public SongWithUrl? SongWithUrl { get; set; }
        public Dictionary<string, string>? Debug { get; set; }
    }

    public class SearchSongResponse
    {
        public bool Success { get; set; }
        public List<SongSearchItem>? SearchResults { get; set; }
        public Dictionary<string, string>? Debug { get; set; }
    }
}
