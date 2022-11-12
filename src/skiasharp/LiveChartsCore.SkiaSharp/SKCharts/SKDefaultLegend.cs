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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IChartLegend{TDrawingContext}" />
public class SKDefaultLegend : IChartLegend<SkiaSharpDrawingContext>, IImageControl
{
    private static readonly int s_zIndex = 10050;
    private ContainerOrientation _orientation = ContainerOrientation.Vertical;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;
    private readonly Dictionary<IChartSeries<SkiaSharpDrawingContext>, StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>> _activeSeries = new();
    private new List<VisualElement<SkiaSharpDrawingContext>> _toRemoveSeries = new();

    public SKDefaultLegend()
    {
        FontPaint = new SolidColorPaint(new SKColor(30, 30, 30, 255));
    }

    /// <inheritdoc cref="IImageControl.Size"/>
    public LvcSize Size { get; set; }

    /// <summary>
    /// Gets or sets the legend font paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? FontPaint { get; set; }

    /// <summary>
    /// Gets or sets the fonts size.
    /// </summary>
    public double FontSize { get; set; } = 13;

    void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        if (chart.Legend is null || chart.LegendPosition == LegendPosition.Hidden) return;

        Measure(chart);

        if (_stackPanel is null) return;
        if (FontPaint is not null) FontPaint.ZIndex = s_zIndex + 1;

        var actualChartSize = chart.ControlSize;

        if (chart.LegendPosition == LegendPosition.Top)
        {
            chart.Canvas.StartPoint = new LvcPoint(0, Size.Height);
            _stackPanel.X = actualChartSize.Width * 0.5f - Size.Width * 0.5f;
            _stackPanel.Y = -Size.Height;
        }
        if (chart.LegendPosition == LegendPosition.Bottom)
        {
            _stackPanel.X = actualChartSize.Width * 0.5f - Size.Width * 0.5f;
            _stackPanel.Y = actualChartSize.Height;
        }
        if (chart.LegendPosition == LegendPosition.Left)
        {
            chart.Canvas.StartPoint = new LvcPoint(Size.Width, 0);
            _stackPanel.X = -Size.Width;
            _stackPanel.Y = actualChartSize.Height * 0.5f - Size.Height * 0.5f;
        }
        if (chart.LegendPosition == LegendPosition.Right)
        {
            _stackPanel.X = actualChartSize.Width;
            _stackPanel.Y = actualChartSize.Height * 0.5f - Size.Height * 0.5f;
        }

        //var x = location.Value.X + canvasLocation.X;
        //var y = location.Value.Y + canvasLocation.Y;
        //var s = chart.ControlSize;
        //var w = s.Width;
        //var h = s.Height;

        //if (location.Value.X + Bounds.Width > w) x = w - Bounds.Width;
        //if (location.Value.X < 0) x = 0;
        //if (location.Value.Y < 0) y = 0;
        //if (location.Value.Y + Bounds.Height > h) x = h - Bounds.Height;

        chart.AddVisual(_stackPanel);

        foreach (var visual in _toRemoveSeries)
        {
            _ = _stackPanel.Children.Remove(visual);
            chart.RemoveVisual(visual);
        }
    }

    private void BuildLayout(Chart<SkiaSharpDrawingContext> chart)
    {
        _orientation = chart.LegendPosition is LegendPosition.Left or LegendPosition.Right
            ? ContainerOrientation.Vertical
            : ContainerOrientation.Horizontal;

        _stackPanel ??= new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Padding(0),
            Orientation = _orientation,
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle
        };

        _toRemoveSeries = new List<VisualElement<SkiaSharpDrawingContext>>(_stackPanel.Children);

        foreach (var series in chart.ChartSeries)
        {
            var seriesMiniatureVisual = GetSeriesVisual(series);
            _ = _toRemoveSeries.Remove(seriesMiniatureVisual);
        }
    }

    /// <inheritdoc cref="IImageControl.Measure(IChart)"/>
    public void Measure(IChart chart)
    {
        var skiaChart = (Chart<SkiaSharpDrawingContext>)chart;
        BuildLayout(skiaChart);
        if (_stackPanel is null) return;
        Size = _stackPanel.Measure(skiaChart, null, null);
    }

    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> GetSeriesVisual(IChartSeries<SkiaSharpDrawingContext> series)
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
            Padding = new Padding(10, 4),
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
