// The MIT License(MIT)
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
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the default LiveCharts-SkiaSharp settings
/// </summary>
public static class LiveChartsSkiaSharp
{
    /// <summary>
    /// Gets the default paint task.
    /// </summary>
    /// <value>
    /// The default paint.
    /// </value>
    public static DefaultPaint<SkiaSharpDrawingContext> DefaultPaint { get; } = new();

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
        // ToDo: default paint needs to be simplified???
        LiveCharts.DefaultPaint = DefaultPaint;

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
        if (buider is null) buider = (instance, series) => { };

        return new ObservableCollection<ISeries>(
            source.Select(instance =>
            {
                var series = new PieSeries<T> { Values = new ObservableCollection<T> { instance } };
                buider(instance, series);
                return series;
            })
            .ToArray());
    }
}
