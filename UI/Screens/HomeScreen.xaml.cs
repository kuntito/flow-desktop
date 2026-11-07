using flow_desktop.UI.Pages;
using flow_desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace flow_desktop.UI.Screens;

/// <summary>
/// the home screen.
///
/// holds the title bar, and a frame that switches between
/// idle, loading and song playing as the flow state changes.
///
/// it's cached, so leaving and coming back
/// returns to the same screen, mid-song if a song is playing.
/// </summary>
public sealed partial class HomeScreen : Page
{
    public HomeScreen()
    {
        InitializeComponent();
    }

    public FlowViewModel? ViewModel { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // cached screens get this on every visit,
        // so the setup below only runs on the first.
        var isFirstVisit = ViewModel == null;
        if (!isFirstVisit) return;

        ViewModel = (FlowViewModel)e.Parameter;

        FlowFrame.Navigate(
            typeof(IdleFlowPage),
            ViewModel
        );

        TitleBar.OnGoToPlaylists = () =>
        {
            Frame.Navigate(
                typeof(PlaylistsScreen),
                ViewModel
            );
        };

        ViewModel.OnFlowStateChanged += (s, flowState) =>
        {
            switch (flowState)
            {
                case FlowPlaybackState.Loading:
                    FlowFrame.Navigate(
                        typeof(LoadingPage)
                    );
                    break;
                case FlowPlaybackState.Playing:
                    FlowFrame.Navigate(
                        typeof(SongPlayingPage),
                        ViewModel
                    );
                    break;
                case FlowPlaybackState.Error:
                    ErrorSnackBar.FlashInfo("couldn't start flow");
                    FlowFrame.Navigate(
                        typeof(IdleFlowPage),
                        ViewModel
                    );
                    break;
            }
        };
    }
}