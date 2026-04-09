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

public sealed partial class AudioControlSection : UserControl
{
    public AudioControlSection()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty IsPlayingProperty =
        DependencyProperty.Register(
            nameof(IsPlaying),
            typeof(bool),
            typeof(PrevPlayPauseNextButtons),
            new PropertyMetadata(false)
        );

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public event EventHandler? OnPlay;
    public event EventHandler? OnPause;

    private void HandlePlay(object sender, EventArgs e)
    {
        OnPlay?.Invoke(this, EventArgs.Empty);
    }

    private void HandlePause(object sender, EventArgs e)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
}
