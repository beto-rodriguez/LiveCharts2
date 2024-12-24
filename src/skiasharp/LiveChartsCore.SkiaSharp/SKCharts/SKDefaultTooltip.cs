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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// Defines the default tooltip.
/// </summary>
public class SKDefaultTooltip : IChartTooltip
{
    internal Container<PopUpGeometry>? _container;
    private StackLayout? _layout;
    private Paint? _backgroundPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip()
    {
        FontPaint = new SolidColorPaint(new SKColor(28, 49, 58));
        BackgroundPaint = new SolidColorPaint(new SKColor(235, 235, 235, 230))
        {
            ImageFilter = new DropShadow(2, 2, 6, 6, new SKColor(50, 0, 0, 100))
        };
    }

    /// <summary>
    /// Gets or sets the legend font paint.
    /// </summary>
    public Paint? FontPaint { get; set; }

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public Paint? BackgroundPaint
    {
        get => _backgroundPaint;
        set
        {
            _backgroundPaint = value;
            if (value is not null)
                value.PaintStyle = PaintStyle.Fill;
        }
    }

    /// <summary>
    /// Gets or sets the fonts size.
    /// </summary>
    public double TextSize { get; set; } = 16;

    /// <summary>
    /// Gets or sets the easing function.
    /// </summary>
    public Func<float, float> Easing { get; set; } = EasingFunctions.EaseOut;

    /// <summary>
    /// Gets or sets the animations speed.
    /// </summary>
    public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(150);

    /// <inheritdoc cref="IChartTooltip.Show(IEnumerable{ChartPoint}, Chart)" />
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart chart)
    {
        const int wedge = 10;

        if (chart.View.TooltipTextSize is not null) TextSize = chart.View.TooltipTextSize.Value;
        if (chart.View.TooltipBackgroundPaint is not null) BackgroundPaint = chart.View.TooltipBackgroundPaint;
        if (chart.View.TooltipTextPaint is not null) FontPaint = chart.View.TooltipTextPaint;

        if (_container is null || _layout is null)
        {
            _container = new Container<PopUpGeometry>
            {
                Content = _layout = new StackLayout
                {
                    Orientation = ContainerOrientation.Vertical,
                    HorizontalAlignment = Align.Middle,
                    VerticalAlignment = Align.Middle
                }
            };

            _container.Geometry.Fill = BackgroundPaint;
            _container.Geometry.Wedge = wedge;
            _container.Geometry.WedgeThickness = 3;

            _container
                .Animate(
                    new Animation(Easing, AnimationsSpeed),
                    nameof(IDrawnElement.Opacity),
                    nameof(IDrawnElement.ScaleTransform),
                    nameof(IDrawnElement.X),
                    nameof(IDrawnElement.Y));

            var drawTask = chart.Canvas.AddGeometry(_container);
            drawTask.ZIndex = 10100;
        }

        _container.Opacity = 1;
        _container.ScaleTransform = new LvcPoint(1, 1);

        foreach (var child in _layout.Children.ToArray())
            _ = _layout.Children.Remove(child);

        var tableLayout = new TableLayout
        {
            HorizontalAlignment = Align.Middle,
            VerticalAlignment = Align.Middle
        };

        var lw = (float)LiveCharts.DefaultSettings.MaxTooltipsAndLegendsLabelsWidth;

        var i = 0;
        foreach (var point in foundPoints)
        {
            var series = point.Context.Series;

            if (i == 0)
            {
                var title = point.Context.Series.GetSecondaryToolTipText(point) ?? string.Empty;

                if (title != LiveCharts.IgnoreToolTipLabel)
                {
                    _layout.Children.Add(
                        new LabelGeometry
                        {
                            Text = point.Context.Series.GetSecondaryToolTipText(point) ?? string.Empty,
                            Paint = FontPaint,
                            TextSize = (float)TextSize,
                            Padding = new Padding(0, 0, 0, 8),
                            MaxWidth = lw,
                            VerticalAlign = Align.Start,
                            HorizontalAlign = Align.Start
                        });
                }
            }

            var content = point.Context.Series.GetPrimaryToolTipText(point) ?? string.Empty;

            var ltr = LiveCharts.DefaultSettings.IsRightToLeft;

            if (content != LiveCharts.IgnoreToolTipLabel)
            {
                var skiaMiniature = (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(point);
                _ = tableLayout.AddChild(skiaMiniature, i, ltr ? 3 : 0);

                if (point.Context.Series.Name != LiveCharts.IgnoreSeriesName)
                    _ = tableLayout.AddChild(new LabelGeometry
                    {
                        Text = point.Context.Series.Name ?? string.Empty,
                        Paint = FontPaint,
                        TextSize = (float)TextSize,
                        Padding = new Padding(10, 0, 0, 0),
                        MaxWidth = lw,
                        VerticalAlign = Align.Start,
                        HorizontalAlign = Align.Start
                    }, i, 1, horizontalAlign: Align.Start);

                _ = tableLayout.AddChild(new LabelGeometry
                {
                    Text = content,
                    Paint = FontPaint,
                    TextSize = (float)TextSize,
                    Padding = new Padding(10, 2, 0, 2),
                    MaxWidth = lw,
                    VerticalAlign = Align.Start,
                    HorizontalAlign = Align.Start
                }, i, ltr ? 0 : 2, horizontalAlign: Align.End);

                i++;
            }
        }

        _layout.Children.Add(tableLayout);

        var size = _layout.Measure();
        _ = foundPoints.GetTooltipLocation(size, chart);
        _container.Geometry.Placement = chart.AutoToolTipsInfo.ToolTipPlacement;

        const int px = 8;
        const int py = 12;

        switch (chart.AutoToolTipsInfo.ToolTipPlacement)
        {
            case Measure.PopUpPlacement.Top:
                _layout.Padding = new Padding(py, px, py, px + wedge); break;
            case Measure.PopUpPlacement.Bottom:
                _layout.Padding = new Padding(py, px + wedge, py, px); break;
            case Measure.PopUpPlacement.Left:
                _layout.Padding = new Padding(py, px, py + wedge, px); break;
            case Measure.PopUpPlacement.Right:
                _layout.Padding = new Padding(py + wedge, px, py, px); break;
            default: break;
        }

        // the size changed... we need to do the math again
        size = _container.Measure();
        var location = foundPoints.GetTooltipLocation(size, chart);

        _container.X = location.X;
        _container.Y = location.Y;

        chart.Canvas.Invalidate();
    }

    /// <inheritdoc cref="IChartTooltip.Hide"/>
    public void Hide(Chart chart)
    {
        if (chart is null || _container is null) return;
        _container.Opacity = 0f;
        _container.ScaleTransform = new LvcPoint(0.85f, 0.85f);

        chart.Canvas.Invalidate();
    }
}
