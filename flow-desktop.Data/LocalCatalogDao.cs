using Microsoft.EntityFrameworkCore;

namespace flow_desktop.Data
{
    public class LocalCatalogDao
    {
        public async Task ReplaceAll(List<LocalCatalog> songs)
        {
            using var db = new FlowDb();
            using var transaction = await db.Database.BeginTransactionAsync();

            await db.LocalCatalog.ExecuteDeleteAsync();

            db.LocalCatalog.AddRange(songs);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public async Task<bool> IsEmpty()
        {
            using var db = new FlowDb();

            var hasAnySong = await db.LocalCatalog.AnyAsync();

            return !hasAnySong;
        }

        public async Task<List<LocalCatalog>> Search(string normalizedQuery)
        {
            using var db = new FlowDb();

            var pattern = $"%{normalizedQuery}%";

            var matches = await db.LocalCatalog
                .Where(song =>
                    EF.Functions.Like(song.NormalizedTitle, pattern)
                    || EF.Functions.Like(song.NormalizedArtistName, pattern)
                )
                .ToListAsync();

            return matches;
        }
        
        public async Task<LocalCatalog?> GetBySongId(int songId)
        {
            await using var db = new FlowDb();

            var catalogItem = await db.LocalCatalog.FindAsync(songId);

            return catalogItem;
        }
        
        public async Task SetRecency(int songId, long recency)
        {
            await using var db = new FlowDb();

            await db.LocalCatalog
                .Where(item => item.SongId == songId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        item => item.Recency,
                        recency
                    )
                );
        }

        public async Task<List<LocalCatalog>> GetAllRecencies()
        {
            await using var db = new FlowDb();

            var rowsWithRecency = await db.LocalCatalog
                .Where(item => item.Recency != null)
                .ToListAsync();

            return rowsWithRecency;
        }
    }
}