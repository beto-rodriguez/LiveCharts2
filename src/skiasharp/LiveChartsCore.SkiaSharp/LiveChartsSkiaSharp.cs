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

// Ignore Spelling: Skia Lvc

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the default LiveCharts-SkiaSharp settings
/// </summary>
public static class LiveChartsSkiaSharp
{
    /// <summary>
    /// Gets or sets an SKTypeface instance to use globally on any paint that does not specify any.
    /// </summary>
    public static SKTypeface? DefaultSKTypeface { get; set; }

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
    /// Configures LiveCharts using the default settings for SkiaSharp.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>The settings.</returns>
    public static LiveChartsSettings UseDefaults(this LiveChartsSettings settings)
    {
        return settings
            .AddDefaultMappers()
            .AddSkiaSharp()
            .AddLightTheme();
    }

    /// <summary>
    /// Adds SkiaSharp as the library backend.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns></returns>
    public static LiveChartsSettings AddSkiaSharp(this LiveChartsSettings settings)
    {
        return settings.HasProvider(new SkiaSharpProvider());
    }

    /// <summary>
    /// Registers a global SKTypeface instance to use on any <see cref="Paint"/> that does not specify a typeface.
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="typeface"></param>
    /// <returns></returns>
    public static LiveChartsSettings HasGlobalSKTypeface(this LiveChartsSettings settings, SKTypeface typeface)
    {
        DefaultSKTypeface = typeface;
        return settings;
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
    [Obsolete($"Use {nameof(Paint)}.{nameof(Paint.SKTypeface)} instead.")]
    public const string SkiaFontMatchChar = "matchChar";

    /// <summary>
    /// Matches
    /// </summary>
    /// <param name="char"></param>
    /// <returns></returns>
    [Obsolete($"Use {nameof(Paint)}.{nameof(Paint.SKTypeface)} instead.")]
    public static string MatchChar(char @char)
    {
        return $"{SkiaFontMatchChar}|{@char}";
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel>> AsPieSeries<TModel>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel>>? builder = null)
    {
        builder ??= (instance, series) => { };

        return new ObservableCollection<PieSeries<TModel>>(
            source.Select(instance =>
            {
                var series = new PieSeries<TModel> { Values = new ObservableCollection<TModel> { instance } };
                builder(instance, series);
                return series;
            })
            .ToArray());
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel, TVisual>> AsPieSeries<TModel, TVisual>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel, TVisual>>? builder = null)
            where TVisual : class, IDoughnutGeometry<SkiaSharpDrawingContext>, new()
    {
        builder ??= (instance, series) => { };

        return new ObservableCollection<PieSeries<TModel, TVisual>>(
            source.Select(instance =>
            {
                var series = new PieSeries<TModel, TVisual>
                {
                    Values = new ObservableCollection<TModel> { instance }
                };
                builder(instance, series);
                return series;
            })
            .ToArray());
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel, TVisual, TLabel>> AsPieSeries<TModel, TVisual, TLabel>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel, TVisual, TLabel>>? builder = null)
            where TVisual : class, IDoughnutGeometry<SkiaSharpDrawingContext>, new()
            where TLabel : class, ILabelGeometry<SkiaSharpDrawingContext>, new()
    {
        builder ??= (instance, series) => { };

        return new ObservableCollection<PieSeries<TModel, TVisual, TLabel>>(
            source.Select(instance =>
            {
                var series = new PieSeries<TModel, TVisual, TLabel>
                {
                    Values = new ObservableCollection<TModel> { instance }
                };
                builder(instance, series);
                return series;
            })
            .ToArray());
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    [Obsolete($"Renamed to {nameof(AsPieSeries)}.")]
    public static ObservableCollection<PieSeries<T>> AsLiveChartsPieSeries<T>(
        this IEnumerable<T> source,
        Action<T, PieSeries<T>>? builder = null)
    {
        return AsPieSeries(source, builder);
    }

    /// <summary>
    /// Calculates the distance in pixels from the target <see cref="ChartPoint"/> to the given location in the UI.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="location">The location.</param>
    /// <returns>The distance in pixels.</returns>
    public static double GetDistanceTo(this ChartPoint target, LvcPoint location)
    {
        LvcPointD dataCoordinates;
        double x, y;

        if (target.Context is ICartesianChartView<SkiaSharpDrawingContext> cartesianChart)
        {
            dataCoordinates = cartesianChart.ScalePixelsToData(new LvcPointD(location));

            var cartesianSeries = (ICartesianSeries<SkiaSharpDrawingContext>)target.Context.Series;

            if (target.Context.Series.SeriesProperties.HasFlag(SeriesProperties.PrimaryAxisHorizontalOrientation))
            {
                var primaryAxis = cartesianChart.Core.YAxes[cartesianSeries.ScalesYAt];
                var secondaryAxis = cartesianChart.Core.XAxes[cartesianSeries.ScalesXAt];

                var drawLocation = cartesianChart.Core.DrawMarginLocation;
                var drawMarginSize = cartesianChart.Core.DrawMarginSize;
                var secondaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
                var primaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);

                var coordinate = target.Coordinate;

                x = secondaryScale.ToPixels(coordinate.SecondaryValue);
                y = primaryScale.ToPixels(coordinate.PrimaryValue);
            }
            else
            {
                var primaryAxis = cartesianChart.Core.YAxes[cartesianSeries.ScalesXAt];
                var secondaryAxis = cartesianChart.Core.XAxes[cartesianSeries.ScalesYAt];

                var drawLocation = cartesianChart.Core.DrawMarginLocation;
                var drawMarginSize = cartesianChart.Core.DrawMarginSize;

                var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
                var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

                var coordinate = target.Coordinate;

                x = secondaryScale.ToPixels(coordinate.SecondaryValue);
                y = primaryScale.ToPixels(coordinate.PrimaryValue);
            }
        }
        else if (target.Context is IPolarChartView<SkiaSharpDrawingContext> polarChart)
        {
            dataCoordinates = polarChart.ScalePixelsToData(new LvcPointD(location));

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
        var dx = dataCoordinates.X - x;
        var dy = dataCoordinates.Y - y;

        var distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

        return distance;
    }
}
