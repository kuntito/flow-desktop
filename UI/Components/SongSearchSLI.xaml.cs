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

public sealed partial class SongSearchSLI : UserControl
{
    public static readonly DependencyProperty SongTitleProperty =
        DependencyProperty.Register(
            nameof(SongTitle),
            typeof(string),
            typeof(SongSearchSLI),
            new PropertyMetadata(string.Empty)
        );

    public string SongTitle
    {
        get => (string)GetValue(SongTitleProperty);
        set => SetValue(SongTitleProperty, value);
    }

    public static readonly DependencyProperty ArtistStrProperty =
        DependencyProperty.Register(
            nameof(ArtistStr),
            typeof(string),
            typeof(SongSearchSLI),
            new PropertyMetadata(string.Empty)
        );

    public string ArtistStr
    {
        get => (string)GetValue(ArtistStrProperty);
        set => SetValue(ArtistStrProperty, value);
    }

    public SongSearchSLI()
    {
        InitializeComponent();

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
}
