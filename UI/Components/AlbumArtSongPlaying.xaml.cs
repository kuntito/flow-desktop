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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class AlbumArtSongPlaying : UserControl
{
    public AlbumArtSongPlaying()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty AlbumArtUrlProperty =
        DependencyProperty.Register(
            nameof(AlbumArtUrl),
            typeof(string),
            typeof(AlbumArtSongPlaying),
            new PropertyMetadata(string.Empty)
        );

    public string AlbumArtUrl
    {
        get => (string)GetValue(AlbumArtUrlProperty);
        set => SetValue(AlbumArtUrlProperty, value);
    }
}
