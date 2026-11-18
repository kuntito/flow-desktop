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
    }
}