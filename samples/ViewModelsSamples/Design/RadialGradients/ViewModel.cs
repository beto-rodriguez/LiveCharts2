﻿using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Design.RadialGradients;

[ObservableObject]
public partial class ViewModel
{
    // radial gradients are based on SkiaSharp circular gradients
    // for more info please see:
    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/effects/shaders/circular-gradients

    private static readonly SKColor[] s_colors =
    {
        new SKColor(179, 229, 252),
        new SKColor(1, 87, 155)
        // ...

        // you can add as many colors as you require to build the gradient
        // by default all the distance between each color is equal
        // use the colorPos parameter in the constructor of the RadialGradientPaint class
        // to specify the distance between each color
    };

    public ISeries[] Series { get; set; } =
    {
        new PieSeries<int>
        {
            Values = new []{ 7 },
            Stroke = null,
            Fill = new RadialGradientPaint(s_colors),
            Pushout = 10,
            MaxOuterRadius = 0.9
        },
        new PieSeries<int>
        {
            Values = new []{ 3 },
            Stroke = null,
            Fill = new RadialGradientPaint(new SKColor(255, 205, 210), new SKColor(183, 28, 28))
        }
    };
}
