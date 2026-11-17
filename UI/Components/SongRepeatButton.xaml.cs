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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class SongRepeatButton : UserControl
{
    public SongRepeatButton()
    {
        InitializeComponent();
    }

    public event EventHandler? OnToggleRepeat;
    public event EventHandler? OnResetRepeat;

    public static readonly DependencyProperty RepeatCountProperty =
        DependencyProperty.Register(
            nameof(RepeatCount),
            typeof(int),
            typeof(SongRepeatButton),
            new PropertyMetadata(0)
        );

    public int RepeatCount
    {
        get => (int)GetValue(RepeatCountProperty);
        set => SetValue(RepeatCountProperty, value);
    }

    private ImageSource IconFor(int repeatCount)
    {
        var iconName = repeatCount switch
        {
            > 3 => "ic_repeat_for_a_min",
            3 => "ic_repeat_three",
            2 => "ic_repeat_two",
            1 => "ic_repeat_one",
            _ => "ic_repeat_off",
        };

        return new BitmapImage(new Uri($"ms-appx:///Assets/{iconName}.png"));
    }

    private bool _isFinishingDoubleTap = false;

    private void OnClick(object sender, RoutedEventArgs e)
    {
        if (_isFinishingDoubleTap)
        {
            _isFinishingDoubleTap = false;
            return;
        }

        OnToggleRepeat?.Invoke(this, EventArgs.Empty);
    }

    private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        _isFinishingDoubleTap = true;
        OnResetRepeat?.Invoke(this, EventArgs.Empty);
    }
    

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(SongRepeatButton),
            new PropertyMetadata(0.0)
        );

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
