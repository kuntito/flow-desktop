using System;

namespace flow_desktop.Models
{
    public record ActivePlaylist(
        int PlaylistId,
        DateTime EndsAt
    );
}