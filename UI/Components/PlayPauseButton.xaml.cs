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
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// TODO start here, add cursor on hover of play pause button
namespace flow_desktop.UI.Components
{
    public sealed partial class PlayPauseButton : UserControl
    {
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(
                nameof(IsPlaying),
                typeof(bool),
                typeof(PlayPauseButton),
                new PropertyMetadata(
                    false,
                    new PropertyChangedCallback(OnIsPlayingChanged)
                )
            );

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public PlayPauseButton()
        {
            InitializeComponent();
            UpdatePlayPauseIcon();

            PointerEntered += (s, e) =>
            {
                ProtectedCursor = InputSystemCursor.Create(
                    InputSystemCursorShape.Hand
                );
            };

            PointerExited += (s, e) =>
            {
                ProtectedCursor = InputSystemCursor.Create(
                    InputSystemCursorShape.Arrow
                );
            };
        }

        private static void OnIsPlayingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var button = (PlayPauseButton)d;
            button.UpdatePlayPauseIcon();
        }

        private void UpdatePlayPauseIcon()
        {
            PlayPauseIcon.Source = IsPlaying
                ? new BitmapImage(new Uri("ms-appx:///Assets/ic_pause.png"))
                : new BitmapImage(new Uri("ms-appx:///Assets/ic_play.png"));
        }

        private void OnClick(
            object sender,
            TappedRoutedEventArgs e
        )
        {
            IsPlaying = !IsPlaying;
        }
    }
}
