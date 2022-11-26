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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the light theme extensions.
/// </summary>
public static class LightThemeExtensions
{
    /// <summary>
    /// Adds the light theme.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="additionalStyles">the additional styles.</param>
    /// <returns></returns>
    public static LiveChartsSettings AddLightTheme(
        this LiveChartsSettings settings, Action<Theme<SkiaSharpDrawingContext>>? additionalStyles = null)
    {
        GaugeBuilder.DefaultLabelsPaint = new SolidColorPaint(new SKColor(40, 40, 40));

        return settings
            .HasTheme((Theme<SkiaSharpDrawingContext> theme) =>
            {
                _ = theme
                   .WithColors(ColorPalletes.MaterialDesign500)
                   .WithStyle(style =>
                       style
                           .HasRuleForCharts(chart =>
                           {
                               chart.AnimationsSpeed = TimeSpan.FromMilliseconds(800);
                               chart.EasingFunction = EasingFunctions.ExponentialOut;
                           })
                           .HasRuleForAxes(axis =>
                           {
                               axis.TextSize = 16;
                               axis.ShowSeparatorLines = true;
                               axis.NamePaint = LiveChartsSkiaSharp.DefaultPaint;
                               axis.LabelsPaint = LiveChartsSkiaSharp.DefaultPaint;
                               axis.SeparatorsPaint = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           // ForAnySeries() will be called for all the series
                           .HasRuleForAnySeries(series =>
                           {
                               if (series is not IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled) return;
                               strokedAndFilled.Fill = LiveChartsSkiaSharp.DefaultPaint;
                               strokedAndFilled.Stroke = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           .HasRuleForLineSeries(lineSeries =>
                           {
                               // at this point ForAnySeries() was already called
                               // we are configuring the missing properties
                               lineSeries.GeometrySize = 18;
                               lineSeries.GeometryFill = new SolidColorPaint(LvcColor.FromArgb(255, 250, 250, 250).AsSKColor());
                               lineSeries.GeometryStroke = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           .HasRuleForStepLineSeries(steplineSeries =>
                           {
                               // at this point ForAnySeries() was already called
                               // we are configuring the missing properties
                               steplineSeries.GeometrySize = 18;
                               steplineSeries.GeometryFill = new SolidColorPaint(LvcColor.FromArgb(255, 250, 250, 250).AsSKColor());
                               steplineSeries.GeometryStroke = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           .HasRuleForStackedLineSeries(stackedLine =>
                           {
                               // at this point both ForAnySeries() and ForLineSeries() were already called
                               // again we are correcting the previous settings
                               stackedLine.GeometrySize = 0;
                               stackedLine.GeometryFill = null;
                               stackedLine.GeometryStroke = null;
                               stackedLine.Stroke = null;
                               stackedLine.Fill = LiveChartsSkiaSharp.DefaultPaint;
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
                               pieSeries.Fill = LiveChartsSkiaSharp.DefaultPaint;
                               pieSeries.Stroke = null;
                               pieSeries.Pushout = 0;
                           })
                           .HasRuleForStackedStepLineSeries(stackedStep =>
                           {
                               stackedStep.GeometrySize = 0;
                               stackedStep.GeometryFill = null;
                               stackedStep.GeometryStroke = null;
                               stackedStep.Stroke = null;
                               stackedStep.Fill = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           .HasRuleForHeatSeries(heatSeries =>
                           {
                               // ... rules here
                           })
                           .HasRuleForFinancialSeries(financialSeries =>
                           {
                               financialSeries.UpFill = LiveChartsSkiaSharp.DefaultPaint;
                               financialSeries.DownFill = LiveChartsSkiaSharp.DefaultPaint;
                               financialSeries.UpStroke = LiveChartsSkiaSharp.DefaultPaint;
                               financialSeries.DownStroke = LiveChartsSkiaSharp.DefaultPaint;
                           })
                           .HasRuleForPolarLineSeries(polarLine =>
                           {
                               polarLine.GeometrySize = 18;
                               polarLine.GeometryFill = new SolidColorPaint(LvcColor.FromArgb(255, 250, 250, 250).AsSKColor());
                               polarLine.GeometryStroke = LiveChartsSkiaSharp.DefaultPaint;
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
    /// Adds the default resolvers.
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns></returns>
    public static Theme<SkiaSharpDrawingContext> AddDefaultLightResolvers(
        this Theme<SkiaSharpDrawingContext> theme)
    {
        return theme
            .WithSeriesDefaultsResolver(
                (LvcColor[] colors, IChartSeries<SkiaSharpDrawingContext> series, bool forceApply) =>
                {
                    if (forceApply)
                    {
                        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);
                        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                        var initializer = stylesBuilder.GetVisualsInitializer();

                        initializer.ApplyStyleToSeries(series);
                    }

                    if (series.DataLabelsPaint == LiveChartsSkiaSharp.DefaultPaint)
                    {
                        series.DataLabelsPaint = new SolidColorPaint(new SKColor(40, 40, 40));
                    }

                    if ((series.SeriesProperties & SeriesProperties.GaugeFill) != 0)
                    {
                        var gaugeSeries = (IPieSeries<SkiaSharpDrawingContext>)series;

                        if (gaugeSeries.Stroke == LiveChartsSkiaSharp.DefaultPaint)
                            gaugeSeries.Stroke = null;

                        if (gaugeSeries.Fill == LiveChartsSkiaSharp.DefaultPaint)
                            gaugeSeries.Fill = new SolidColorPaint(new SKColor(0, 0, 0, 15));

                        return;
                    }

                    var color = colors[series.SeriesId % colors.Length];
                    series.Name ??= $"Series {series.SeriesId + 1}";

                    if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries ||
                        (series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar)
                    {
                        var sf = (IStrokedAndFilled<SkiaSharpDrawingContext>)series;

                        if (sf.Fill == LiveChartsSkiaSharp.DefaultPaint) sf.Fill = new SolidColorPaint(color.AsSKColor());
                        if (sf.Stroke == LiveChartsSkiaSharp.DefaultPaint) sf.Stroke = null;

                        return;
                    }

                    if (series is IStrokedAndFilled<SkiaSharpDrawingContext> strokedAndFilled)
                    {
                        if (strokedAndFilled.Fill == LiveChartsSkiaSharp.DefaultPaint)
                        {
                            var opacity = 0.2;
                            var mask1 = SeriesProperties.Line | SeriesProperties.Stacked;
                            var mask2 = SeriesProperties.StepLine | SeriesProperties.Stacked;
                            if ((series.SeriesProperties & mask1) == mask1 || (series.SeriesProperties & mask2) == mask2)
                                opacity = 1;

                            strokedAndFilled.Fill = new SolidColorPaint(color.AsSKColor((byte)(opacity * 255)));
                        }
                        if (strokedAndFilled.Stroke == LiveChartsSkiaSharp.DefaultPaint)
                            strokedAndFilled.Stroke = new SolidColorPaint(color.AsSKColor(), 5);
                    }

                    if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
                    {
                        var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                        if (lineSeries.GeometryFill == LiveChartsSkiaSharp.DefaultPaint)
                            lineSeries.GeometryFill = new SolidColorPaint(color.AsSKColor());
                        if (lineSeries.GeometryStroke == LiveChartsSkiaSharp.DefaultPaint)
                            lineSeries.GeometryStroke =
                                new SolidColorPaint(color.AsSKColor(), lineSeries.Stroke?.StrokeThickness ?? 5);
                    }

                    if ((series.SeriesProperties & SeriesProperties.PolarLine) == SeriesProperties.PolarLine)
                    {
                        var polarLine = (IPolarLineSeries<SkiaSharpDrawingContext>)series;

                        if (polarLine.GeometryFill == LiveChartsSkiaSharp.DefaultPaint)
                            polarLine.GeometryFill = new SolidColorPaint(color.AsSKColor());
                        if (polarLine.GeometryStroke == LiveChartsSkiaSharp.DefaultPaint)
                            polarLine.GeometryStroke =
                                new SolidColorPaint(color.AsSKColor(), polarLine.Stroke?.StrokeThickness ?? 5);
                    }

                    if ((series.SeriesProperties & SeriesProperties.StepLine) == SeriesProperties.StepLine)
                    {
                        var steplineSeries = (IStepLineSeries<SkiaSharpDrawingContext>)series;

                        if (steplineSeries.GeometryFill == LiveChartsSkiaSharp.DefaultPaint)
                            steplineSeries.GeometryFill = new SolidColorPaint(color.AsSKColor());
                        if (steplineSeries.GeometryStroke == LiveChartsSkiaSharp.DefaultPaint)
                            steplineSeries.GeometryStroke =
                                new SolidColorPaint(color.AsSKColor(), steplineSeries.Stroke?.StrokeThickness ?? 5);
                    }

                    if ((series.SeriesProperties & SeriesProperties.Financial) == SeriesProperties.Financial)
                    {
                        var financialSeries = (IFinancialSeries<SkiaSharpDrawingContext>)series;

                        if (financialSeries.UpFill == LiveChartsSkiaSharp.DefaultPaint)
                            financialSeries.UpFill = new SolidColorPaint(new SKColor(139, 195, 74, 255));
                        if (financialSeries.UpStroke == LiveChartsSkiaSharp.DefaultPaint)
                            financialSeries.UpStroke = new SolidColorPaint(new SKColor(139, 195, 74, 255), 3);
                        if (financialSeries.DownFill == LiveChartsSkiaSharp.DefaultPaint)
                            financialSeries.DownFill = new SolidColorPaint(new SKColor(239, 83, 80, 255));
                        if (financialSeries.DownStroke == LiveChartsSkiaSharp.DefaultPaint)
                            financialSeries.DownStroke = new SolidColorPaint(new SKColor(239, 83, 80, 255), 3);
                    }
                })
            .WithAxisDefaultsResolver(
                (IPlane<SkiaSharpDrawingContext> plane, bool forceApply) =>
                {
                    if (forceApply)
                    {
                        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);
                        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                        var initializer = stylesBuilder.GetVisualsInitializer();

                        initializer.ApplyStyleToAxis(plane);
                    }

                    if (plane.NamePaint == LiveChartsSkiaSharp.DefaultPaint)
                        plane.NamePaint = new SolidColorPaint(new SKColor(35, 35, 35));

                    if (plane.LabelsPaint == LiveChartsSkiaSharp.DefaultPaint)
                        plane.LabelsPaint = new SolidColorPaint(new SKColor(90, 90, 90));

                    if (plane is ICartesianAxis cartesian)
                    {
                        if (plane.SeparatorsPaint == LiveChartsSkiaSharp.DefaultPaint)
                            plane.SeparatorsPaint = cartesian.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaint(new SKColor(235, 235, 235));

                        if (cartesian.Padding == Padding.Default) cartesian.Padding = new Padding(12);
                    }
                    else
                    {
                        if (plane.SeparatorsPaint == LiveChartsSkiaSharp.DefaultPaint)
                            plane.SeparatorsPaint = new SolidColorPaint(new SKColor(235, 235, 235));
                    }
                });
    }
}
