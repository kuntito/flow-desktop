using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace flow_desktop.UI.Components;

/// <summary>
/// three animated bars that say something's playing.
///
/// each bar swings between 30% and 100% of its height.
/// they start 0, 100 and 200ms apart, so they look out of step.
/// </summary>
public sealed partial class AppSoundBars : UserControl
{
    public static readonly DependencyProperty BarBrushProperty =
        DependencyProperty.Register(
            nameof(BarBrush),
            typeof(Brush),
            typeof(AppSoundBars),
            new PropertyMetadata(null)
        );

    public Brush BarBrush
    {
        get => (Brush)GetValue(BarBrushProperty);
        set => SetValue(BarBrushProperty, value);
    }

    public AppSoundBars()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            BarsStoryboard.Begin();
        };

        Unloaded += (s, e) =>
        {
            BarsStoryboard.Stop();
        };
    }
}