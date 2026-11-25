using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace flow_desktop.Data
{
    public class FlowDb : DbContext
    {
        public DbSet<LocalCatalog> LocalCatalog => Set<LocalCatalog>();
        public DbSet<CatalogCacheItem> CatalogCache { get; set; }
        public DbSet<ListenHistoryItem> ListenHistory { get; set; }
        public DbSet<ListenHistoryCursor> ListenHistoryCursor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var localAppData = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            );

            var flowFolder = Path.Combine(
                localAppData,
                "flow"
            );

            Directory.CreateDirectory(flowFolder);

            var dbPath = Path.Combine(
                flowFolder,
                "flow.db"
            );

            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}