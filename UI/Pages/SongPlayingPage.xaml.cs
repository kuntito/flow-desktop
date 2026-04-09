using flow_desktop.Models;
using flow_desktop.Services;
using flow_desktop.UI.Components;
using flow_desktop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SongPlayingPage : Page
{
    public FlowViewModel? ViewModel { get; set; }

    public SongPlayingPage()
    {
        InitializeComponent();

    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FlowViewModel)e.Parameter;
    }

    // TODO remove this
    private void PlayDummySong()
    {
        ViewModel?.PlaySong(
            Song.DummySong
        );
    }

    private void OnPlay(object sender, EventArgs e)
    {
        PlayDummySong();
    }

    private void OnPause(object sender, EventArgs e)
    {
        ViewModel?.PauseSong();
    }
}
