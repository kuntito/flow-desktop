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

public sealed partial class AudioControlButtons : UserControl
{
    public AudioControlButtons()
    {
        InitializeComponent();
    }
    
    public event EventHandler? OnToggleRepeat;
    public event EventHandler? OnResetRepeat;
    
    public static readonly DependencyProperty RepeatCountProperty =
        DependencyProperty.Register(
            nameof(RepeatCount),
            typeof(int),
            typeof(AudioControlButtons),
            new PropertyMetadata(0)
        );

    public int RepeatCount
    {
        get => (int)GetValue(RepeatCountProperty);
        set => SetValue(RepeatCountProperty, value);
    }

    private void HandleToggleRepeat(object sender, EventArgs e)
    {
        OnToggleRepeat?.Invoke(this, EventArgs.Empty);
    }
    
    private void HandleResetRepeat(object sender, EventArgs e)
    {
        OnResetRepeat?.Invoke(this, EventArgs.Empty);
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(ContainerWidth),
            typeof(double),
            typeof(SeekBar),
            new PropertyMetadata(0.0)
        );

    public double ContainerWidth
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
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



    public event EventHandler? OnPlayLoadedSong;
    public event EventHandler? OnPause;
    public event EventHandler? OnNext;
    

    private void HandlePlayLoadedSong(object sender, EventArgs e)
    {
        OnPlayLoadedSong?.Invoke(this, EventArgs.Empty);
    }

    private void HandlePause(object sender, EventArgs e)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
    
    private void HandleNext(object sender, EventArgs e)
    {
        OnNext?.Invoke(this, EventArgs.Empty);
    }
}
