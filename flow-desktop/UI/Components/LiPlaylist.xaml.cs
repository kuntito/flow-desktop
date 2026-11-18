using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace flow_desktop.UI.Components;


public sealed partial class LiPlaylist : UserControl
{
    public static readonly DependencyProperty PlaylistNameProperty =
        DependencyProperty.Register(
            nameof(PlaylistName),
            typeof(string),
            typeof(LiPlaylist),
            new PropertyMetadata("")
        );

    public string PlaylistName
    {
        get => (string)GetValue(PlaylistNameProperty);
        set => SetValue(PlaylistNameProperty, value);
    }
    
    public static readonly DependencyProperty IsAutoProperty =
        DependencyProperty.Register(
            nameof(IsAuto),
            typeof(bool),
            typeof(LiPlaylist),
            new PropertyMetadata(false)
        );

    public bool IsAuto
    {
        get => (bool)GetValue(IsAutoProperty);
        set => SetValue(IsAutoProperty, value);
    }
    
    private ImageSource IconFor(bool isAuto)
    {
        var iconPath = isAuto
            ? "ms-appx:///Assets/ic_snowflake.png"
            : "ms-appx:///Assets/ic_wavy.png";

        return new BitmapImage(new Uri(iconPath));
    }
    
    public static readonly DependencyProperty PlaylistIdProperty =
        DependencyProperty.Register(
            nameof(PlaylistId),
            typeof(int),
            typeof(LiPlaylist),
            new PropertyMetadata(0)
        );

    public int PlaylistId
    {
        get => (int)GetValue(PlaylistIdProperty);
        set => SetValue(PlaylistIdProperty, value);
    }

    public static readonly DependencyProperty ActivePlaylistIdProperty =
        DependencyProperty.Register(
            nameof(ActivePlaylistId),
            typeof(object),
            typeof(LiPlaylist),
            new PropertyMetadata(null)
        );
    
    // no playlist id is ever below 1, so -1 indicates a missing playlist.
    private const int InvalidPlaylistId = -1;
    public int ActivePlaylistId
    {
        get => GetValue(ActivePlaylistIdProperty) as int? ?? InvalidPlaylistId;
        set => SetValue(ActivePlaylistIdProperty, value);
    }

    private Visibility IconVisibility(int playlistId, int activePlaylistId)
    {
        var isActive = playlistId == activePlaylistId;

        return isActive
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private Visibility BarsVisibility(int playlistId, int activePlaylistId)
    {
        var isActive = playlistId == activePlaylistId;

        return isActive
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
    
    
    public LiPlaylist()
    {
        InitializeComponent();
    }
}