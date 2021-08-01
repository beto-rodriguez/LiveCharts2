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
using LiveChartsCore.Kernel.Data;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
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
                                        .WithState(LiveCharts.BarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.LineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.StepLineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.PieSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.ScatterSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.StackedBarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                        .WithState(LiveCharts.HeatSeriesHoverState, null, new SolidColorPaintTask(defaultHoverColor), true);
                               })
                               .HasRuleForAxes(axis =>
                               {
                                   axis.TextSize = 16;
                                   axis.ShowSeparatorLines = true;
                                   axis.NamePaint = DefaultPaintTask;
                                   axis.LabelsPaint = DefaultPaintTask;
                                   axis.SeparatorsPaint = DefaultPaintTask;
                               })
                               // ForAnySeries() will be called for all the series
                               .HasRuleForAnySeries(series =>
                               {
                                   if (series is not IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled) return;
                                   strokedAndFilled.Fill = DefaultPaintTask;
                                   strokedAndFilled.Stroke = DefaultPaintTask;
                               })
                               .HasRuleForLineSeries(lineSeries =>
                               {
                                   // at this point ForAnySeries() was already called
                                   // we are configuring the missing properties
                                   lineSeries.GeometrySize = 18;
                                   lineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 250, 250, 250).AsSKColor());
                                   lineSeries.GeometryStroke = DefaultPaintTask;
                               })
                               .HasRuleForStepLineSeries(steplineSeries =>
                               {
                                   // at this point ForAnySeries() was already called
                                   // we are configuring the missing properties
                                   steplineSeries.GeometrySize = 18;
                                   steplineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 250, 250, 250).AsSKColor());
                                   steplineSeries.GeometryStroke = DefaultPaintTask;
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
                               })
                               .HasRuleForStackedStepLineSeries(stackedStep =>
                               {
                                   stackedStep.GeometrySize = 0;
                                   stackedStep.GeometryFill = null;
                                   stackedStep.GeometryStroke = null;
                                   stackedStep.Stroke = null;
                                   stackedStep.Fill = DefaultPaintTask;
                               })
                               .HasRuleForHeatSeries(heatSeries =>
                               {
                                   // ... rules here
                               })
                               .HasRuleForFinancialSeries(financialSeries =>
                               {
                                   financialSeries.UpFill = DefaultPaintTask;
                                   financialSeries.DownFill = DefaultPaintTask;
                                   financialSeries.UpStroke = DefaultPaintTask;
                                   financialSeries.DownStroke = DefaultPaintTask;
                               }))
                       // finally add a resolver for the DefaultPaintTask
                       // the library already provides the AddDefaultLightResolvers() and AddDefaultDarkResolvers methods
                       // this method only translates 'DefaultPaintTask' to a valid stroke/fill based on
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
                                               LiveCharts.BarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                               LiveCharts.StepLineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                               LiveCharts.LineSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                               LiveCharts.PieSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                               LiveCharts.ScatterSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                               LiveCharts.StackedBarSeriesHoverKey, null, new SolidColorPaintTask(defaultHoverColor), true)
                                           .WithState(
                                                LiveCharts.HeatSeriesHoverState, null, new SolidColorPaintTask(defaultHoverColor), true);
                               })
                               .HasRuleForAxes(axis =>
                               {
                                   axis.TextSize = 18;
                                   axis.ShowSeparatorLines = true;
                                   axis.NamePaint = DefaultPaintTask;
                                   axis.LabelsPaint = DefaultPaintTask;
                                   axis.SeparatorsPaint = DefaultPaintTask;
                               })
                               // ForAnySeries() will be called for all the series
                               .HasRuleForAnySeries(series =>
                               {
                                   if (series is not IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled) return;
                                   strokedAndFilled.Fill = DefaultPaintTask;
                                   strokedAndFilled.Stroke = DefaultPaintTask;
                               })
                               .HasRuleForLineSeries(lineSeries =>
                               {
                                   lineSeries.GeometrySize = 18;
                                   lineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 40, 40, 40).AsSKColor());
                                   lineSeries.GeometryStroke = DefaultPaintTask;
                               })
                               .HasRuleForStepLineSeries(steplineSeries =>
                               {
                                   // at this point ForAnySeries() was already called
                                   // we are configuring the missing properties
                                   steplineSeries.GeometrySize = 18;
                                   steplineSeries.GeometryFill = new SolidColorPaintTask(Color.FromArgb(255, 40, 40, 40).AsSKColor());
                                   steplineSeries.GeometryStroke = DefaultPaintTask;
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
                               })
                               .HasRuleForStackedStepLineSeries(stackedStep =>
                               {
                                   stackedStep.GeometrySize = 0;
                                   stackedStep.GeometryFill = null;
                                   stackedStep.GeometryStroke = null;
                                   stackedStep.Stroke = null;
                                   stackedStep.Fill = DefaultPaintTask;
                               })
                               .HasRuleForHeatSeries(heatSeries =>
                               {
                                   // ... rules here
                               })
                               .HasRuleForFinancialSeries(financialSeries =>
                               {
                                   financialSeries.UpFill = DefaultPaintTask;
                                   financialSeries.DownFill = DefaultPaintTask;
                                   financialSeries.UpStroke = DefaultPaintTask;
                                   financialSeries.DownStroke = DefaultPaintTask;
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
                    (Color[] colors, IChartSeries<SkiaSharpDrawingContext> series, bool forceApply) =>
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
                            var gaugeSeries = (IPieSeries<SkiaSharpDrawingContext>)series;
                            if (gaugeSeries.Stroke == DefaultPaintTask) gaugeSeries.Stroke = null;
                            if (gaugeSeries.Fill == DefaultPaintTask) gaugeSeries.Fill = new SolidColorPaintTask(new SKColor(0, 0, 0, 15));
                            return;
                        }

                        var color = colors[series.SeriesId % colors.Length];
                        series.Name ??= $"Series {series.SeriesId + 1}";

                        if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries ||
                            (series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
                        {
                            var sf = (IStrokedAndFilled<SkiaSharpDrawingContext>)series;

                            if (sf.Fill == DefaultPaintTask) sf.Fill = new SolidColorPaintTask(color.AsSKColor());
                            if (sf.Stroke == DefaultPaintTask) sf.Stroke = new SolidColorPaintTask(color.AsSKColor(), 3);

                            return;
                        }

                        if (series is IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled)
                        {
                            if (strokedAndFilled.Fill == DefaultPaintTask)
                            {
                                var opacity = 0.2;
                                var mask1 = SeriesProperties.Line | SeriesProperties.Stacked;
                                var mask2 = SeriesProperties.StepLine | SeriesProperties.Stacked;
                                if ((series.SeriesProperties & mask1) == mask1 || (series.SeriesProperties & mask2) == mask2)
                                    opacity = 1;

                                strokedAndFilled.Fill = new SolidColorPaintTask(color.AsSKColor((byte)(opacity * 255)));
                            }
                            if (strokedAndFilled.Stroke == DefaultPaintTask)
                                strokedAndFilled.Stroke = new SolidColorPaintTask(color.AsSKColor(), 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                        {
                            var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                            if (lineSeries.GeometryFill == DefaultPaintTask)
                                lineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (lineSeries.GeometryStroke == DefaultPaintTask)
                                lineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), lineSeries.Stroke?.StrokeThickness ?? 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.StepLine) == SeriesProperties.StepLine)
                        {
                            var steplineSeries = (IStepLineSeries<SkiaSharpDrawingContext>)series;

                            if (steplineSeries.GeometryFill == DefaultPaintTask)
                                steplineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (steplineSeries.GeometryStroke == DefaultPaintTask)
                                steplineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), steplineSeries.Stroke?.StrokeThickness ?? 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.Financial) == SeriesProperties.Financial)
                        {
                            var financialSeries = (IFinancialSeries<SkiaSharpDrawingContext>)series;

                            if (financialSeries.UpFill == DefaultPaintTask)
                                financialSeries.UpFill = new SolidColorPaintTask(new SKColor(139, 195, 74, 255));
                            if (financialSeries.UpStroke == DefaultPaintTask)
                                financialSeries.UpStroke = new SolidColorPaintTask(new SKColor(139, 195, 74, 255), 3);
                            if (financialSeries.DownFill == DefaultPaintTask)
                                financialSeries.DownFill = new SolidColorPaintTask(new SKColor(239, 83, 80, 255));
                            if (financialSeries.DownStroke == DefaultPaintTask)
                                financialSeries.DownStroke = new SolidColorPaintTask(new SKColor(239, 83, 80, 255), 3);
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

                        if (axis.NamePaint == DefaultPaintTask)
                            axis.NamePaint = new SolidColorPaintTask(new SKColor(35, 35, 35));

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
                    (Color[] colors, IChartSeries<SkiaSharpDrawingContext> series, bool forceApply) =>
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
                            var gaugeSeries = (IPieSeries<SkiaSharpDrawingContext>)series;
                            if (gaugeSeries.Stroke == DefaultPaintTask) gaugeSeries.Stroke = null;
                            if (gaugeSeries.Fill == DefaultPaintTask) gaugeSeries.Fill = new SolidColorPaintTask(new SKColor(255, 255, 255, 15));
                            return;
                        }

                        var color = colors[series.SeriesId % colors.Length];
                        series.Name ??= $"Series {series.SeriesId + 1}";

                        if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries ||
                            (series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
                        {
                            var sf = (IStrokedAndFilled<SkiaSharpDrawingContext>)series;
                            if (sf.Fill == DefaultPaintTask) sf.Fill = new SolidColorPaintTask(color.AsSKColor());
                            if (sf.Stroke == DefaultPaintTask) sf.Stroke = new SolidColorPaintTask(color.AsSKColor(), 3);

                            return;
                        }

                        if (series is IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled)
                        {
                            if (strokedAndFilled.Fill == DefaultPaintTask)
                            {
                                var opacity = 0.2;
                                var mask1 = SeriesProperties.Line | SeriesProperties.Stacked;
                                var mask2 = SeriesProperties.StepLine | SeriesProperties.Stacked;
                                if ((series.SeriesProperties & mask1) == mask1 || (series.SeriesProperties & mask2) == mask2)
                                    opacity = 1;

                                strokedAndFilled.Fill = new SolidColorPaintTask(color.AsSKColor((byte)(opacity * 255)));
                            }
                            if (strokedAndFilled.Stroke == DefaultPaintTask)
                                strokedAndFilled.Stroke = new SolidColorPaintTask(color.AsSKColor(), 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                        {
                            var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                            if (lineSeries.GeometryFill == DefaultPaintTask)
                                lineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (lineSeries.GeometryStroke == DefaultPaintTask)
                                lineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), lineSeries.Stroke?.StrokeThickness ?? 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.StepLine) == SeriesProperties.StepLine)
                        {
                            var steplineSeries = (IStepLineSeries<SkiaSharpDrawingContext>)series;

                            if (steplineSeries.GeometryFill == DefaultPaintTask)
                                steplineSeries.GeometryFill = new SolidColorPaintTask(color.AsSKColor());
                            if (steplineSeries.GeometryStroke == DefaultPaintTask)
                                steplineSeries.GeometryStroke =
                                    new SolidColorPaintTask(color.AsSKColor(), steplineSeries.Stroke?.StrokeThickness ?? 5);
                        }

                        if ((series.SeriesProperties & SeriesProperties.Financial) == SeriesProperties.Financial)
                        {
                            var financialSeries = (IFinancialSeries<SkiaSharpDrawingContext>)series;

                            if (financialSeries.UpFill == DefaultPaintTask)
                                financialSeries.UpFill = new SolidColorPaintTask(new SKColor(139, 195, 74, 255));
                            if (financialSeries.UpStroke == DefaultPaintTask)
                                financialSeries.UpStroke = new SolidColorPaintTask(new SKColor(139, 195, 74, 255), 3);
                            if (financialSeries.DownFill == DefaultPaintTask)
                                financialSeries.DownFill = new SolidColorPaintTask(new SKColor(239, 83, 80, 255));
                            if (financialSeries.DownStroke == DefaultPaintTask)
                                financialSeries.DownStroke = new SolidColorPaintTask(new SKColor(239, 83, 80, 255), 3);
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

                        if (axis.NamePaint == DefaultPaintTask)
                            axis.NamePaint = new SolidColorPaintTask(new SKColor(235, 235, 235));

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
