using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class TapToStartPrompt : UserControl
{
    public TapToStartPrompt()
    {
        InitializeComponent();
        Loaded += (s, e) => BlinkingAnimation.Begin();

        PointerEntered += (s, e) =>
        {
            BlinkingAnimation.Stop();
            TapToStartButton.Opacity = 1;
            ProtectedCursor = InputSystemCursor.Create(
                InputSystemCursorShape.Hand
            );
        };

        PointerExited += (s, e) =>
        {
            BlinkingAnimation.Begin();
            ProtectedCursor = InputSystemCursor.Create(
                InputSystemCursorShape.Arrow
            );
        };


        PointerPressed += (s, e) =>
        {
            TapToStartButton.Opacity = 0.5;
        };

        PointerReleased += (s, e) =>
        {
            TapToStartButton.Opacity = 1;
        };
    }


    private void OnClick(object sender, TappedRoutedEventArgs e)
    {

    }
}
