using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using flow_desktop.Models;
using flow_desktop.Services;
using System.Runtime.CompilerServices;

namespace flow_desktop.ViewModels
{
    public class FlowViewModel: INotifyPropertyChanged
    {
        private readonly SongPlayer _songPlayer;
        private readonly FlowApiDataSource _flowDS;

        public event PropertyChangedEventHandler? PropertyChanged;

        private PlayerState _playerState = new();

        public Song? LoadedSong => _playerState.LoadedSong;
        public string SongTitle => _playerState.LoadedSong?.Title ?? "...";
        public string ArtistStr => _playerState.LoadedSong?.ArtistStr ?? "...";

        public bool IsPlaying => _playerState.IsPlaying;
        public float PlayProgress => _playerState.PlayProgress;

        private const string PlaceholderArtUrl = "https://sounds-xyz.s3.eu-north-1.amazonaws.com/albumArt/artworkUnknown.png";
        public string AlbumArtUrl => _playerState.LoadedSong?.AlbumArtUrl ?? PlaceholderArtUrl;

        public FlowViewModel()
        {
            _songPlayer = new SongPlayer();
            _flowDS = new FlowApiDataSource();

            _songPlayer.OnStateChanged += (sender, ps) =>
            {
                if (ps == null) return;
                
                _playerState = ps;
                OnPropertyChanged(nameof(LoadedSong));
                OnPropertyChanged(nameof(SongTitle));
                OnPropertyChanged(nameof(ArtistStr));

                OnPropertyChanged(nameof(IsPlaying));

                OnPropertyChanged(nameof(PlayProgress));
            };
        }

        public void PlaySong(Song song)
        {
            _songPlayer.Play(song);
        }

        public void PauseSong()
        {
            _songPlayer.Pause();
        }

        private void OnPropertyChanged(
            [CallerMemberName] string? name = null
        )
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name)
            );
        }

        private FlowPlaybackState _flowState = FlowPlaybackState.Idle;
        public FlowPlaybackState FlowState => _flowState;

        public event EventHandler<FlowPlaybackState>? OnFlowStateChanged;

        public async Task StartFlow()
        {
            UpdateFlowState(FlowPlaybackState.Loading);

            Song? nextSong = await FetchNextSong();
            if (nextSong != null)
            {
                PlaySong(nextSong);
                UpdateFlowState(FlowPlaybackState.Playing);
            }
        }

        private async Task<Song?> FetchNextSong()
        {
            GetNextSongResponse? response = await _flowDS.SafeFetchNextSong();
            SongWithUrl? songWithUrl = response?.SongWithUrl;

            if (songWithUrl == null)
            {
                UpdateFlowState(FlowPlaybackState.Error);
                return null;
            }

            Song nextSong = songWithUrl.ToSong();
            return nextSong;
        }

        private void UpdateFlowState(FlowPlaybackState newFlowState)
        {
            _flowState = newFlowState;
            OnFlowStateChanged?.Invoke(this, _flowState);
        }
    }
}


public enum FlowPlaybackState
{
    Idle,
    Loading,
    Playing,
    Error
}