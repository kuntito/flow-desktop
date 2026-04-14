using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using flow_desktop.Models;
using Microsoft.UI.Xaml;

namespace flow_desktop.Services
{
    public class SongPlayer: IDisposable
    {
        private readonly MediaPlayer _mediaPlayer;


        private PlayerState _playerState = new();
        public PlayerState PlayerState => _playerState;

        private readonly DispatcherTimer _positionTimer;
        public event EventHandler<PlayerState?> OnStateChanged;

        public SongPlayer()
        {
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

        private void UpdatePlayerState(
            Action<PlayerState> updateAction
        )
        {
            updateAction(_playerState);
            OnStateChanged?.Invoke(this, _playerState);
        }

        public void Dispose()
        {
            _mediaPlayer.Dispose();
        }
    }
}
