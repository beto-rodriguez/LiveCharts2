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

using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using System;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView
{
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
        public static DefaultPaintTask<SkiaSharpDrawingContext> DefaultPaintTask { get; } = new DefaultPaintTask<SkiaSharpDrawingContext>();

        /// <summary>
        /// Gets the default platform builder.
        /// </summary>
        /// <value>
        /// The default platform builder.
        /// </value>
        public static Action<LiveChartsSettings> DefaultPlatformBuilder =>
            (LiveChartsSettings settings) => settings
                .AddDefaultMappers()
                .AddSkiaSharp();

        /// <summary>
        /// Adds SkiaSharp as the UI provider for LiveCharts.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static LiveChartsSettings AddSkiaSharp(
            this LiveChartsSettings settings, Action<Theme<SkiaSharpDrawingContext>> builder = null)
        {
            return settings
                .HasDataFactory(new DataFactory<SkiaSharpDrawingContext>())
                .HasTheme((Theme<SkiaSharpDrawingContext> theme) =>
                {
                    _ = theme
                        .WithColors(ColorPacks.MaterialDesign500)
                        .WithVisualsInitializer(initializer =>
                            initializer
                                .ForCharts(chart =>
                                {
                                    var defaultHoverColor = new SKColor(255, 255, 255, 180);

                                    // The point states dictionary defines the fill and stroke to use for a point marked with the
                                    // state key, LiveCharts uses this dictionary to highlight a chart point when the mouse is
                                    // over a point, for example, the first .WithState() defines that every time a point is marked
                                    // with the LiveCharts.BarSeriesHoverKey key, the library will draw a null stroke and
                                    // new SKColor(255, 255, 255, 180) as the fill (defaultHoverColor).

                                    chart.PointStates =
                                        new PointStatesDictionary<SkiaSharpDrawingContext>()
                                            .WithState(
                                                LiveCharts.BarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                            .WithState(
                                                LiveCharts.LineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                            .WithState(
                                                LiveCharts.PieSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                            .WithState(
                                                LiveCharts.ScatterSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                            .WithState(
                                                LiveCharts.StackedBarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor));
                                })
                                .ForAxes(axis =>
                                {
                                    axis.ShowSeparatorLines = true;
                                    axis.TextBrush = DefaultPaintTask;
                                    axis.SeparatorsBrush = DefaultPaintTask;
                                })
                                // ForAnySeries() will be called for all the series
                                .ForAnySeries(series =>
                                {
                                    series.Fill = DefaultPaintTask;
                                    series.Stroke = DefaultPaintTask;
                                })
                                .ForLineSeries(lineSeries =>
                                {
                                    // at this point ForAnySeries() was already called
                                    // we are configuring the missing properties
                                    lineSeries.GeometrySize = 18;
                                    lineSeries.GeometryFill = new SolidColorPaintTask(new SKColor(250, 250, 250));
                                    lineSeries.GeometryStroke = DefaultPaintTask;
                                })
                                .ForStackedLineSeries(stackedLine =>
                                {
                                    // at this point both ForAnySeries() and ForLineSeries() were already called
                                    // again we are correcting the previous settings
                                    stackedLine.GeometrySize = 0;
                                    stackedLine.GeometryFill = null;
                                    stackedLine.GeometryStroke = null;
                                    stackedLine.Stroke = null;
                                    stackedLine.Fill = DefaultPaintTask;
                                })
                                .ForPieSeries(pieSeries =>
                                {
                                    pieSeries.Fill = DefaultPaintTask;
                                    pieSeries.Stroke = null;
                                    pieSeries.Pushout = 0;
                                }))
                        // finally add a resolver for the DefaultPaintTask
                        // the library already provides the AddDefaultResolvers() method
                        .AddDefaultResolvers();

                    // user defined settings
                    builder?.Invoke(theme);
                });
        }

        /// <summary>
        /// Adds the default resolvers.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public static Theme<SkiaSharpDrawingContext> AddDefaultResolvers(
            this Theme<SkiaSharpDrawingContext> theme)
        {
            return theme
                .WithSeriesDefaultsResolver(
                    (Color[] colors, IDrawableSeries<SkiaSharpDrawingContext> series) =>
                    {
                        var color = colors[series.SeriesId % colors.Length];
                        if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

                        if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
                        {
                            if (series.Fill == DefaultPaintTask) series.Fill = new SolidColorPaintTask(ColorAsSKColor(color));
                            if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3);

                            return;
                        }

                        if (series.Fill == DefaultPaintTask)
                        {
                            var mask = SeriesProperties.Line | SeriesProperties.Stacked;
                            var opacity = (series.SeriesProperties & mask) == mask ? 1 : 0.5;

                            series.Fill = new SolidColorPaintTask(ColorAsSKColor(color, (byte)(opacity * 255)));
                        }
                        if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3.5f);

                        if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                        {
                            var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;
                            if (lineSeries.GeometryFill == DefaultPaintTask)
                                lineSeries.GeometryFill = new SolidColorPaintTask(ColorAsSKColor(color));
                            if (lineSeries.GeometryStroke == DefaultPaintTask)
                                lineSeries.GeometryStroke = new SolidColorPaintTask(ColorAsSKColor(color), lineSeries.Stroke?.StrokeThickness ?? 3.5f);
                        }
                    })
                .WithAxisDefaultsResolver(
                    (IAxis<SkiaSharpDrawingContext> axis) =>
                    {
                        if (axis.SeparatorsBrush == DefaultPaintTask)
                            axis.SeparatorsBrush = axis.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaintTask(new SKColor(225, 225, 225));

                        if (axis.TextBrush == DefaultPaintTask)
                            axis.TextBrush = new SolidColorPaintTask(new SKColor(180, 180, 180));
                    });
        }

        private static SKColor ColorAsSKColor(Color color, byte? alphaOverrides = null)
        {
            return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
        }
    }
}

