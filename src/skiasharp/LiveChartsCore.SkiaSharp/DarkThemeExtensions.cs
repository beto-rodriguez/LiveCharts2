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
/// Defines the dark theme extensions.
/// </summary>
public static class DarkThemeExtensions
{
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
                _ = LiveCharts.DefaultSettings
                    .WithAnimationsSpeed(TimeSpan.FromMilliseconds(800))
                    .WithEasingFunction(EasingFunctions.ExponentialOut)
                    .WithTooltipBackgroundPaint(new SolidColorPaint(new SKColor(45, 45, 45)))
                    .WithTooltipTextPaint(new SolidColorPaint(new SKColor(245, 245, 245)));

                var colors = ColorPalletes.MaterialDesign200;

                _ = theme
                    .HasRuleForAxes(axis =>
                    {
                        axis.TextSize = 16;
                        axis.ShowSeparatorLines = true;
                        axis.NamePaint = new SolidColorPaint(new SKColor(235, 235, 235));
                        axis.LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200));
                        if (axis is ICartesianAxis cartesian)
                        {
                            axis.SeparatorsPaint = cartesian.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaint(new SKColor(90, 90, 90));
                            cartesian.Padding = new Padding(12);
                        }
                        else
                        {
                            axis.SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90));
                        }
                    })
                    .HasRuleForLineSeries(lineSeries =>
                    {
                        var color = lineSeries.GetThemedColor(colors);

                        lineSeries.Name = $"Series #{lineSeries.SeriesId + 1}";
                        lineSeries.GeometrySize = 12;
                        lineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        lineSeries.GeometryFill = new SolidColorPaint(new SKColor(30, 30, 30));
                        lineSeries.Stroke = new SolidColorPaint(color, 4);
                        lineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStepLineSeries(steplineSeries =>
                    {
                        var color = steplineSeries.GetThemedColor(colors);

                        steplineSeries.Name = $"Series #{steplineSeries.SeriesId + 1}";
                        steplineSeries.GeometrySize = 12;
                        steplineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        steplineSeries.GeometryFill = new SolidColorPaint(new SKColor(30, 30, 30));
                        steplineSeries.Stroke = new SolidColorPaint(color, 4);
                        steplineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStackedLineSeries(stackedLine =>
                    {
                        var color = stackedLine.GetThemedColor(colors);

                        stackedLine.Name = $"Series #{stackedLine.SeriesId + 1}";
                        stackedLine.GeometrySize = 0;
                        stackedLine.GeometryStroke = null;
                        stackedLine.GeometryFill = null;
                        stackedLine.Stroke = null;
                        stackedLine.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForBarSeries(barSeries =>
                    {
                        var color = barSeries.GetThemedColor(colors);

                        barSeries.Name = $"Series #{barSeries.SeriesId + 1}";
                        barSeries.Stroke = null;
                        barSeries.Fill = new SolidColorPaint(color);
                        barSeries.Rx = 4;
                        barSeries.Ry = 4;
                    })
                    .HasRuleForStackedBarSeries(stackedBarSeries =>
                    {
                        var color = stackedBarSeries.GetThemedColor(colors);

                        stackedBarSeries.Name = $"Series #{stackedBarSeries.SeriesId + 1}";
                        stackedBarSeries.Stroke = null;
                        stackedBarSeries.Fill = new SolidColorPaint(color);
                        stackedBarSeries.Rx = 0;
                        stackedBarSeries.Ry = 0;
                    })
                    .HasRuleForPieSeries(pieSeries =>
                    {
                        var color = pieSeries.GetThemedColor(colors);

                        pieSeries.Name = $"Series #{pieSeries.SeriesId + 1}";
                        pieSeries.Stroke = null;
                        pieSeries.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForStackedStepLineSeries(stackedStep =>
                    {
                        var color = stackedStep.GetThemedColor(colors);

                        stackedStep.Name = $"Series #{stackedStep.SeriesId + 1}";
                        stackedStep.GeometrySize = 0;
                        stackedStep.GeometryStroke = null;
                        stackedStep.GeometryFill = null;
                        stackedStep.Stroke = null;
                        stackedStep.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForHeatSeries(heatSeries =>
                    {
                        // ... rules here
                    })
                    .HasRuleForFinancialSeries(financialSeries =>
                    {
                        financialSeries.Name = $"Series #{financialSeries.SeriesId + 1}";
                        financialSeries.UpFill = new SolidColorPaint(new SKColor(139, 195, 74, 255));
                        financialSeries.UpStroke = new SolidColorPaint(new SKColor(139, 195, 74, 255), 3);
                        financialSeries.DownFill = new SolidColorPaint(new SKColor(239, 83, 80, 255));
                        financialSeries.DownStroke = new SolidColorPaint(new SKColor(239, 83, 80, 255), 3);
                    })
                    .HasRuleForPolarLineSeries(polarLine =>
                    {
                        var color = polarLine.GetThemedColor(colors);

                        polarLine.Name = $"Series #{polarLine.SeriesId + 1}";
                        polarLine.GeometrySize = 12;
                        polarLine.GeometryStroke = new SolidColorPaint(color, 4);
                        polarLine.GeometryFill = new SolidColorPaint(new SKColor());
                        polarLine.Stroke = new SolidColorPaint(color, 4);
                        polarLine.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForGaugeSeries(gaugeSeries =>
                    {
                        var color = gaugeSeries.GetThemedColor(colors);

                        gaugeSeries.Name = $"Series #{gaugeSeries.SeriesId + 1}";
                        gaugeSeries.Stroke = null;
                        gaugeSeries.Fill = new SolidColorPaint(color);
                        gaugeSeries.DataLabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200));
                    })
                    .HasRuleForGaugeFillSeries(gaugeFill =>
                    {
                        gaugeFill.Fill = new SolidColorPaint(new SKColor(255, 255, 255, 30));
                    });

                additionalStyles?.Invoke(theme);
            });
    }

    private static SKColor GetThemedColor(this ISeries series, LvcColor[] colors)
    {
        return colors[series.SeriesId % colors.Length].AsSKColor();
    }
}
