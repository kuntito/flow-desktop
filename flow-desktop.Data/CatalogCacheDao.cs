using Microsoft.EntityFrameworkCore;

namespace flow_desktop.Data;

public class CatalogCacheDao
{
    public async Task<string?> GetFilePath(int songId)
    {
        await using var db = new FlowDb();

        var filePath = await db.CatalogCache
            .Where(item => item.SongId == songId)
            .Select(item => item.FilePath)
            .FirstOrDefaultAsync();

        return filePath;
    }

    // i changed this to set, cause that's what it does.
    //  updates suggests the function adds the new timestamp.
    public async Task SetRecency(int songId, long recency)
    {
        await using var db = new FlowDb();

        await db.CatalogCache
            .Where(item => item.SongId == songId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    item => item.Recency,
                    recency
                )
            );
    }

    public async Task Upsert(CatalogCacheItem cacheItem)
    {
        await using var db = new FlowDb();

        var existingItem = await db.CatalogCache.FindAsync(cacheItem.SongId);

        if (existingItem == null)
        {
            db.CatalogCache.Add(cacheItem);
        }
        else
        {
            existingItem.FilePath = cacheItem.FilePath;
            existingItem.Recency = cacheItem.Recency;
            existingItem.FileSizeBytes = cacheItem.FileSizeBytes;
        }

        await db.SaveChangesAsync();
    }

    public async Task Delete(int songId)
    {
        await using var db = new FlowDb();

        await db.CatalogCache
            .Where(item => item.SongId == songId)
            .ExecuteDeleteAsync();
    }
    
    public async Task<long> TotalSize()
    {
        await using var db = new FlowDb();

        var totalSize = await db.CatalogCache.SumAsync(item => item.FileSizeBytes);

        return totalSize;
    }
    
    public async Task<int> Count()
    {
        await using var db = new FlowDb();

        var count = await db.CatalogCache.CountAsync();

        return count;
    }

    public async Task<CatalogCacheItem?> LeastRecent()
    {
        await using var db = new FlowDb();

        var leastRecentItem = await db.CatalogCache
            .OrderBy(item => item.Recency)
            .FirstOrDefaultAsync();

        return leastRecentItem;
    }
}