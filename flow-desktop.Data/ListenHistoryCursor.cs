using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace flow_desktop.Data;

[Table("listen_history_cursor")]
public class ListenHistoryCursor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; } = 0;

    public int CurSeq { get; set; }
}