using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace flow_desktop.Data;

[Table("catalog_cache")]
public class CatalogCacheItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int SongId { get; set; }

    public string FilePath { get; set; } = string.Empty;
    
    public long Recency { get; set; }
    
    public long FileSizeBytes { get; set; }
}