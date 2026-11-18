using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using flow_desktop.Models;

namespace flow_desktop.Helpers;

public class SongSearchManager: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private SongSearchState _songSearchState = new SongSearchState.Idle();

    public SongSearchState SongSearchState
    {
        get => _songSearchState;
        private set
        {
            _songSearchState = value;
            OnPropertyChanged(nameof(SongSearchState));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }

    private readonly Func<string, CancellationToken, Task<List<SongSearchItem>?>> _searchSong;
    public SongSearchManager(
        Func<string, CancellationToken, Task<List<SongSearchItem>?>> searchSong
    )
    {
        _searchSong = searchSong;
    }


    private CancellationTokenSource? _searchCts;
    public async Task SearchForSong(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            SongSearchState = new SongSearchState.Idle();
            return;
        }
        
        CancellationTokenSource? previousCts = _searchCts;
        // if there's a previous search request, cancel it.
        if (previousCts != null)
        {
            await previousCts.CancelAsync();
        }
        
        _searchCts = new CancellationTokenSource();
        var cancelToken = _searchCts.Token;

        SongSearchState = new SongSearchState.Searching();

        var songSearchResults = await _searchSong(query, cancelToken);

        if (songSearchResults == null)
        {
            SongSearchState = new SongSearchState.Error();
        }
        else if (songSearchResults.Count == 0)
        {
            SongSearchState = new SongSearchState.NoResults();
        }
        else
        {
            SongSearchState = new SongSearchState.FoundResults(
                songSearchResults
            );
        }
    }
}