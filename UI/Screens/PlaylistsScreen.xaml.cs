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
    
    private bool IsActivePlaylist(PlaylistItem playlist)
    {
        return playlist.PlaylistId == ViewModel?.ActivePlaylistId;
    }


    private void OnPlaylistMenuOpening(object sender, object e)
    {
        var menu = (MenuFlyout)sender;
        var activateItem = (MenuFlyoutItem)menu.Items[0];
        var deactivateItem = (MenuFlyoutItem)menu.Items[1];
        
        var playlist = activateItem.Tag as PlaylistItem;
        if (playlist == null) return;

        var isActive = IsActivePlaylist(playlist);

        activateItem.Visibility = isActive
            ? Visibility.Collapsed
            : Visibility.Visible;

        deactivateItem.Visibility = isActive
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
    
    
    private void OnActivatePlaylistClick(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as FrameworkElement;
        var playlist = menuItem?.Tag as PlaylistItem;

        if (playlist == null) return;

        ViewModel?.ActivatePlaylist(playlist);
    }
    

    private void OnDeactivatePlaylistClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.EndPlaylist();
    }
}