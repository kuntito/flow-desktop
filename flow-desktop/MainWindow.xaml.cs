using flow_desktop.UI.Screens;
using flow_desktop.ViewModels;
using Microsoft.UI.Xaml;

namespace flow_desktop
{
    /// <summary>
    /// the app's only window.
    ///
    /// it holds one frame that switches between screens,
    /// starting on the home screen.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public FlowViewModel FlowViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            this.AppWindow.Resize(
                new Windows.Graphics.SizeInt32(1280, 1280)
            );

            ScreenFrame.Navigate(
                typeof(HomeScreen),
                FlowViewModel
            );
        }
    }
}