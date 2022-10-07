﻿using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace ViewModelsSamples.Axes.Style;

[ObservableObject]
public partial class ViewModel
{
    private static readonly SKColor s_gray = new(195, 195, 195);
    private static readonly SKColor s_gray1 = new(160, 160, 160);
    private static readonly SKColor s_gray2 = new(90, 90, 90);
    private static readonly SKColor s_dark3 = new(60, 60, 60);
    private static readonly SKColor s_crosshair = new(255, 171, 145);

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<ObservablePoint>
        {
            Values = Fetch(),
            Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
            Fill = null,
            GeometrySize = 0
        }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "X axis",
            NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 18,
            Padding = new Padding(5, 15, 5, 5),
            LabelsPaint = new SolidColorPaint(s_gray),
            SeparatorsPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1,
                PathEffect = new DashEffect(new float[] { 3, 3 })
            },
            SubseparatorsPaint = new SolidColorPaint
            {
                Color = s_gray2,
                StrokeThickness = 0.5f
            },
            ZeroPaint = new SolidColorPaint
            {
                Color = s_gray1,
                StrokeThickness = 2
            },
            TicksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1.5f
            },
            SubticksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            },
            CrosshairPaint = new SolidColorPaint
            {
                Color = s_crosshair,
                StrokeThickness = 3
            },
            CrosshairLabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Black,
                SKFontStyle =  new SKFontStyle(SKFontStyleWeight.SemiBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            },
            CrosshairLabelsBackground = s_crosshair.AsLvcColor(),
            CrosshairPadding = new Padding(10, 20, 10, 10)
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Y axis",
            NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 18,
            Padding = new Padding(5, 0, 15, 0),
            LabelsPaint = new SolidColorPaint(s_gray),
            SeparatorsPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1,
                PathEffect = new DashEffect(new float[] { 3, 3 })
            },
            SubseparatorsPaint = new SolidColorPaint
            {
                Color = s_gray2,
                StrokeThickness = 0.5f
            },
            ZeroPaint = new SolidColorPaint
            {
                Color = s_gray1,
                StrokeThickness = 2
            },
            TicksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1.5f
            },
            SubticksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            },
            CrosshairPaint = new SolidColorPaint
            {
                Color = s_crosshair,
                StrokeThickness = 3
            },
            CrosshairLabelsPaint = new SolidColorPaint
            {
                Color = SKColors.Black,
                SKFontStyle =  new SKFontStyle(SKFontStyleWeight.SemiBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            },
            CrosshairLabelsBackground = s_crosshair.AsLvcColor(),
            CrosshairPadding = new Padding(10, 10, 30, 10)
        }
    };

    public DrawMarginFrame Frame { get; set; } =
    new()
    {
        Fill = new SolidColorPaint(s_dark3),
        Stroke = new SolidColorPaint
        {
            Color = s_gray,
            StrokeThickness = 1
        }
    };

    private static List<ObservablePoint> Fetch()
    {
        var list = new List<ObservablePoint>();
        var fx = EasingFunctions.BounceInOut;

        for (var x = 0f; x < 1f; x += 0.001f)
        {
            var y = fx(x);
            list.Add(new ObservablePoint(x - 0.5, y - 0.5));
        }

        return list;
    }
}
