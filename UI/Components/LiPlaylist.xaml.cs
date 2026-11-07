using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

    public LiPlaylist()
    {
        InitializeComponent();
    }
}