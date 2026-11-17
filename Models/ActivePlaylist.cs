using System;

namespace flow_desktop.Models
{
    public record ActivePlaylist(
        int PlaylistId,
        string PlaylistName,
        bool IsAuto,
        DateTime EndsAt
    );
}