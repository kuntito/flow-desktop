using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace flow_desktop.UI.Components;

public sealed partial class TopBarPlaylistsScreen : UserControl
{
    private static readonly DependencyProperty dependencyPropNavBack =
        DependencyProperty.Register(
            nameof(NavBack),
            typeof(Action),
            typeof(TopBarPlaylistsScreen),
            new PropertyMetadata(null)
        );

    public Action NavBack
    {
        get => (Action)GetValue(dependencyPropNavBack);
        set => SetValue(dependencyPropNavBack, value);
    }

    private void OnNavBackClick(object sender, RoutedEventArgs e)
    {
        NavBack?.Invoke();
    }


    public TopBarPlaylistsScreen()
    {
        InitializeComponent();
    }
}