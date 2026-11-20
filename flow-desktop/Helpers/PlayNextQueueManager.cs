using System;
using System.Collections.Generic;
using System.Linq;
using flow_desktop.Models;

namespace flow_desktop.Helpers
{
    public class PlayNextQueueManager
    {
        public event EventHandler? OnQueueChanged;
        
        private List<PnqItem> _queue = new();
        public IReadOnlyList<PnqItem> Queue => _queue;

        public bool HasNextSong => _queue.Count > 0;

        private void SetQueue(List<PnqItem> someQueue)
        {
            _queue = someQueue;
            OnQueueChanged?.Invoke(this, EventArgs.Empty);
        }

        public SongSearchItem? GetNextSong()
        {
            if (_queue.Count == 0) return null;

            var nextInQueue = _queue[0];

            var restInQueue = _queue
                .Skip(1)
                .ToList();

            SetQueue(restInQueue);

            return nextInQueue.Song;
        }

        public void AddNext(SongSearchItem song)
        {
            var newItem = new PnqItem(song);

            var newQueue = new List<PnqItem> { newItem };
            newQueue.AddRange(_queue);
            
            SetQueue(newQueue);
        }

        public void Include(SongSearchItem song)
        {
            var newQueueItem = new PnqItem(song);

            if (_queue.Count == 0)
            {
                SetQueue(new List<PnqItem> { newQueueItem });
                return;
            }

            var insertIndex = Random.Shared.Next(
                1,
                _queue.Count + 1
            );

            var newQueue = new List<PnqItem>(_queue);
            newQueue.Insert(
                insertIndex,
                newQueueItem
            );
            
            SetQueue(newQueue);
        }

        public void MoveSong(int fromIndex, int toIndex)
        {
            var newQueue = new List<PnqItem>(_queue);

            var movedItem = newQueue[fromIndex];
            newQueue.RemoveAt(fromIndex);
            newQueue.Insert(toIndex, movedItem);
            
            SetQueue(newQueue);
        }

        // TODO impl this in UI, double click to play a song.
        public SongSearchItem? CherryPickAndTrim(int itemIndex)
        {
            var isValidIndex = itemIndex >= 0 && itemIndex < _queue.Count;
            if (!isValidIndex) return null;

            var pickedItem = _queue[itemIndex];

            var restOfQueue = _queue
                .Skip(itemIndex + 1)
                .ToList();
            
            SetQueue(restOfQueue);

            return pickedItem.Song;
        }

        public void Remove(string key)
        {
            var updatedQueue = _queue
                .Where(item => item.Key != key)
                .ToList();
            
            SetQueue(updatedQueue);
        }
        
        public void Clear()
        {
            SetQueue(new List<PnqItem>());
        }
    }
}