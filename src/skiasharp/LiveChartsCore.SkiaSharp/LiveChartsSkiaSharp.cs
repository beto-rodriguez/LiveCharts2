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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.TypeConverters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the default LiveCharts-SkiaSharp settings
/// </summary>
public static class LiveChartsSkiaSharp
{
    internal static SKTypeface? DefaultSKTypeface { get; set; }

    internal static Action<SKPaint, bool>? DefaultSkiaSharpPaintBuilder { get; set; }

    internal static Func<SKTypeface, float, SKFont> DefaultSkiaSharpFontBuilder { get; set; }
        = (typeFace, size) =>
            // could this be improved?
            // like creating font settings based on the screen dpi, etc.
            new SKFont(typeFace, size)
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Hinting = SKFontHinting.Normal
            };

    /// <summary>
    /// Configures LiveCharts using the default settings for SkiaSharp.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="renderingSettings">The optional rendering settings.</param>
    /// <returns>The settings.</returns>
    public static LiveChartsSettings UseDefaults(
        this LiveChartsSettings settings, RenderingSettings? renderingSettings = null)
    {
        if (!LiveCharts.DefaultSettings.HasBackedDefined)
            _ = settings.AddSkiaSharp();

        if (!LiveCharts.DefaultSettings.HasThemeDefined)
            _ = settings.AddDefaultTheme();

        if (!LiveCharts.DefaultSettings.HasMappersDefined)
            _ = settings.AddDefaultMappers();

        if (LiveCharts.RenderingSettings is null)
        {
            var targetRenderSettings = renderingSettings ?? RenderingSettings.Default;

            // the next conditions are used to test the rendering settings across
            // multiple os/frameworks via cli flags.

#if __GPU_TRUE__
            targetRenderSettings.UseGPU = true;
#endif
#if __GPU_FALSE__
            targetRenderSettings.UseGPU = false;
#endif
#if __VSYNC_TRUE__
            targetRenderSettings.TryUseVSync = true;
#endif
#if __VSYNC_FALSE__
            targetRenderSettings.TryUseVSync = false;
#endif
#if __FPS_10__
            targetRenderSettings.LiveChartsRenderLoopFPS = 10;
#endif
#if __FPS_20__
            targetRenderSettings.LiveChartsRenderLoopFPS = 20;
#endif
#if __FPS_30__
            targetRenderSettings.LiveChartsRenderLoopFPS = 30;
#endif
#if __FPS_45__
            targetRenderSettings.LiveChartsRenderLoopFPS = 45;
#endif
#if __FPS_60__
            targetRenderSettings.LiveChartsRenderLoopFPS = 60;
#endif
#if __FPS_75__
            targetRenderSettings.LiveChartsRenderLoopFPS = 75;
#endif
#if __FPS_90__
            targetRenderSettings.LiveChartsRenderLoopFPS = 90;
#endif
#if __FPS_120__
            targetRenderSettings.LiveChartsRenderLoopFPS = 120;
#endif
#if __DIAGNOSE__
            targetRenderSettings.ShowFPS = true;
#endif

            _ = settings.RenderingSettings(targetRenderSettings);
        }

        return settings;
    }

    /// <summary>
    /// Adds SkiaSharp as the library backend.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>The current settings.</returns>
    public static LiveChartsSettings AddSkiaSharp(this LiveChartsSettings settings)
    {
        PropertyDefinition.Parsers[typeof(Paint)] = HexToPaintTypeConverter.Parse;
        PropertyDefinition.Parsers[typeof(LvcColor)] = HexToLvcColorTypeConverter.Parse;
        PropertyDefinition.Parsers[typeof(Margin)] = MarginTypeConverter.ParseMargin;
        PropertyDefinition.Parsers[typeof(Padding)] = PaddingTypeConverter.ParsePadding;
        PropertyDefinition.Parsers[typeof(LvcPointD)] = PointDTypeConverter.ParsePoint;
        PropertyDefinition.Parsers[typeof(LvcPoint)] = PointTypeConverter.ParsePoint;

        return settings.HasProvider(new SkiaSharpProvider());
    }

    /// <summary>
    /// Registers a global SKTypeface instance to use on any <see cref="SkiaPaint"/> that does not specify a typeface.
    /// </summary>
    /// <param name="settings">The current settings.</param>
    /// <param name="typeface">The typeface to load for text paints.</param>
    /// <returns>The current settings.</returns>
    public static LiveChartsSettings HasGlobalSKTypeface(this LiveChartsSettings settings, SKTypeface typeface)
    {
        DefaultSKTypeface = typeface;
        return settings;
    }

    /// <summary>
    /// Registers a global action that configures the SkiaSharp paint before drawing it, if you need to
    /// override the builder for a specific <see cref="SkiaPaint"/> instance, use the
    /// <see cref="SkiaPaint.ConfigureSkiaSharpPaint(Action{SKPaint, bool})"/> method.
    /// </summary>
    /// <param name="settings">The current settings.</param>
    /// <param name="paintBuilder">
    /// An action that configures the SkiaSharp paint, when LiveCharts is drawing it will call this action
    /// to configure the paint before drawing it, the boolean parameter indicates whether the paint is being used for
    /// a text element or not.
    /// </param>
    /// <returns>The current settings.</returns>
    public static LiveChartsSettings HasPaintSettings(this LiveChartsSettings settings, Action<SKPaint, bool> paintBuilder)
    {
        DefaultSkiaSharpPaintBuilder = paintBuilder;
        return settings;
    }

    /// <summary>
    /// Registers a global function that builds the SkiaSharp SKFont to draw text with it, if you need to
    /// override the builder for a specific <see cref="SkiaPaint"/> instance, use the
    /// <see cref="SkiaPaint.ConfigureSkiaSharpFont(Func{SKTypeface, float, SKFont})"/> method.
    /// </summary>
    /// <param name="settings">The current settings.</param>
    /// <param name="fontBuilder">
    /// The font builder, when LiveCharts is drawing text it will call this action.
    /// </param>
    /// <returns>The current settings.</returns>
    public static LiveChartsSettings HasFontSettings(this LiveChartsSettings settings, Func<SKTypeface, float, SKFont> fontBuilder)
    {
        DefaultSkiaSharpFontBuilder = fontBuilder;
        return settings;
    }

    /// <summary>
    /// Converts a <see cref="LvcColor"/> to a <see cref="SKColor"/> instance.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="alphaOverrides">The alpha overrides.</param>
    /// <returns></returns>
    public static SKColor AsSKColor(this LvcColor color, byte? alphaOverrides = null) =>
        color == LvcColor.Empty
            ? SKColor.Empty
            : new(color.R, color.G, color.B, alphaOverrides ?? color.A);

    /// <summary>
    /// Creates a new color based on the 
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="opacity">The opacity from 0 to 255.</param>
    /// <returns></returns>
    public static LvcColor WithOpacity(this LvcColor color, byte opacity) =>
        LvcColor.FromArgb(opacity, color);

    /// <summary>
    /// Converts a <see cref="SKColor"/> to a <see cref="LvcColor"/> intance.
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns></returns>
    public static LvcColor AsLvcColor(this SKColor color) =>
        new(color.Red, color.Green, color.Blue, color.Alpha);

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

        if (target.Context is ICartesianChartView cartesianChart)
        {
            dataCoordinates = cartesianChart.ScalePixelsToData(new LvcPointD(location));

            var cartesianSeries = (ICartesianSeries)target.Context.Series;

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
        else if (target.Context is IPolarChartView polarChart)
        {
            dataCoordinates = polarChart.ScalePixelsToData(new LvcPointD(location));

            var polarSeries = (IPolarSeries)target.Context.Series;

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
