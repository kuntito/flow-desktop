using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using flow_desktop.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;

namespace flow_desktop.Services
{
    public class SongPlayer: IDisposable
    {
        private readonly MediaPlayer _mediaPlayer;
        private readonly DispatcherQueue _dispatcherQueue;


        private PlayerState _playerState = new();
        private PlayerState PlayerState => _playerState;

        private readonly DispatcherTimer _positionTimer;
        public event EventHandler<PlayerState?> OnPlayerStateChanged;
        public event EventHandler? OnPlaybackComplete;

        public SongPlayer()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _mediaPlayer = new MediaPlayer();

            _positionTimer = new DispatcherTimer();
            _positionTimer.Interval = TimeSpan.FromSeconds(1);

            _positionTimer.Tick += (s, e) =>
            {
                UpdatePlayerState(ps =>
                {
                    ps.CurrentPositionMs = (int)_mediaPlayer.Position.TotalMilliseconds;
                });
            };

            
            _mediaPlayer.MediaEnded += (sender, args) =>
            {
                // `_mediaPlayer.MediaEnded` runs on a background thread.
                // the code it executes touches UI and must run on the UI thread.
                // `_dispatcherQueue.TryEnqueue` runs the code on the UI thread.
                _dispatcherQueue.TryEnqueue(() =>
                {
                    _positionTimer.Stop();

                    UpdatePlayerState(ps =>
                    {
                        ps.IsPlaying = false;
                        ps.CurrentPositionMs = 0;
                    });

                    OnPlaybackComplete?.Invoke(this, EventArgs.Empty);
                });
            };
        }

        public void Play(Song song)
        {
            _mediaPlayer.Source = MediaSource.CreateFromUri(
                new Uri(song.SongUrl)
            );
            _mediaPlayer.Play();
            _positionTimer.Start();

            UpdatePlayerState(ps =>
            {
                ps.LoadedSong = song;
                ps.IsPlaying = true;
                ps.DurationMs = song.DurationMillis;
                ps.CurrentPositionMs = 0;
            });

        }

        public void PlayLoadedSong()
        {
            if (PlayerState.LoadedSong != null)
            {
                _mediaPlayer.Play();
                UpdatePlayerState(
                    ps => ps.IsPlaying = true    
                );
                _positionTimer.Start();
            }
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            UpdatePlayerState(ps =>
            {
                ps.IsPlaying = false;
            });

            _positionTimer.Stop();
        }

        public void SeekTo(float progress)
        {
            var durationMs = _playerState.DurationMs;
            if (durationMs > 0)
            {
                var newPositionMs = (int)(progress * durationMs);
                _mediaPlayer.Position = TimeSpan.FromMilliseconds(newPositionMs);
                UpdatePlayerState(ps =>
                {
                    ps.CurrentPositionMs = newPositionMs;
                });
            }
        }
        
        private void UpdatePlayerState(
            Action<PlayerState> updateAction
        )
        {
            updateAction(_playerState);
            OnPlayerStateChanged?.Invoke(this, _playerState);
        }

        public void Dispose()
        {
            _mediaPlayer.Dispose();
        }
    }
}
