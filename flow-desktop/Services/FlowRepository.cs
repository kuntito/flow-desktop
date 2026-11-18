using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using flow_desktop.Data;
using flow_desktop.Helpers;
using flow_desktop.Models;

// TODO move other calls to repo
namespace flow_desktop.Services
{
    public class FlowRepository(
        FlowApiDataSource flowDS,
        LocalCatalogDao localCatalogDao
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
        
        public async Task RefreshLocalCatalog()
        {
            GetCatalogueResponse? response = await flowDS.SafeGetCatalogue();

            List<CatalogSong>? catalogSongs = response?.Songs;
            if (catalogSongs == null) return;

            var rows = catalogSongs
                .Select(song => new LocalCatalog
                {
                    SongId = song.Id,
                    SongTitle = song.Title,
                    SongArtistName = song.ArtistStr,
                    AlbumArtUrl = song.AlbumArtUrl,
                    NormalizedTitle = SongSearchNormalizer.Normalize(song.Title),
                    NormalizedArtistName = SongSearchNormalizer.Normalize(song.ArtistStr),
                    DurationMillis = song.DurationMillis,
                    ListenCount = song.ListenCount,
                    Recency = song.Recency,
                })
                .ToList();

            await localCatalogDao.ReplaceAll(rows);
        }
        
        /// <summary>
        /// searches local db for songs.
        /// </summary>
        private async Task<List<SongSearchItem>?> SearchLocal(string query)
        {
            var isCatalogueEmpty = await localCatalogDao.IsEmpty();
            if (isCatalogueEmpty) return null;

            var normalizedQuery = SongSearchNormalizer.Normalize(query);
            if (string.IsNullOrWhiteSpace(normalizedQuery))
            {
                return new List<SongSearchItem>();
            }

            List<LocalCatalog> matches = await localCatalogDao.Search(normalizedQuery);

            var mostListenedFirst = matches.OrderByDescending(song => song.ListenCount ?? 0);

            var searchItems = mostListenedFirst
                .Select(song => new SongSearchItem
                {
                    Id = song.SongId,
                    Title = song.SongTitle,
                    ArtistStr = song.SongArtistName,
                    AlbumArtUrl = song.AlbumArtUrl,
                })
                .ToList();

            return searchItems;
        }

        /// <summary>
        /// searches server db for songs.
        /// </summary>
        private async Task<List<SongSearchItem>?> SearchCloud(
            string query,
            CancellationToken cancelToken
        )
        {
            SearchSongResponse? response = await flowDS.SafeSearchSong(query, cancelToken);

            return response?.searchResults;
        }
        
        public async Task<List<SongSearchItem>?> SearchSong(
            string query,
            CancellationToken cancelToken
        )
        {
            List<SongSearchItem>? localResults = await SearchLocal(query);

            if (localResults == null)
            {
                List<SongSearchItem>? cloudResults = await SearchCloud(query, cancelToken);
                return cloudResults;
            }

            return localResults;
        }
    }
}