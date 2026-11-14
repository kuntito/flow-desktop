using flow_desktop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using flow_desktop.Models;
using Microsoft.UI.Xaml.Input;

namespace flow_desktop.UI.Screens;

/// <summary>
/// lists the playlists.
///
/// double-clicking one starts it,
/// and the back button returns to the home screen.
/// </summary>
public sealed partial class PlaylistsScreen : Page
{
    public PlaylistsScreen()
    {
        InitializeComponent();
    }

    public FlowViewModel? ViewModel { get; set; }
    
    private Visibility VisibleWhenFalse(bool value)
    {
        return value
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FlowViewModel)e.Parameter;
        
        TopBar.NavBack = () =>
        {
            Frame.GoBack();
        };
        
        TopBar.Refresh = () =>
        {
            _ = ViewModel?.LoadPlaylists();
        };
    }
    
    private void OnPlaylistDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var tappedElement = e.OriginalSource as FrameworkElement;
        var playlist = tappedElement?.DataContext as PlaylistItem;

        if (playlist == null) return;

        _ = ViewModel?.StartPlaylist(playlist);
    }
}