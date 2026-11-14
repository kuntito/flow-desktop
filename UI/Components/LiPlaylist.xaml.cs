using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace flow_desktop.UI.Components;


public sealed partial class LiPlaylist : UserControl
{
    public static readonly DependencyProperty PlaylistNameProperty =
        DependencyProperty.Register(
            nameof(PlaylistName),
            typeof(string),
            typeof(LiPlaylist),
            new PropertyMetadata("")
        );

    public string PlaylistName
    {
        get => (string)GetValue(PlaylistNameProperty);
        set => SetValue(PlaylistNameProperty, value);
    }
    
    public static readonly DependencyProperty IsAutoProperty =
        DependencyProperty.Register(
            nameof(IsAuto),
            typeof(bool),
            typeof(LiPlaylist),
            new PropertyMetadata(false)
        );

    public bool IsAuto
    {
        get => (bool)GetValue(IsAutoProperty);
        set => SetValue(IsAutoProperty, value);
    }
    
    private ImageSource IconFor(bool isAuto)
    {
        var iconPath = isAuto
            ? "ms-appx:///Assets/ic_snowflake.png"
            : "ms-appx:///Assets/ic_wavy.png";

        return new BitmapImage(new Uri(iconPath));
    }

    public LiPlaylist()
    {
        InitializeComponent();
    }
}