using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace flow_desktop.Data;

[Table("listen_history")]
[Index(nameof(EventId), IsUnique = true)]
public class ListenHistoryItem
{
    [Key]
    public int Id { get; set; }

    public string EventId { get; set; } = string.Empty;

    public int SongId { get; set; }

    public long ListenedAtMillis { get; set; }

    public bool IsSynced { get; set; }
}