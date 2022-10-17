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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Themes;

/// <summary>
/// Defines the dark theme extensions.
/// </summary>
public static class BaseThemeExtensions
{
    /// <summary>
    /// Adds the light theme.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="colorPallete">The color pallete.</param>
    /// <param name="primaryColor">The primary color.</param>
    /// <param name="secondaryColor">The secondary color.</param>
    /// <param name="animationsSpeed">The animations speed.</param>
    /// <param name="easing">The easing.</param>
    /// <param name="baseTextSize">The base text size.</param>
    /// <param name="baseGeometrySize">The base geometry size.</param>
    /// <param name="baseStrokeThickness">The base stroke thickness.</param>
    /// <param name="financialUpColor">The financial up color.</param>
    /// <param name="financialDownColor">The financial down color.</param>
    /// <returns></returns>
    public static LiveChartsSettings AddTheme(
        this LiveChartsSettings settings,
        SKColor[]? colorPallete,
        SKColor primaryColor,
        SKColor secondaryColor,
        TimeSpan? animationsSpeed = null,
        Func<float, float>? easing = null,
        double? baseTextSize = null,
        double? baseGeometrySize = null,
        double? baseStrokeThickness = null,
        SKColor? financialUpColor = null,
        SKColor? financialDownColor = null)
    {
        colorPallete ??= ColorPalletes.MaterialDesign500.AsSKColors();
        animationsSpeed ??= TimeSpan.FromMilliseconds(800);
        easing ??= EasingFunctions.ExponentialOut;
        baseTextSize ??= 18;
        baseGeometrySize ??= 18;
        baseStrokeThickness ??= 3;
        financialUpColor ??= new SKColor(139, 195, 74, 255);
        financialDownColor ??= new SKColor(239, 83, 80, 255);

        GaugeBuilder.DefaultLabelsPaint = new SolidColorPaint(primaryColor);

        return settings
            .HasTheme((Theme<SkiaSharpDrawingContext> theme) =>
            {
                _ = theme
                    .SetRuleFor<IChartView<SkiaSharpDrawingContext>>(chart =>
                    {
                        chart.AnimationsSpeed = animationsSpeed.Value;
                        chart.EasingFunction = easing;
                    })
                    .SetRuleFor<IPlane<SkiaSharpDrawingContext>>(plane =>
                    {
                        plane.TextSize = baseTextSize.Value;
                        plane.ShowSeparatorLines = true;

                        plane.NamePaint = new SolidColorPaint(primaryColor);
                        plane.LabelsPaint = new SolidColorPaint(primaryColor);

                        if (plane is ICartesianAxis<SkiaSharpDrawingContext> cartesianAxis)
                        {
                            cartesianAxis.SeparatorsPaint = cartesianAxis.Orientation == Measure.AxisOrientation.X
                                ? null
                                : new SolidColorPaint(primaryColor.WithAlpha((byte)(255 * 0.125)));
                            cartesianAxis.Padding = new Padding(8);
                        }
                        else
                        {
                            plane.SeparatorsPaint = new SolidColorPaint((byte)(255 * 0.125));
                        }
                    })
                    .SetRuleFor<ISeries<SkiaSharpDrawingContext>>(series =>
                    {
                        series.Name ??= $"Series {series.SeriesId + 1}";
                    })
                    .SetRuleFor<IBarSeries<SkiaSharpDrawingContext>>(barSeries =>
                    {
                        var color = colorPallete[barSeries.SeriesId % colorPallete.Length];
                        barSeries.Fill = new SolidColorPaint(color);
                        barSeries.Stroke = null;

                        if (barSeries.IsStackedSeries())
                        {
                            barSeries.Rx = 0;
                            barSeries.Ry = 0;
                        }
                        else
                        {
                            barSeries.Rx = 4;
                            barSeries.Ry = 4;
                        }
                    })
                    .SetRuleFor<ILineSeries<SkiaSharpDrawingContext>>(lineSeries =>
                    {
                        var color = colorPallete[lineSeries.SeriesId % colorPallete.Length];

                        if (lineSeries.IsStackedSeries())
                        {
                            lineSeries.Stroke = null;
                            lineSeries.Fill = new SolidColorPaint(color);
                            lineSeries.GeometrySize = 0;
                            lineSeries.GeometryFill = null;
                            lineSeries.GeometryStroke = null;
                        }
                        else
                        {
                            lineSeries.Stroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                            lineSeries.Fill = new SolidColorPaint(color.StackAlpha(0.2f));
                            lineSeries.GeometrySize = baseGeometrySize.Value;
                            lineSeries.GeometryFill = new SolidColorPaint(secondaryColor.Tint(0.5f));
                            lineSeries.GeometryStroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                        }
                    })
                    .SetRuleFor<IStepLineSeries<SkiaSharpDrawingContext>>(stepLineSeries =>
                    {
                        var color = colorPallete[stepLineSeries.SeriesId % colorPallete.Length];

                        if (stepLineSeries.IsStackedSeries())
                        {
                            stepLineSeries.Stroke = null;
                            stepLineSeries.Fill = new SolidColorPaint(color);
                            stepLineSeries.GeometrySize = 0;
                            stepLineSeries.GeometryFill = null;
                            stepLineSeries.GeometryStroke = null;
                        }
                        else
                        {
                            stepLineSeries.Stroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                            stepLineSeries.Fill = new SolidColorPaint(color.StackAlpha(0.2f));
                            stepLineSeries.GeometrySize = baseGeometrySize.Value;
                            stepLineSeries.GeometryFill = new SolidColorPaint(secondaryColor.Tint(0.5f));
                            stepLineSeries.GeometryStroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                        }
                    })
                    .SetRuleFor<IPieSeries<SkiaSharpDrawingContext>>(pieSeries =>
                    {
                        var color = colorPallete[pieSeries.SeriesId % colorPallete.Length];

                        pieSeries.Fill = new SolidColorPaint(color);
                        pieSeries.Stroke = null;
                        pieSeries.Pushout = 0;
                    })
                    .SetRuleFor<IFinancialSeries<SkiaSharpDrawingContext>>(financialSeries =>
                    {
                        financialSeries.UpFill = new SolidColorPaint(financialUpColor.Value);
                        financialSeries.UpStroke = new SolidColorPaint(financialUpColor.Value, (float)baseStrokeThickness.Value);
                        financialSeries.DownFill = new SolidColorPaint(financialDownColor.Value);
                        financialSeries.DownStroke = new SolidColorPaint(financialDownColor.Value, (float)baseStrokeThickness.Value);
                    })
                    .SetRuleFor<IPolarLineSeries<SkiaSharpDrawingContext>>(polarLine =>
                    {
                        var color = colorPallete[polarLine.SeriesId % colorPallete.Length];

                        polarLine.Stroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                        polarLine.Fill = new SolidColorPaint(color.StackAlpha(0.2f));
                        polarLine.GeometrySize = baseGeometrySize.Value;
                        polarLine.GeometryFill = new SolidColorPaint(secondaryColor.Tint(0.5f));
                        polarLine.GeometryStroke = new SolidColorPaint(color, (float)baseStrokeThickness.Value);
                    });
            });
    }
}
