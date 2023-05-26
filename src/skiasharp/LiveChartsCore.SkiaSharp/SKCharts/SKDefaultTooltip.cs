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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

public class SKDefaultTooltip : IChartTooltip<SkiaSharpDrawingContext>
{
    internal StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _panel;
    private static readonly int s_zIndex = 10050;
    private IPaint<SkiaSharpDrawingContext>? _backgroundPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip()
    {
        FontPaint = new SolidColorPaint(new SKColor(28, 49, 58));
        BackgroundPaint = new SolidColorPaint(new SKColor(240, 240, 240, 200))
        {
            ImageFilter = new DropShadow(2, 2, 2, 2, new SKColor(30, 30, 30, 60))
        };
    }

    /// <summary>
    /// Gets or sets the legend font paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? FontPaint { get; set; }

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? BackgroundPaint
    {
        get => _backgroundPaint;
        set
        {
            _backgroundPaint = value;
            if (value is not null)
            {
                value.IsFill = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the fonts size.
    /// </summary>
    public double TextSize { get; set; } = 16;

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Show(IEnumerable{ChartPoint}, Chart{TDrawingContext})" />
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        if (_panel is null)
        {
            _panel = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(12, 8),
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Middle,
                VerticalAlignment = Align.Middle,
                BackgroundPaint = BackgroundPaint
            };

            _panel
                .Animate(chart,
                    nameof(RoundedRectangleGeometry.X),
                    nameof(RoundedRectangleGeometry.Y));
        }

        if (BackgroundPaint is not null) BackgroundPaint.ZIndex = s_zIndex;
        if (FontPaint is not null) FontPaint.ZIndex = s_zIndex + 1;

        foreach (var child in _panel.Children.ToArray())
        {
            _ = _panel.Children.Remove(child);
            chart.RemoveVisual(child);
        }

        var tableLayout = new TableLayout<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            HorizontalAlignment = Align.Middle,
            VerticalAlignment = Align.Middle
        };

        var i = 0;
        foreach (var point in foundPoints)
        {
            var series = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            if (i == 0)
            {
                _panel.Children.Add(
                    new LabelVisual
                    {
                        Text = point.GetFormattedSecondaryValueForToolTip(chart),
                        Paint = FontPaint,
                        TextSize = TextSize,
                        Padding = new Padding(0, 0, 0, 0),
                        VerticalAlignment = Align.Start,
                        HorizontalAlignment = Align.Start
                    });

                _panel.Children.Add(new StackPanel<RectangleGeometry, SkiaSharpDrawingContext> { Padding = new(0, 8) });
            }

            tableLayout.AddChild(series.GetMiniatresSketch().AsDrawnControl(), i, 0);
            tableLayout.AddChild(
                new LabelVisual
                {
                    Text = point.Context.Series.Name ?? string.Empty,
                    Paint = FontPaint,
                    TextSize = TextSize,
                    Padding = new Padding(8, 0, 0, 0),
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                }, i, 1, horizontalAlign: Align.Start);
            tableLayout.AddChild(
                new LabelVisual
                {
                    Text = point.GetFormattedPrimaryValueForToolTip(chart),
                    Paint = FontPaint,
                    TextSize = TextSize,
                    Padding = new Padding(16, 0, 0, 0),
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                }, i, 2, horizontalAlign: Align.End);

            i++;
        }

        _panel.Children.Add(tableLayout);

        var size = _panel.Measure(chart);
        var location = foundPoints.GetTooltipLocation(size, chart);

        _panel.X = location.X;
        _panel.Y = location.Y;

        chart.AddVisual(_panel);
    }

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Hide"/>
    public void Hide(Chart<SkiaSharpDrawingContext> chart)
    {
        if (chart is null || _panel is null) return;
        chart.RemoveVisual(_panel);
    }
}
