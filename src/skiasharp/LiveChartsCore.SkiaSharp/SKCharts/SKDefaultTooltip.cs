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
    private Chart<SkiaSharpDrawingContext>? _chart;
    private TableLayout<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _tableLayout;
    private static readonly int s_zIndex = 10050;
    private IPaint<SkiaSharpDrawingContext>? _backgroundPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip()
    {
        FontPaint = new SolidColorPaint(new SKColor(28, 49, 58));
        BackgroundPaint = new SolidColorPaint(new SKColor(240, 240, 240))
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

    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _chart = chart;

        if (_tableLayout is null)
        {
            _tableLayout = new TableLayout<RoundedRectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(12, 8),
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle,
                BackgroundPaint = BackgroundPaint
            };

            _tableLayout
                .Animate(chart,
                    nameof(ISizedGeometry<SkiaSharpDrawingContext>.X),
                    nameof(ISizedGeometry<SkiaSharpDrawingContext>.Y));
        }

        if (BackgroundPaint is not null) BackgroundPaint.ZIndex = s_zIndex;
        if (FontPaint is not null) FontPaint.ZIndex = s_zIndex + 1;

        foreach (var child in _tableLayout.EnumerateChildren())
        {
            _tableLayout.RemoveChildAt(child.Row, child.Column);
            chart.RemoveVisual(child.VisualElement);
        }

        var i = 0;
        foreach (var point in foundPoints)
        {
            var series = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            _tableLayout.AddChild(series.GetMiniatresSketch().AsDrawnControl(), i, 0);
            _tableLayout.AddChild(
                new LabelVisual
                {
                    Text = point.Context.Series.Name ?? string.Empty,
                    Paint = FontPaint,
                    TextSize = TextSize,
                    Padding = new Padding(8, 0, 0, 0),
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                }, i, 1);
            _tableLayout.AddChild(
                new LabelVisual
                {
                    Text = point.AsTooltipString,
                    Paint = FontPaint,
                    TextSize = TextSize,
                    Padding = new Padding(16, 0, 0, 0),
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                }, i, 2);

            i++;

            //_ = _tableLayout.AddChild(new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
            //{
            //    Padding = new Padding(0, 4),
            //    VerticalAlignment = Align.Middle,
            //    HorizontalAlignment = Align.Middle,
            //    Children =
            //    {
            //        series.GetMiniatresSketch().AsDrawnControl(),
            //        new LabelVisual
            //        {
            //            Text = point.AsTooltipString,
            //            Paint = FontPaint,
            //            TextSize = TextSize,
            //            Padding = new Padding(8, 0, 0, 0),
            //            VerticalAlignment = Align.Start,
            //            HorizontalAlignment = Align.Start
            //        }
            //    }
            //});
        }

        var size = _tableLayout.Measure(chart);
        var location = foundPoints.GetTooltipLocation(size, chart);

        _tableLayout.X = location.X;
        _tableLayout.Y = location.Y;

        _chart.AddVisual(_tableLayout);
    }

    public void Hide()
    {
        if (_chart is null || _tableLayout is null) return;
        _chart.RemoveVisual(_tableLayout);
    }
}
