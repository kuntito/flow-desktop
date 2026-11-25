using System;
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
        LocalCatalogDao localCatalogDao,
        CatalogCache catalogCache,
        ListenHistoryDao listenHistoryDao
    )
    {
        private async Task<Song?> GetSongByIdLocal(int songId)
        {
            LocalCatalog? catalogItem = await localCatalogDao.GetBySongId(songId);
            if (catalogItem == null) return null;
            if (catalogItem.DurationMillis == null) return null;

            string? cachedPath = await catalogCache.GetCachedPath(songId);
            if (cachedPath == null) return null;
            
            var localSong = new Song
            {
                Id = catalogItem.SongId,
                Title = catalogItem.SongTitle,
                ArtistStr = catalogItem.SongArtistName,
                DurationMillis = catalogItem.DurationMillis.Value,
                AlbumArtUrl = catalogItem.AlbumArtUrl,
                CachedFilePath = cachedPath,
            };

            return localSong;
        }
        
        /// <summary>
        /// the point is to attach a song to its filepath
        /// if one exists, else it assigns null.
        ///
        /// and behind the scenes, tries to download the song
        /// so it's ready for next time.
        /// </summary>
        private async Task<Song> AttachFileOrDownload(SongWithUrl songWithUrl)
        {
            string? cachedPath = await catalogCache.GetFilePathOrDownload(
                songId: songWithUrl.Id,
                songUrl: songWithUrl.SongUrl
            );

            Song song = songWithUrl.ToSong();
            song.CachedFilePath = cachedPath;

            return song;
        }
        
        /// <summary>
        /// returns a specific song.
        /// plays from disk if the song is cached, no network needed.
        /// otherwise fetches from the server.
        /// </summary>
        public async Task<Song?> FetchSpecificSong(int songId)
        {
            Song? localSong = await GetSongByIdLocal(songId);
            if (localSong != null) return localSong;
            
            GetSongByIdResponse? response = await flowDS.SafeGetSongById(songId);
            SongWithUrl? songWithUrl = response?.SongWithUrl;
            if (songWithUrl == null) return null;
            
            Song song = await AttachFileOrDownload(songWithUrl);

            return song;
        }

        // TODO, when you implement playlist data living on PC
        //  rather than being fetched, you might wanna get the song id
        //  and use fetch specific song.
        public async Task<Song?> FetchPlaylistSong(int playlistId)
        {
            GetPlaylistNextSongResponse? response = await flowDS.SafeGetPlaylistNextSong(playlistId);
            SongWithUrl? songWithUrl = response?.SongWithUrl;
            if (songWithUrl == null) return null;

            Song song = await AttachFileOrDownload(songWithUrl);

            return song;
        }
        
        /// <summary>
        /// picks the next default song from disk, least recently played first.
        ///
        /// pool size is how many cached songs there must be before this kicks in.
        /// with too few, the same handful would keep coming back around.
        ///
        /// so returns null as something failed or not enough songs.
        /// </summary>
        private async Task<Song?> GetDefaultSongLocal()
        {
            const int poolSize = 100;

            int cachedCount = await catalogCache.Count();
            if (cachedCount < poolSize) return null;

            int? songId = await catalogCache.GetLeastRecentSongId();
            if (songId == null) return null;

            Song? localSong = await GetSongByIdLocal(songId.Value);

            return localSong;
        }

        public async Task<Song?> FetchDefaultSong()
        {
            Song? localSong = await GetDefaultSongLocal();
            if (localSong != null) return localSong;

            GetNextSongResponse? response = await flowDS.SafeFetchNextSong();
            SongWithUrl? songWithUrl = response?.SongWithUrl;
            if (songWithUrl == null) return null;

            Song song = await AttachFileOrDownload(songWithUrl);

            return song;
        }
        
        public async Task UpdateRecency(int songId)
        {
            long recency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await localCatalogDao.SetRecency(
                songId: songId,
                recency: recency
            );
        }
        
        public async Task SyncLocalCatalog()
        {
            List<LocalCatalog> rowsWithRecency = await localCatalogDao.GetAllRecencies();

            var recencyItems = rowsWithRecency
                .Select(row => new SyncRecencyItem(
                    SongId: row.SongId,
                    Recency: row.Recency!.Value
                ))
                .ToList();

            GetCatalogueResponse? response = await flowDS.SafeSyncCatalog(recencyItems);

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

        public async Task<Song> EnrichWithFilepath(Song song)
        {
            string? cachedPath = await catalogCache.GetCachedPath(song.Id);

            var enrichedSong = new Song
            {
                Id = song.Id,
                Title = song.Title,
                ArtistStr = song.ArtistStr,
                DurationMillis = song.DurationMillis,
                AlbumArtUrl = song.AlbumArtUrl,
                SongUrl = song.SongUrl,
                CachedFilePath = cachedPath,
            };

            return enrichedSong;
        }
        
        public async Task LogListen(int songId)
        {
            var listen = new ListenHistoryItem
            {
                EventId = Guid.NewGuid().ToString(),
                SongId = songId,
                ListenedAtMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                IsSynced = false,
            };

            await listenHistoryDao.Insert(listen);
        }
        
        private record SplitListens(
            List<string> IdsExistingListens,
            List<ListenEvent> NewListens
        );

        private static SplitListens SplitReturnedListens(
            List<ListenEvent> returnedListens,
            HashSet<string> eventIdsUnsyncedListens
        )
        {
            var idsExistingListens = new List<string>();
            var newListens = new List<ListenEvent>();

            foreach (var listen in returnedListens)
            {
                if (eventIdsUnsyncedListens.Contains(listen.EventId))
                {
                    idsExistingListens.Add(listen.EventId);
                }
                else
                {
                    newListens.Add(listen);
                }
            }

            return new SplitListens(
                IdsExistingListens: idsExistingListens,
                NewListens: newListens
            );
        }
        
        private async Task<bool> SyncListenHistoryBatch(
            List<ListenHistoryItem> batch,
            HashSet<string> eventIdsUnsyncedListens
        )
        {
            var eventsUnsyncedListens = batch
                .Select(item => item.ToListenEvent())
                .ToList();

            int curSeq = await listenHistoryDao.GetCurSeq() ?? 0;

            SyncListenHistoryResponse? response = await flowDS.SafeSyncListenHistory(
                events: eventsUnsyncedListens,
                afterSeq: curSeq
            );
            if (response == null) return false;

            int? latestSeq = response.LatestSeq;
            if (latestSeq == null) return false;

            List<ListenEvent>? returnedListens = response.Events;
            if (returnedListens == null) return false;

            var (idsExistingListens, newListens) = SplitReturnedListens(
                returnedListens: returnedListens,
                eventIdsUnsyncedListens: eventIdsUnsyncedListens
            );

            var itemsNewListens = newListens
                .Select(listen => listen.ToListenHistoryItem(isSynced: true))
                .ToList();

            await listenHistoryDao.ApplySync(
                idsExistingListens: idsExistingListens,
                newListens: itemsNewListens,
                curSeq: latestSeq.Value
            );

            return true;
        }
        
        private readonly SemaphoreSlim _syncListenHistoryLock = new(1, 1);

        public async Task SyncListenHistory()
        {
            const int batchSize = 500;

            bool isLockTaken = await _syncListenHistoryLock.WaitAsync(0);
            if (!isLockTaken) return;

            try
            {
                List<ListenHistoryItem> itemsUnsyncedListens = await listenHistoryDao.GetUnsynced();
                var eventIdsUnsyncedListens = itemsUnsyncedListens
                    .Select(item => item.EventId)
                    .ToHashSet();

                var batches = itemsUnsyncedListens
                    .Chunk(batchSize)
                    .Select(chunk => chunk.ToList())
                    .ToList();

                var batchIndex = 0;
                do
                {
                    var batch = batchIndex < batches.Count
                        ? batches[batchIndex]
                        : new List<ListenHistoryItem>();

                    bool isBatchSynced = await SyncListenHistoryBatch(
                        batch: batch,
                        eventIdsUnsyncedListens: eventIdsUnsyncedListens
                    );
                    if (!isBatchSynced) break;

                    batchIndex++;
                } while (batchIndex < batches.Count);
            }
            finally
            {
                _syncListenHistoryLock.Release();
            }
        }
    }
}