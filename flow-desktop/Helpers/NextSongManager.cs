using System;
using System.Diagnostics;
using System.Threading.Tasks;
using flow_desktop.Models;

namespace flow_desktop.Helpers
{
    public enum NextSongSource
    {
        UserChoice,
        Pnq,
        Playlist,
        ApiDefault,
    }

    public record NextSongItem(
        Song Song,
        NextSongSource Source
    );

    public class NextSongManager(
        Func<int, Task<Song?>> fetchSpecificSong,
        Func<int, Task<Song?>> fetchPlaylistSong,
        Func<Task<Song?>> fetchDefaultSong,
        Func<int?> getActivePlaylistId,
        Func<bool> hasQueuedSong,
        Func<SongSearchItem?> popNextInQueue,
        Func<int, Task> updateRecency
    )
    {
        public async Task<Song?> GetNextSong(int? prioritySongId = null)
        {
            NextSongItem? nextSongItem;

            int? activePlaylistId = getActivePlaylistId();

            if (prioritySongId != null)
            {
                Song? pickedSong = await fetchSpecificSong(prioritySongId.Value);

                nextSongItem = pickedSong == null
                    ? null
                    : new NextSongItem(
                        Song: pickedSong,
                        Source: NextSongSource.UserChoice
                    );
            }             
            else if (hasQueuedSong())
            {
                SongSearchItem nextInQueue = popNextInQueue()!;
                Song? pnqSong = await fetchSpecificSong(nextInQueue.Id);

                nextSongItem = pnqSong == null
                    ? null
                    : new NextSongItem(
                        Song: pnqSong,
                        Source: NextSongSource.Pnq
                    );
            }
            else if (activePlaylistId != null)
            {
                Song? playlistSong = await fetchPlaylistSong(activePlaylistId.Value);

                nextSongItem = playlistSong == null
                    ? null
                    : new NextSongItem(
                        Song: playlistSong,
                        Source: NextSongSource.Playlist
                    );
            }
            else
            {
                Song? defaultSong = await fetchDefaultSong();

                nextSongItem = defaultSong == null
                    ? null
                    : new NextSongItem(
                        Song: defaultSong,
                        Source: NextSongSource.ApiDefault
                    );
            }

            if (nextSongItem != null)
            {
                await updateRecency(nextSongItem.Song.Id);
            }

            Debug.WriteLine($"getNextSong: {nextSongItem?.Song.Title}, source: {nextSongItem?.Source}");

            return nextSongItem?.Song;
        }
    }
}