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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FlowViewModel)e.Parameter;
        
        TopBar.NavBack = () =>
        {
            Frame.GoBack();
        };
    }
    
    private void OnPlaylistDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var tappedElement = e.OriginalSource as FrameworkElement;
        var playlist = tappedElement?.DataContext as PlaylistItem;

        if (playlist == null) return;

        // TODO start the playlist through the view model.
        Debug.WriteLine($"start playlist: {playlist.Name}");

        Frame.GoBack();
    }
}