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

using LiveChartsCore.Drawing.Common;
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
                .AddSkiaSharp()
                .AddLightTheme();

        /// <summary>
        /// Adds SkiaSharp as the UI provider for LiveCharts.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static LiveChartsSettings AddSkiaSharp(this LiveChartsSettings settings)
        {
            return settings
                .HasDataFactory(new DataFactory<SkiaSharpDrawingContext>())
                .HasAxisProvider(() => new Axis());
        }

        /// <summary>
        /// Adds the light theme.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="additionalStyles">the additional styles.</param>
        /// <returns></returns>
        public static LiveChartsSettings AddLightTheme(
            this LiveChartsSettings settings, Action<Theme<SkiaSharpDrawingContext>>? additionalStyles = null)
        {
            return settings
                .HasTheme((Theme<SkiaSharpDrawingContext> theme) =>
                {
                    _ = theme
                       .WithColors(ColorPalletes.MaterialDesign500)
                       .WithStyle(style =>
                           style
                               .HasRuleForCharts(chart =>
                               {
                                   //chart.BackColor = Color.FromArgb(255, 255, 255, 255);
                                   chart.AnimationsSpeed = TimeSpan.FromMilliseconds(700);
                                   chart.EasingFunction = EasingFunctions.ExponentialOut;

                                   // The point states dictionary defines the fill and stroke to use for a point marked with the
                                   // state key, LiveCharts uses this dictionary to highlight a chart point when the mouse is
                                   // over a point, for example, the first .WithState() defines that every time a point is marked
                                   // with the LiveCharts.BarSeriesHoverKey key, the library will draw a null stroke and
                                   // new SKColor(255, 255, 255, 180) as the fill (defaultHoverColor).
                                   var defaultHoverColor = Color.FromArgb(180, 255, 255, 255).AsSKColor();
                                   chart.PointStates = new PointStatesDictionary<SkiaSharpDrawingContext>()
                                        .WithState(LiveCharts.BarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                        .WithState(LiveCharts.LineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                        .WithState(LiveCharts.PieSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                        .WithState(LiveCharts.ScatterSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor))
                                        .WithState(LiveCharts.StackedBarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor));
                               })
                               .HasRuleForAxes(axis =>
                               {
                                   axis.TextSize = 16;
                                   axis.ShowSeparatorLines = true;
                                   axis.LabelsPaint = DefaultPaintTask;
                                   axis.SeparatorsPaint = DefaultPaintTask;
                               })
                               // ForAnySeries() will be called for all the series
                               .HasRuleForAnySeries(series =>
                               {
                                   series.Fill = DefaultPaintTask;
                                   series.Stroke = DefaultPaintTask;
                               })
                               .HasRuleForLineSeries(lineSeries =>
                               {
                                   // at this point ForAnySeries() was already called
                                   // we are configuring the missing properties
                                   lineSeries.GeometrySize = 18;
                                   lineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 250, 250, 250).AsSKColor());
                                   lineSeries.GeometryStroke = DefaultPaintTask;
                               })
                               .HasRuleForStackedLineSeries(stackedLine =>
                               {
                                   // at this point both ForAnySeries() and ForLineSeries() were already called
                                   // again we are correcting the previous settings
                                   stackedLine.GeometrySize = 0;
                                   stackedLine.GeometryFill = null;
                                   stackedLine.GeometryStroke = null;
                                   stackedLine.Stroke = null;
                                   stackedLine.Fill = DefaultPaintTask;
                               })
                               .HasRuleForBarSeries(barSeries =>
                               {
                                   // only ForAnySeries() has run, a bar series is not
                                   // any of the previous types.
                                   barSeries.Stroke = null;
                                   barSeries.Rx = 4;
                                   barSeries.Ry = 4;
                               })
                               .HasRuleForStackedBarSeries(stackedBarSeries =>
                               {
                                   stackedBarSeries.Rx = 0;
                                   stackedBarSeries.Ry = 0;
                               })
                               .HasRuleForPieSeries(pieSeries =>
                               {
                                   pieSeries.Fill = DefaultPaintTask;
                                   pieSeries.Stroke = null;
                                   pieSeries.Pushout = 0;
                               }))
                       // finally add a resolver for the DefaultPaintTask
                       // the library already provides the AddDefaultLightResolvers() and AddDefaultDarkResolvers methods
                       // these method only translates 'DefaultPaintTask' to a valid stroke/fill based on
                       // the series context
                       .AddDefaultLightResolvers();

                    additionalStyles?.Invoke(theme);
                });
        }

        /// <summary>
        /// Adds the light theme.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="additionalStyles">The additional styles.</param>
        /// <returns></returns>
        public static LiveChartsSettings AddDarkTheme(
            this LiveChartsSettings settings, Action<Theme<SkiaSharpDrawingContext>>? additionalStyles = null)
        {
            return settings
                .HasTheme((Theme<SkiaSharpDrawingContext> theme) =>
                {
                    _ = theme
                       .WithColors(ColorPalletes.MaterialDesign200)
                       .WithStyle(style =>
                           style
                               .HasRuleForCharts(chart =>
                               {
                                   //chart.BackColor = Color.FromArgb(255, 40, 40, 40);
                                   chart.AnimationsSpeed = TimeSpan.FromMilliseconds(700);
                                   chart.EasingFunction = EasingFunctions.ExponentialOut;

                                   // The point states dictionary defines the fill and stroke to use for a point marked with the
                                   // state key, LiveCharts uses this dictionary to highlight a chart point when the mouse is
                                   // over a point, for example, the first .WithState() defines that every time a point is marked
                                   // with the LiveCharts.BarSeriesHoverKey key, the library will draw a null stroke and
                                   // new SKColor(255, 255, 255, 180) as the fill (defaultHoverColor).
                                   var defaultHoverColor = Color.FromArgb(40, 255, 255, 255).AsSKColor();
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
                               .HasRuleForAxes(axis =>
                               {
                                   axis.TextSize = 18;
                                   axis.ShowSeparatorLines = true;
                                   axis.LabelsPaint = DefaultPaintTask;
                                   axis.SeparatorsPaint = DefaultPaintTask;
                               })
                               // ForAnySeries() will be called for all the series
                               .HasRuleForAnySeries(series =>
                               {
                                   series.Fill = DefaultPaintTask;
                                   series.Stroke = DefaultPaintTask;
                               })
                               .HasRuleForLineSeries(lineSeries =>
                               {
                                   // at this point ForAnySeries() was already called
                                   // we are configuring the missing properties
                                   lineSeries.GeometrySize = 18;
                                   lineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 40, 40, 40).AsSKColor());
                                   lineSeries.GeometryStroke = DefaultPaintTask;
                               })
                               .HasRuleForStackedLineSeries(stackedLine =>
                               {
                                   // at this point both ForAnySeries() and ForLineSeries() were already called
                                   // again we are correcting the previous settings
                                   stackedLine.GeometrySize = 0;
                                   stackedLine.GeometryFill = null;
                                   stackedLine.GeometryStroke = null;
                                   stackedLine.Stroke = null;
                                   stackedLine.Fill = DefaultPaintTask;
                               })
                               .HasRuleForBarSeries(barSeries =>
                               {
                                   // only ForAnySeries() has run, a bar series is not
                                   // any of the previous types.
                                   barSeries.Stroke = null;
                                   barSeries.Rx = 4;
                                   barSeries.Ry = 4;
                               })
                               .HasRuleForStackedBarSeries(stackedBarSeries =>
                               {
                                   stackedBarSeries.Rx = 0;
                                   stackedBarSeries.Ry = 0;
                               })
                               .HasRuleForPieSeries(pieSeries =>
                               {
                                   pieSeries.Fill = DefaultPaintTask;
                                   pieSeries.Stroke = null;
                                   pieSeries.Pushout = 0;
                               }))
                       // finally add a resolver for the DefaultPaintTask
                       // the library already provides the AddDefaultResolvers() method
                       // this method only translates 'DefaultPaintTask' to a valid stroke/fill based on
                       // the series context
                       .AddDefaultDarkResolvers();

                    additionalStyles?.Invoke(theme);
                });
        }

        /// <summary>
        /// Adds the default resolvers.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public static Theme<SkiaSharpDrawingContext> AddDefaultLightResolvers(
            this Theme<SkiaSharpDrawingContext> theme)
        {
            return theme
                .WithSeriesDefaultsResolver(
                    (Color[] colors, IDrawableSeries<SkiaSharpDrawingContext> series, bool forceApply) =>
                    {
                        if (forceApply)
                        {
                            if (!LiveCharts.IsConfigured) LiveCharts.Configure(DefaultPlatformBuilder);
                            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                            var initializer = stylesBuilder.GetVisualsInitializer();

                            initializer.ApplyStyleToSeries(series);
                        }

                        if (series.DataLabelsPaint == DefaultPaintTask)
                        {
                            series.DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40));
                        }

                        if ((series.SeriesProperties & SeriesProperties.GaugeFill) != 0)
                        {
                            if (series.Stroke == DefaultPaintTask) series.Stroke = null;
                            if (series.Fill == DefaultPaintTask) series.Fill = new SolidColorPaintTask(new SKColor(0, 0, 0, 15));
                            return;
                        }

                        var color = colors[series.SeriesId % colors.Length];
                        if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

                        if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries ||
                            (series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
                        {
                            if (series.Fill == DefaultPaintTask) series.Fill = new SolidColorPaintTask(color.AsSKColor());
                            if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(color.AsSKColor(), 3);

                            return;
                        }

                        if (series.Fill == DefaultPaintTask)
                        {
                            var mask = SeriesProperties.Line | SeriesProperties.Stacked;
                            var opacity = (series.SeriesProperties & mask) == mask ? 1 : 0.2;

                            series.Fill = new SolidColorPaintTask(color.AsSKColor((byte)(opacity * 255)));
                        }
                        if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(color.AsSKColor(), 5);

                        if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                        {
                            var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                            if (lineSeries.GeometryFill == DefaultPaintTask)
                                lineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (lineSeries.GeometryStroke == DefaultPaintTask)
                                lineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), lineSeries.Stroke?.StrokeThickness ?? 5);
                        }
                    })
                .WithAxisDefaultsResolver(
                    (IAxis<SkiaSharpDrawingContext> axis, bool forceApply) =>
                    {
                        if (forceApply)
                        {
                            if (!LiveCharts.IsConfigured) LiveCharts.Configure(DefaultPlatformBuilder);
                            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                            var initializer = stylesBuilder.GetVisualsInitializer();

                            initializer.ApplyStyleToAxis(axis);
                        }

                        if (axis.SeparatorsPaint == DefaultPaintTask)
                            axis.SeparatorsPaint = axis.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaintTask(new SKColor(235, 235, 235));

                        if (axis.LabelsPaint == DefaultPaintTask)
                            axis.LabelsPaint = new SolidColorPaintTask(new SKColor(90, 90, 90));

                        if (axis.Padding == Padding.Default)
                            axis.Padding = new Padding { Bottom = 8, Left = 8, Right = 8, Top = 8 };
                    });
        }

        /// <summary>
        /// Adds the default resolvers.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public static Theme<SkiaSharpDrawingContext> AddDefaultDarkResolvers(
            this Theme<SkiaSharpDrawingContext> theme)
        {
            return theme
                .WithSeriesDefaultsResolver(
                    (Color[] colors, IDrawableSeries<SkiaSharpDrawingContext> series, bool forceApply) =>
                    {
                        if (forceApply)
                        {
                            if (!LiveCharts.IsConfigured) LiveCharts.Configure(DefaultPlatformBuilder);
                            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                            var initializer = stylesBuilder.GetVisualsInitializer();

                            initializer.ApplyStyleToSeries(series);
                        }

                        if (series.DataLabelsPaint == DefaultPaintTask)
                        {
                            series.DataLabelsPaint = new SolidColorPaintTask(new SKColor(230, 230, 230));
                        }

                        if ((series.SeriesProperties & SeriesProperties.GaugeFill) != 0)
                        {
                            if (series.Stroke == DefaultPaintTask) series.Stroke = null;
                            if (series.Fill == DefaultPaintTask) series.Fill = new SolidColorPaintTask(new SKColor(255, 255, 255, 15));
                            return;
                        }

                        var color = colors[series.SeriesId % colors.Length];
                        if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

                        if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries ||
                            (series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
                        {
                            if (series.Fill == DefaultPaintTask) series.Fill = new SolidColorPaintTask(color.AsSKColor());
                            if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(color.AsSKColor(), 3);

                            return;
                        }

                        if (series.Fill == DefaultPaintTask)
                        {
                            var mask = SeriesProperties.Line | SeriesProperties.Stacked;
                            var opacity = (series.SeriesProperties & mask) == mask ? 1 : 0.2;

                            series.Fill = new SolidColorPaintTask(color.AsSKColor((byte)(opacity * 255)));
                        }
                        if (series.Stroke == DefaultPaintTask) series.Stroke = new SolidColorPaintTask(color.AsSKColor(), 5);

                        if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                        {
                            var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                            if (lineSeries.GeometryFill == DefaultPaintTask)
                                lineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (lineSeries.GeometryStroke == DefaultPaintTask)
                                lineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), lineSeries.Stroke?.StrokeThickness ?? 5);
                        }
                    })
                .WithAxisDefaultsResolver(
                    (IAxis<SkiaSharpDrawingContext> axis, bool forceApply) =>
                    {
                        if (forceApply)
                        {
                            if (!LiveCharts.IsConfigured) LiveCharts.Configure(DefaultPlatformBuilder);
                            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                            var initializer = stylesBuilder.GetVisualsInitializer();

                            initializer.ApplyStyleToAxis(axis);
                        }

                        if (axis.SeparatorsPaint == DefaultPaintTask)
                            axis.SeparatorsPaint = axis.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaintTask(new SKColor(90, 90, 90));

                        if (axis.LabelsPaint == DefaultPaintTask)
                            axis.LabelsPaint = new SolidColorPaintTask(new SKColor(200, 200, 200));

                        if (axis.Padding == Padding.Default)
                            axis.Padding = new Padding { Bottom = 8, Left = 8, Right = 8, Top = 8 };
                    });
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="SKColor"/> instance.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="alphaOverrides">The alpha overrides.</param>
        /// <returns></returns>
        public static SKColor AsSKColor(this Color color, byte? alphaOverrides = null)
        {
            return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
        }

        /// <summary>
        /// Creates a new color based on the 
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity from 0 to 255.</param>
        /// <returns></returns>
        public static Color WithOpacity(this Color color, byte opacity)
        {
            return Color.FromArgb(opacity, color.R, color.G, color.B);
        }
    }
}
