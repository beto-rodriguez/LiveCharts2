﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Themes;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the default LiveCharts-SkiaSharp settings
/// </summary>
public static class LiveChartsSkiaSharp
{
    /// <summary>
    /// Gets the default platform builder.
    /// </summary>
    /// <value>
    /// The default platform builder.
    /// </value>
    public static Action<LiveChartsSettings> DefaultPlatformBuilder =>
        (LiveChartsSettings settings) => settings
            .AddDefaultMappers()
            .AddSkiaSharp()
            .AddLightTheme();

    /// <summary>
    /// Adds SkiaSharp as the backend provider for LiveCharts.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns></returns>
    public static LiveChartsSettings AddSkiaSharp(this LiveChartsSettings settings)
    {
        return settings.HasProvider(new SkiaSharpProvider());
    }

    /// <summary>
    /// Converts a <see cref="LvcColor"/> to a <see cref="SKColor"/> instance.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="alphaOverrides">The alpha overrides.</param>
    /// <returns></returns>
    public static SKColor AsSKColor(this LvcColor color, byte? alphaOverrides = null)
    {
        return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
    }

    /// <summary>
    /// Converts a <see cref="LvcColor"/> to a <see cref="SKColor"/> instance.
    /// </summary>
    /// <param name="colors">The color.</param>
    /// <returns></returns>
    public static SKColor[] AsSKColors(this LvcColor[] colors)
    {
        return colors.Select(x => x.AsSKColor()).ToArray();
    }

    /// <summary>
    /// Creates a new color based on the 
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="opacity">The opacity from 0 to 255.</param>
    /// <returns></returns>
    public static LvcColor WithOpacity(this LvcColor color, byte opacity)
    {
        return LvcColor.FromArgb(opacity, color);
    }

    /// <summary>
    /// Converts a <see cref="SKColor"/> to a <see cref="LvcColor"/> intance.
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns></returns>
    public static LvcColor AsLvcColor(this SKColor color)
    {
        return new LvcColor(color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Shades a color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="shade">The shade.</param>
    /// <returns></returns>
    public static SKColor Shade(this SKColor color, float shade)
    {
        return new SKColor(
            (byte)(color.Red * (1 - shade)),
            (byte)(color.Green * (1 - shade)),
            (byte)(color.Blue * (1 - shade)),
            color.Alpha);
    }

    /// <summary>
    /// Tints a color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="tint">The tint.</param>
    /// <returns></returns>
    public static SKColor Tint(this SKColor color, float tint)
    {
        return new SKColor(
            (byte)(color.Red + (255 - color.Red) * tint),
            (byte)(color.Green + (255 - color.Green) * tint),
            (byte)(color.Blue + (255 - color.Blue) * tint),
            color.Alpha);
    }

    /// <summary>
    /// Multiplies the current alpha for the given factor.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="factor">The factor.</param>
    /// <returns></returns>
    public static SKColor StackAlpha(this SKColor color, float factor)
    {
        return new SKColor(
            color.Red, color.Green, color.Blue, (byte)(color.Alpha * factor));
    }

    /// <summary>
    /// Gets the <see cref="SkiaFontMatchChar"/> key.
    /// </summary>
    public const string SkiaFontMatchChar = "matchChar";

    /// <summary>
    /// Matches
    /// </summary>
    /// <param name="char"></param>
    /// <returns></returns>
    public static string MatchChar(char @char)
    {
        return $"{SkiaFontMatchChar}|{@char}";
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="buider">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<ISeries> AsLiveChartsPieSeries<T>(
        this IEnumerable<T> source,
        Action<T, PieSeries<T>>? buider = null)
    {
        buider ??= (instance, series) => { };

        return new ObservableCollection<ISeries>(
            source.Select(instance =>
            {
                var series = new PieSeries<T> { Values = new ObservableCollection<T> { instance } };
                buider(instance, series);
                return series;
            })
            .ToArray());
    }

    /// <summary>
    /// Calculates the distance in pixels from the target <see cref="ChartPoint"/> to the given location in the UI.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="location">The location.</param>
    /// <returns>The distance in pixels.</returns>
    public static double GetDistanceTo(this ChartPoint target, LvcPoint location)
    {
        double[] dataCoordinates;
        double x, y;

        if (target.Context is ICartesianChartView<SkiaSharpDrawingContext> cartesianChart)
        {
            dataCoordinates = cartesianChart.ScaleUIPoint(location);

            var cartesianSeries = (ICartesianSeries<SkiaSharpDrawingContext>)target.Context.Series;

            if (target.Context.Series.SeriesProperties.HasFlag(SeriesProperties.PrimaryAxisHorizontalOrientation))
            {
                var primaryAxis = cartesianChart.Core.YAxes[cartesianSeries.ScalesYAt];
                var secondaryAxis = cartesianChart.Core.XAxes[cartesianSeries.ScalesXAt];

                var drawLocation = cartesianChart.Core.DrawMarginLocation;
                var drawMarginSize = cartesianChart.Core.DrawMarginSize;
                var secondaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
                var primaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);

                x = secondaryScale.ToPixels(target.SecondaryValue);
                y = primaryScale.ToPixels(target.PrimaryValue);
            }
            else
            {
                var primaryAxis = cartesianChart.Core.YAxes[cartesianSeries.ScalesXAt];
                var secondaryAxis = cartesianChart.Core.XAxes[cartesianSeries.ScalesYAt];

                var drawLocation = cartesianChart.Core.DrawMarginLocation;
                var drawMarginSize = cartesianChart.Core.DrawMarginSize;

                var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
                var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

                x = secondaryScale.ToPixels(target.SecondaryValue);
                y = primaryScale.ToPixels(target.PrimaryValue);
            }
        }
        else if (target.Context is IPolarChartView<SkiaSharpDrawingContext> polarChart)
        {
            dataCoordinates = polarChart.ScaleUIPoint(location);

            var polarSeries = (IPolarSeries<SkiaSharpDrawingContext>)target.Context.Series;

            var angleAxis = polarChart.Core.AngleAxes[polarSeries.ScalesAngleAt];
            var radiusAxis = polarChart.Core.RadiusAxes[polarSeries.ScalesRadiusAt];

            var drawLocation = polarChart.Core.DrawMarginLocation;
            var drawMarginSize = polarChart.Core.DrawMarginSize;

            var scaler = new PolarScaler(
                drawLocation, drawMarginSize, angleAxis, radiusAxis,
                polarChart.Core.InnerRadius, polarChart.Core.InitialRotation, polarChart.Core.TotalAnge);

            var scaled = scaler.ToPixels(target);
            x = scaled.X;
            y = scaled.Y;
        }
        else
        {
            throw new NotImplementedException();
        }

        // calculate the distance
        var dx = dataCoordinates[0] - x;
        var dy = dataCoordinates[1] - y;

        var distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

        return distance;
    }
}
