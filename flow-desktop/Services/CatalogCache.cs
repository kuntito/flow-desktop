using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using flow_desktop.Data;
using flow_desktop.Helpers;

namespace flow_desktop.Services;

public class CatalogCache
{
    private const int CacheSizeMb = 20 * 1024;

    private readonly CatalogCacheDao _catalogCacheDao;
    private readonly string _cacheDir;
    
    public CatalogCache(CatalogCacheDao catalogCacheDao)
    {
        _catalogCacheDao = catalogCacheDao;

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _cacheDir = Path.Combine(
            localAppData,
            "flow",
            "cache"
        );
        Directory.CreateDirectory(_cacheDir);
    }

    private readonly SemaphoreSlim _evictLock = new(1, 1);

    public async Task MaintainCacheSize()
    {
        await _evictLock.WaitAsync();

        try
        {
            var maxSizeBytes = (long)CacheSizeMb * 1024 * 1024;

            while (await _catalogCacheDao.TotalSize() > maxSizeBytes)
            {
                var leastRecentItem = await _catalogCacheDao.LeastRecent();
                if (leastRecentItem == null) break;

                try
                {
                    File.Delete(leastRecentItem.FilePath);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"maintainCacheSize: couldn't delete {leastRecentItem.FilePath}, {e.Message}");
                    break;
                }

                await _catalogCacheDao.Delete(leastRecentItem.SongId);
            }
        }
        finally
        {
            _evictLock.Release();
        }
    }
    
    public async Task<string?> GetCachedPath(int songId)
    {
        var filePath = await _catalogCacheDao.GetFilePath(songId);
        if (filePath == null) return null;
        
        var isFileOnDisk = File.Exists(filePath);
        if (!isFileOnDisk)
        {
            await _catalogCacheDao.Delete(songId);
            return null;
        }

        var recency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await _catalogCacheDao.SetRecency(
            songId: songId,
            recency: recency
        );

        return filePath;
    }
    
    public async Task<string?> GetFilePathOrDownload(int songId, string songUrl)
    {
        var cachedPath = await GetCachedPath(songId);

        if (cachedPath == null)
        {
            _ = DownloadSong(songId, songUrl);
        }

        return cachedPath;
    }
    
    private readonly object _claimLock = new();
    private readonly HashSet<int> _ongoingDownloads = new();
    
    private async Task DownloadSong(int songId, string songUrl)
    {
        lock (_claimLock)
        {
            var isDownloading = _ongoingDownloads.Contains(songId);
            if (isDownloading) return;

            _ongoingDownloads.Add(songId);
        }

        try
        {
            var destPath = Path.Combine(
                _cacheDir,
                $"{songId}.mp3"
            );

            var downloadedFile = await FileDownloader.DownloadFile(
                url: songUrl,
                destPath: destPath
            );

            if (downloadedFile != null)
            {
                var cacheItem = new CatalogCacheItem
                {
                    SongId = songId,
                    FilePath = downloadedFile.FullName,
                    Recency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    FileSizeBytes = downloadedFile.Length,
                };

                await _catalogCacheDao.Upsert(cacheItem);
                await MaintainCacheSize();
            }
        }
        finally
        {
            lock (_claimLock)
            {
                _ongoingDownloads.Remove(songId);
            }
        }
    }
    
    public async Task<int?> GetLeastRecentSongId()
    {
        CatalogCacheItem? leastRecentItem = await _catalogCacheDao.LeastRecent();

        return leastRecentItem?.SongId;
    }
    
    public async Task<int> Count()
    {
        var count = await _catalogCacheDao.Count();

        return count;
    }
}