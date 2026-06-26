
using System;
using System.ComponentModel;
using System.Diagnostics;
using flow_desktop.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class AppTitleBar : UserControl, INotifyPropertyChanged
{
    private static readonly DependencyProperty dependencyPropOnSearch =
        DependencyProperty.Register(
            nameof(OnSearch),
            typeof(Action<string>),
            typeof(AppTitleBar),
            new PropertyMetadata(null)
        );

    public Action<string> OnSearch
    {
        get => (Action<string>)GetValue(dependencyPropOnSearch);
        set => SetValue(dependencyPropOnSearch, value);
    }
    
    private static readonly DependencyProperty dependencyPropSongSearchState =
        DependencyProperty.Register(
            nameof(SongSearchState),
            typeof(SongSearchState),
            typeof(AppTitleBar),
            new PropertyMetadata(null, OnStateChanged)
        );

    public SongSearchState SongSearchState
    {
        get => (SongSearchState)GetValue(dependencyPropSongSearchState);
        set => SetValue(dependencyPropSongSearchState, value);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var titleBar = (AppTitleBar)d;
        titleBar.PropertyChanged?.Invoke(titleBar, new PropertyChangedEventArgs(nameof(SongSearchState)));
    }
    
    public AppTitleBar()
    {
        InitializeComponent();
    }
}
