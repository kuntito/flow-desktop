using System;

namespace flow_desktop.Helpers;

public class PlaybackCheckpoints(
    int songId,
    int durationMs,
    Action<int> onSongListened
)
{
    private bool _isListenLogged = false;

    private int _trackedPositionMs = 0;
    private const int AcceptableGapMs = 3000;
    private const float ListenFraction = 0.75f;
    private readonly float _msRequiredForListen = ListenFraction * durationMs;

    private bool IsSongListened => durationMs > 0 && _trackedPositionMs >= _msRequiredForListen;

    public void UpdateCheckpoints(int playerPositionMs)
    {
        if (playerPositionMs <= _trackedPositionMs)
        {
            _trackedPositionMs = playerPositionMs;
        }
        else if (_trackedPositionMs + AcceptableGapMs >= playerPositionMs)
        {
            _trackedPositionMs = playerPositionMs;
        }

        if (IsSongListened && !_isListenLogged)
        {
            onSongListened(songId);
            _isListenLogged = true;
        }
    }
}