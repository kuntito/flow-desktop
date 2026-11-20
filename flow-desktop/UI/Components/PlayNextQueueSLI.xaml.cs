using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace flow_desktop.UI.Components;

public sealed partial class PlayNextQueueSLI : UserControl
{
    public static readonly DependencyProperty SongTitleProperty =
        DependencyProperty.Register(
            nameof(SongTitle),
            typeof(string),
            typeof(PlayNextQueueSLI),
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
            typeof(PlayNextQueueSLI),
            new PropertyMetadata(string.Empty)
        );

    public string ArtistStr
    {
        get => (string)GetValue(ArtistStrProperty);
        set => SetValue(ArtistStrProperty, value);
    }
    
    public event EventHandler? OnRemove;

    private void Remove(object sender, RoutedEventArgs e)
    {
        OnRemove?.Invoke(this, EventArgs.Empty);
    }


    public PlayNextQueueSLI()
    {
        InitializeComponent();
    }
}