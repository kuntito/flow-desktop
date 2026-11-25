using Microsoft.EntityFrameworkCore;

namespace flow_desktop.Data;

public class ListenHistoryDao
{
    public async Task Insert(ListenHistoryItem listen)
    {
        await using var db = new FlowDb();

        db.ListenHistory.Add(listen);

        await db.SaveChangesAsync();
    }

    public async Task<List<ListenHistoryItem>> GetUnsynced()
    {
        await using var db = new FlowDb();

        var unsyncedListens = await db.ListenHistory
            .Where(item => !item.IsSynced)
            .OrderBy(item => item.Id)
            .ToListAsync();

        return unsyncedListens;
    }

    public async Task<int?> GetCurSeq()
    {
        await using var db = new FlowDb();

        var cursor = await db.ListenHistoryCursor.FindAsync(0);

        return cursor?.CurSeq;
    }
    
    private static async Task MarkSynced(
        FlowDb db,
        List<string> eventIds
    )
    {
        await db.ListenHistory
            .Where(item => eventIds.Contains(item.EventId))
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    item => item.IsSynced,
                    true
                )
            );
    }
    
    private static async Task InsertListens(
        FlowDb db,
        List<ListenHistoryItem> listensToInsert
    )
    {
        var eventIds = listensToInsert
            .Select(item => item.EventId)
            .ToList();

        var existingEventIds = await db.ListenHistory
            .Where(item => eventIds.Contains(item.EventId))
            .Select(item => item.EventId)
            .ToListAsync();
        var existingEventIdSet = existingEventIds.ToHashSet();

        var newListens = new List<ListenHistoryItem>();

        foreach (var listen in listensToInsert)
        {
            var isExisting = existingEventIdSet.Contains(listen.EventId);
            if (isExisting)
            {
                // ...
            } else
            {
                newListens.Add(listen);
            }
        }

        db.ListenHistory.AddRange(newListens);
    }
    
    private static async Task SetCursor(
        FlowDb db,
        int curSeq
    )
    {
        var cursor = await db.ListenHistoryCursor.FindAsync(0);

        if (cursor == null)
        {
            db.ListenHistoryCursor.Add(
                new ListenHistoryCursor
                {
                    Id = 0,
                    CurSeq = curSeq,
                }
            );
        }
        else
        {
            cursor.CurSeq = curSeq;
        }
    }
    
    public async Task ApplySync(
        List<string> idsExistingListens,
        List<ListenHistoryItem> newListens,
        int curSeq
    )
    {
        await using var db = new FlowDb();
        await using var transaction = await db.Database.BeginTransactionAsync();

        await MarkSynced(db, idsExistingListens);
        await InsertListens(db, newListens);
        await SetCursor(db, curSeq);

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}