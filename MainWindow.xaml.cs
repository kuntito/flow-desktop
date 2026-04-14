using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using flow_desktop.Helpers;
using flow_desktop.Services;
using flow_desktop.UI.Pages;
using flow_desktop.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public FlowViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            this.AppWindow.Resize(
                new Windows.Graphics.SizeInt32(1280, 1280)
            );

            RootFrame.Navigate(
                typeof(IdleFlowPage),
                ViewModel
            );

            ViewModel.OnFlowStateChanged += (s, flowState) =>
            {

                switch (flowState)
                {
                    case FlowPlaybackState.Loading:
                        RootFrame.Navigate(
                            typeof(LoadingPage)
                        );
                        break;
                    case FlowPlaybackState.Playing:
                        RootFrame.Navigate(
                            typeof(SongPlayingPage),
                            ViewModel
                        );
                        break;
                    case FlowPlaybackState.Error:
                        ErrorSnackBar.FlashInfo("couldn't start flow");
                        RootFrame.Navigate(
                            typeof(IdleFlowPage),
                            ViewModel
                        );
                        break;
                }
            };
        }
    }
}
