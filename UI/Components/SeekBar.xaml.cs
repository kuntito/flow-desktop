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
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace flow_desktop.UI.Components;

public sealed partial class SeekBar : UserControl
{
    private FrameworkElement? _sliderThumb;
    public SeekBar()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            _sliderThumb = FindThumb(SeekBarSlider);
            if (_sliderThumb != null){
                _sliderThumb.Opacity = 0;
            }
        };

        PointerEntered += (s, e) => AnimateThumbOpacity(1);

        PointerExited += (s, e) => AnimateThumbOpacity(0);
    }

    /// <summary>
    /// <para>i want the thumb to only be visible when interacted with.
    /// for that, i need a reference to it.</para>
    /// <para>i'm using Microsoft's default Slider component and its thumb
    /// is buried inside its control template and can't be referenced
    /// directly in XAML.</para>
    /// <para>this fn walks the visual tree depth-first and returns the
    /// thumb when it finds it.</para>
    /// </summary>
    private FrameworkElement? FindThumb(DependencyObject parent)
    {
        for (
            int i = 0;
            i < VisualTreeHelper.GetChildrenCount(parent);
            i++
        )
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is Thumb) return child as FrameworkElement;

            var maybeThumb = FindThumb(child);
            if (maybeThumb != null) return maybeThumb;
        }

        return null;
    }

    /// <summary>
    /// <para>rather than sharply showing/hiding the thumb, i want it
    /// to fade in and out smoothly.</para>
    /// <para>this creates a short opacity animation on the thumb
    /// element and plays it immediately.</para>
    /// </summary>
    private void AnimateThumbOpacity(double targetOpacity)
    {
        if (_sliderThumb == null)
        {
            return;
        }

        var animationValue = new DoubleAnimation
        {
            To = targetOpacity,
            Duration = new Duration(TimeSpan.FromMilliseconds(150))
        };

        var storyboard = new Storyboard();

        Storyboard.SetTarget(animationValue, _sliderThumb);
        Storyboard.SetTargetProperty(animationValue, "Opacity");

        storyboard.Children.Add(animationValue);
        storyboard.Begin();
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(SeekBarWidth),
            typeof(double),
            typeof(SeekBar),
            new PropertyMetadata(0.0)
        );

    public double SeekBarWidth
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
