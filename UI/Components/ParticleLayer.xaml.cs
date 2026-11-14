using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace flow_desktop.UI.Components;

/// <summary>
/// a layer of slow, rising, softly pulsing dots.
///
/// same as mobile's ParticleLayer.
/// each frame, every particle drifts up, wraps to the bottom
/// when it passes the top, and fades between two opacities.
///
/// it ignores input, so clicks pass through to whatever is underneath.
/// </summary>
public sealed partial class ParticleLayer : UserControl
{
    private const int ParticleCount = 30;

    private readonly Random _random = new();
    private readonly List<Particle> _particles = new();

    public ParticleLayer()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_particles.Count == 0)
        {
            CreateParticles();
        }

        CompositionTarget.Rendering += OnFrame;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnFrame;
    }

    private void CreateParticles()
    {
        var telli = (Color)Application.Current.Resources["ColorTelli"];

        var particleColors = new List<Color>
        {
            (Color)Application.Current.Resources["ColorAguero"],
            (Color)Application.Current.Resources["ColorMarcelo"],
            Color.FromArgb(
                (byte)(255 * 0.8),
                telli.R,
                telli.G,
                telli.B
            ),
        };

        for (var i = 0; i < ParticleCount; i++)
        {
            var radius = _random.NextSingle() * 1.5f + 0.75f;
            var color = particleColors[_random.Next(particleColors.Count)];

            var dot = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = new SolidColorBrush(color),
            };

            var particle = new Particle
            {
                X = _random.NextSingle(),
                Y = _random.NextSingle(),
                Speed = _random.NextSingle() * 0.8f + 0.2f,
                Radius = radius,
                Opacity = _random.NextSingle(),
                OpacitySpeed = _random.NextSingle() * 0.005f + 0.001f,
                OpacityDirection = _random.Next(2) == 0 ? 1f : -1f,
                Dot = dot,
            };

            _particles.Add(particle);
            ParticleCanvas.Children.Add(dot);
        }
    }

    private void OnFrame(object? sender, object e)
    {
        var width = ActualWidth;
        var height = ActualHeight;

        foreach (var p in _particles)
        {
            p.Y -= p.Speed * 0.001f;
            if (p.Y < 0f)
            {
                p.Y = 1f;
                p.X = _random.NextSingle();
            }

            p.Opacity += p.OpacitySpeed * p.OpacityDirection;
            if (p.Opacity >= 0.6f || p.Opacity <= 0.1f)
            {
                p.OpacityDirection *= -1f;
            }

            // Canvas positions by the top-left corner,
            // so shift by the radius to place the dot's center.
            Canvas.SetLeft(p.Dot, p.X * width - p.Radius);
            Canvas.SetTop(p.Dot, p.Y * height - p.Radius);
            p.Dot.Opacity = p.Opacity;
        }
    }

    private class Particle
    {
        public float X;
        public float Y;
        public float Speed;
        public float Radius;
        public float Opacity;
        public float OpacitySpeed;
        public float OpacityDirection;
        public required Ellipse Dot;
    }
}