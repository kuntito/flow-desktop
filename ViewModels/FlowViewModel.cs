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
using flow_desktop.Helpers;

namespace flow_desktop.ViewModels
{
    public class FlowViewModel: INotifyPropertyChanged
    {
        private SongSearchManager SongSearchManager { get; }
        
        private readonly SongPlayer _songPlayer;
        private readonly FlowApiDataSource _flowDS;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(
            [CallerMemberName] string? name = null
        )
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name)
            );
        }


        private PlayerState _playerState = new();

        public Song? LoadedSong => _playerState.LoadedSong;
        public string SongTitle => _playerState.LoadedSong?.Title ?? "...";
        public string ArtistStr => _playerState.LoadedSong?.ArtistStr ?? "...";

        public bool IsPlaying => _playerState.IsPlaying;
        public float PlayProgress => _playerState.PlayProgress;

        private const string PlaceholderArtUrl = "https://sounds-xyz.s3.eu-north-1.amazonaws.com/albumArt/artworkUnknown.png";
        public string AlbumArtUrl => _playerState.LoadedSong?.AlbumArtUrl ?? PlaceholderArtUrl;

        public SongSearchState SongSearchState => SongSearchManager.SongSearchState;
        
        private List<PlaylistItem> _playlists = new();
        public List<PlaylistItem> Playlists => _playlists;
        public bool IsPlaylistsUnavailable => _playlists.Count == 0;
        
        private static readonly TimeSpan PlaylistDuration = TimeSpan.FromMinutes(90);
        private ActivePlaylist? _activePlaylist = null;

        public Action<string> SearchForSong => async (query) =>
        {
            await SongSearchManager.SearchForSong(query);
        };
        
        
        public FlowViewModel()
        {
            _flowDS = new FlowApiDataSource();
            SongSearchManager = new SongSearchManager(_flowDS);
            SongSearchManager.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SongSearchManager.SongSearchState))
                {
                    OnPropertyChanged(nameof(SongSearchState));
                }
            };
            
 
            _songPlayer = new SongPlayer();
            _songPlayer.OnPlayerStateChanged += (sender, ps) =>
            {
                if (ps == null) return;
                
                _playerState = ps;
                OnPropertyChanged(nameof(LoadedSong));
                OnPropertyChanged(nameof(SongTitle));
                OnPropertyChanged(nameof(ArtistStr));
                OnPropertyChanged(nameof(AlbumArtUrl));

                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(PlayProgress));
            };

            _songPlayer.OnPlaybackComplete += async (sender, e) =>
            {
                await HandleNextSongPlay();
            };
            
            // underscore keeps IDE quiet, behavior stays the same without it
            _ = LoadPlaylists();
        }
        

        private void PlaySong(Song song)
        {
            _songPlayer.Play(song);
        }

        public void PlayLoadedSong()
        {
            _songPlayer.PlayLoadedSong();
        }

        public void PauseSong()
        {
            _songPlayer.Pause();
        }

        public void SeekTo(float progress)
        {
            _songPlayer.SeekTo(progress);
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
        
        public async Task StartPlaylist(PlaylistItem playlist)
        {
            // so the current song goes quiet while the playlist song loads.
            PauseSong();

            var endsAt = DateTime.Now + PlaylistDuration;

            _activePlaylist = new ActivePlaylist(
                PlaylistId: playlist.PlaylistId,
                EndsAt: endsAt
            );

            await StartFlow();
        }
        
        /// <summary>
        /// the active playlist's id, or null if none is running.
        ///
        /// a playlist past its end time is cleared here,
        /// so it also returns null.
        ///
        /// TODO the name says get, but it also clears an ended playlist.
        ///  fine while only FetchNextSong calls it.
        ///  if anything else needs the active playlist, say the UI,
        ///  split it into a plain read and the clearing.
        /// </summary>
        private int? GetActivePlaylistId()
        {
            if (_activePlaylist == null) return null;

            var hasEnded = DateTime.Now >= _activePlaylist.EndsAt;
            if (hasEnded)
            {
                _activePlaylist = null;
                return null;
            }

            return _activePlaylist.PlaylistId;
        }
        

        private async Task<Song?> FetchNextSong()
        {
            int? activePlaylistId = GetActivePlaylistId();

            SongWithUrl? songWithUrl;
            
            if (activePlaylistId == null)
            {
                GetNextSongResponse? response = await _flowDS.SafeFetchNextSong();
                songWithUrl = response?.SongWithUrl;
            }
            else
            {
                GetPlaylistNextSongResponse? response = await _flowDS.SafeGetPlaylistNextSong(
                    activePlaylistId.Value
                );
                songWithUrl = response?.SongWithUrl;
            }

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
        
        /// <summary>
        /// fetches every playlist the API has.
        /// </summary>
        private async Task LoadPlaylists()
        {
            var emptyPlaylists = new List<PlaylistItem>();

            // sync typically replaces api's playlist with the list you send it
            // but if you send an empty list, it returns every playlist it has.
            var response = await _flowDS.SafeSyncPlaylists(emptyPlaylists);

            var playlists = response?.Playlists ?? emptyPlaylists;

            _playlists = playlists
                .OrderByDescending(playlist => playlist.PlaylistId)
                .ToList();

            OnPropertyChanged(nameof(Playlists));
            OnPropertyChanged(nameof(IsPlaylistsUnavailable));
        }

        private bool _isFetchingNextSong = false;
        private async Task HandleNextSongPlay()
        {
            if (_isFetchingNextSong) return;
            _isFetchingNextSong = true;

            try
            {
                // without this, if fetch is slow, the ongoing song keeps playing till next song loads
                // ideally, i want it to stop the moment fetching starts
                PauseSong();
                Song? nextSong = await FetchNextSong();
                if (nextSong != null)
                {
                    PlaySong(nextSong);
                }
            }
            finally
            {
                _isFetchingNextSong = false;
            }
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