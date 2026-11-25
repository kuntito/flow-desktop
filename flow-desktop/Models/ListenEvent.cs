using flow_desktop.Data;

namespace flow_desktop.Models;

public record ListenEvent(
    string EventId,
    int SongId,
    long ListenedAt
);

public static class ListenEventMapping
{
    public static ListenEvent ToListenEvent(this ListenHistoryItem item) => new ListenEvent(
        EventId: item.EventId,
        SongId: item.SongId,
        ListenedAt: item.ListenedAtMillis
    );

    public static ListenHistoryItem ToListenHistoryItem(
        this ListenEvent listenEvent,
        bool isSynced
    ) => new ListenHistoryItem
    {
        EventId = listenEvent.EventId,
        SongId = listenEvent.SongId,
        ListenedAtMillis = listenEvent.ListenedAt,
        IsSynced = isSynced,
    };
}