using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using flow_desktop.Models;

// TODO move other calls to repo
namespace flow_desktop.Services
{
    public class FlowRepository(
        FlowApiDataSource flowDS
    )
    {
        public async Task<Song?> FetchSpecificSong(int songId)
        {
            GetSongByIdResponse? response = await flowDS.SafeGetSongById(songId);

            return response?.SongWithUrl?.ToSong();
        }

        public async Task<Song?> FetchPlaylistSong(int playlistId)
        {
            GetPlaylistNextSongResponse? response = await flowDS.SafeGetPlaylistNextSong(playlistId);

            return response?.SongWithUrl?.ToSong();
        }

        public async Task<Song?> FetchDefaultSong()
        {
            GetNextSongResponse? response = await flowDS.SafeFetchNextSong();

            return response?.SongWithUrl?.ToSong();
        }
        
        public async Task<List<CatalogSong>?> GetCatalogue()
        {
            GetCatalogueResponse? response = await flowDS.SafeGetCatalogue();

            return response?.Songs;
        }
        
        public async Task<List<SongSearchItem>?> SearchSong(
            string query,
            CancellationToken cancelToken
        )
        {
            SearchSongResponse? response = await flowDS.SafeSearchSong(query, cancelToken);

            return response?.searchResults;
        }
    }
}