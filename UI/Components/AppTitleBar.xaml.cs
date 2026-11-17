
using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using flow_desktop.Models;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class AppTitleBar : UserControl, INotifyPropertyChanged
{
    private static readonly DependencyProperty dependencyPropOnSearch =
        DependencyProperty.Register(
            nameof(OnSearch),
            typeof(Action<string>),
            typeof(AppTitleBar),
            new PropertyMetadata(null)
        );

    public Action<string> OnSearch
    {
        get => (Action<string>)GetValue(dependencyPropOnSearch);
        set => SetValue(dependencyPropOnSearch, value);
    }
    
    private static readonly DependencyProperty dependencyPropOnPlaySong =
        DependencyProperty.Register(
            nameof(OnPlaySong),
            typeof(Action<SongSearchItem>),
            typeof(AppTitleBar),
            new PropertyMetadata(null)
        );

    public Action<SongSearchItem> OnPlaySong
    {
        get => (Action<SongSearchItem>)GetValue(dependencyPropOnPlaySong);
        set => SetValue(dependencyPropOnPlaySong, value);
    }
    
    private static readonly DependencyProperty dependencyPropOnGoToPlaylists =
        DependencyProperty.Register(
            nameof(OnGoToPlaylists),
            typeof(Action),
            typeof(AppTitleBar),
            new PropertyMetadata(null)
        );

    public Action OnGoToPlaylists
    {
        get => (Action)GetValue(dependencyPropOnGoToPlaylists);
        set => SetValue(dependencyPropOnGoToPlaylists, value);
    }
    
    public static readonly DependencyProperty IsPlaylistActiveProperty =
        DependencyProperty.Register(
            nameof(IsPlaylistActive),
            typeof(bool),
            typeof(AppTitleBar),
            new PropertyMetadata(false)
        );

    public bool IsPlaylistActive
    {
        get => (bool)GetValue(IsPlaylistActiveProperty);
        set => SetValue(IsPlaylistActiveProperty, value);
    }

    public static readonly DependencyProperty ActivePlaylistNameProperty =
        DependencyProperty.Register(
            nameof(ActivePlaylistName),
            typeof(string),
            typeof(AppTitleBar),
            new PropertyMetadata("")
        );

    public string ActivePlaylistName
    {
        get => (string)GetValue(ActivePlaylistNameProperty);
        set => SetValue(ActivePlaylistNameProperty, value);
    }

    public static readonly DependencyProperty IsActivePlaylistAutoProperty =
        DependencyProperty.Register(
            nameof(IsActivePlaylistAuto),
            typeof(bool),
            typeof(AppTitleBar),
            new PropertyMetadata(false)
        );

    public bool IsActivePlaylistAuto
    {
        get => (bool)GetValue(IsActivePlaylistAutoProperty);
        set => SetValue(IsActivePlaylistAutoProperty, value);
    }

    private ImageSource IconFor(bool isAuto)
    {
        var iconPath = isAuto
            ? "ms-appx:///Assets/ic_snowflake.png"
            : "ms-appx:///Assets/ic_wavy.png";

        return new BitmapImage(new Uri(iconPath));
    }

    private void ShowHandCursor(object sender, PointerRoutedEventArgs e)
    {
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void ShowDefaultCursor(object sender, PointerRoutedEventArgs e)
    {
        ProtectedCursor = null;
    }
    
    /// <summary>
    /// called when the blinking playlist name is tapped.
    /// </summary>
    public Action? OnEndPlaylist { get; set; }

    private void DeactivatePlaylist(object sender, TappedRoutedEventArgs e)
    {
        ProtectedCursor = null;
        OnEndPlaylist?.Invoke();
    }

    private Visibility VisibleWhenFalse(bool value)
    {
        return value
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
    
    private static readonly DependencyProperty dependencyPropSongSearchState =
        DependencyProperty.Register(
            nameof(SongSearchState),
            typeof(SongSearchState),
            typeof(AppTitleBar),
            new PropertyMetadata(null, OnStateChanged)
        );

    public SongSearchState SongSearchState
    {
        get => (SongSearchState)GetValue(dependencyPropSongSearchState);
        set => SetValue(dependencyPropSongSearchState, value);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var titleBar = (AppTitleBar)d;
        titleBar.PropertyChanged?.Invoke(titleBar, new PropertyChangedEventArgs(nameof(SongSearchState)));
    }
    
    private void GoToPlaylists(object sender, RoutedEventArgs e)
    {
        OnGoToPlaylists?.Invoke();
    }
    
    public AppTitleBar()
    {
        InitializeComponent();
        
        Loaded += (s, e) =>
        {
            PlaylistNameBlink.Begin();
        };
    }
}
