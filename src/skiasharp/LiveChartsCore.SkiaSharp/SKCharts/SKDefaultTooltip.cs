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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public class SKDefaultTooltip : IChartTooltip<SkiaSharpDrawingContext>, IImageControl
{
    private static readonly int s_zIndex = 10050;
    private Chart<SkiaSharpDrawingContext>? _chart;
    private readonly HashSet<IPaint<SkiaSharpDrawingContext>> _paints = new();
    private IPaint<SkiaSharpDrawingContext>? _backgroundPaint;
    private IPaint<SkiaSharpDrawingContext>? _fontPaint;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;
    private readonly Dictionary<IChartSeries<SkiaSharpDrawingContext>, StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>> _activeSeries = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip()
    {
        FontPaint = new SolidColorPaint(new SKColor(250, 250, 250, 255));
        BackgroundPaint = new SolidColorPaint(new SKColor(35, 35, 35, 200));
    }

    /// <summary>
    /// Gets the location of the tooltip.
    /// </summary>
    public LvcPoint Location { get; private set; }

    /// <inheritdoc cref="IImageControl.Size"/>
    public LvcSize Size { get; private set; }

    /// <summary>
    /// Gets or sets the legend font paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? FontPaint
    {
        get => _fontPaint;
        set
        {
            _fontPaint = value;
            if (value is not null) _ = _paints.Add(value);
        }
    }

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
                _ = _paints.Add(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the fonts size.
    /// </summary>
    public double FontSize { get; set; } = 16;

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Show(IEnumerable{ChartPoint}, Chart{TDrawingContext})"/>
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _chart = chart;

        if (BackgroundPaint is not null) BackgroundPaint.ZIndex = s_zIndex;
        if (FontPaint is not null) FontPaint.ZIndex = s_zIndex + 1;

        var sp = _stackPanel ??= new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Padding(12, 8),
            Orientation = ContainerOrientation.Vertical,
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle,
            BackgroundPaint = BackgroundPaint
        };

        var toRemoveSeries = new List<VisualElement<SkiaSharpDrawingContext>>(_stackPanel.Children);
        foreach (var series in chart.ChartSeries)
        {
            var seriesMiniatureVisual = GetSeriesVisual(series, _chart);
            _ = toRemoveSeries.Remove(seriesMiniatureVisual);
        }

        Size = sp.Measure(chart, null, null);

        LvcPoint? location = null;

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            location = foundPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, Size, chart.DrawMarginSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = foundPoints.GetPieTooltipLocation(
                chart.TooltipPosition, Size);
        }

        if (location is null) throw new Exception("location not supported");

        Location = location.Value;
        sp.X = Location.X;
        sp.Y = Location.Y;

        chart.AddVisual(sp);

        foreach (var visual in toRemoveSeries)
        {
            _ = _stackPanel.Children.Remove(visual);
            chart.RemoveVisual(visual);
        }
    }

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Hide"/>
    public void Hide()
    {
        if (_chart is null || _stackPanel is null) return;
        _chart.RemoveVisual(_stackPanel);
    }

    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> GetSeriesVisual(
        IChartSeries<SkiaSharpDrawingContext> series, Chart<SkiaSharpDrawingContext>? _chart)
    {
        if (_activeSeries.TryGetValue(series, out var seriesPanel)) return seriesPanel;

        var sketch = series.GetMiniatresSketch();

        var relativePanel = new RelativePanel<SkiaSharpDrawingContext>
        {
            Size = new LvcSize((float)sketch.Width, (float)sketch.Height)
        };

        foreach (var schedule in sketch.PaintSchedules)
        {
            foreach (var g in schedule.Geometries)
            {
                var sizedGeometry = (ISizedGeometry<SkiaSharpDrawingContext>)g;
                var vgv = new VariableGeometryVisual(sizedGeometry)
                {
                    Width = sizedGeometry.Width,
                    Height = sizedGeometry.Height,
                };

                schedule.PaintTask.ZIndex = schedule.PaintTask.ZIndex + 1 + s_zIndex;

                if (schedule.PaintTask.IsFill) vgv.Fill = schedule.PaintTask;
                if (schedule.PaintTask.IsStroke) vgv.Stroke = schedule.PaintTask;
                _ = relativePanel.Children.Add(vgv);
            }
        }

        var sp = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Padding(0, 4),
            VerticalAlignment = Align.Middle,
            HorizontalAlignment = Align.Middle,
            Children =
            {
                relativePanel,
                new LabelVisual
                {
                    Text = series.Name ?? string.Empty,
                    Paint = FontPaint,
                    TextSize = FontSize,
                    Padding = new Padding(8, 0, 0, 0),
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                }
            }
        };

        _ = _stackPanel?.Children.Add(sp);
        _activeSeries.Add(series, sp);

        return sp;
    }
}
