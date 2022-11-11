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
using System.Linq;
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
    private static readonly float s_controlPadding = 8f;
    private static readonly float s_geometryPadding = 4f;
    private Chart<SkiaSharpDrawingContext>? _chart;
    private readonly HashSet<IPaint<SkiaSharpDrawingContext>> _paints = new();
    private IPaint<SkiaSharpDrawingContext>? _backgroundPaint;
    private IPaint<SkiaSharpDrawingContext>? _fontPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip()
    {
        FontPaint = new SolidColorPaint(new SKColor(35, 35, 35, 255));
        BackgroundPaint = new SolidColorPaint(new SKColor(255, 255, 255, 240));
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
    public double FontSize { get; set; } = 14;

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Show(IEnumerable{ChartPoint}, Chart{TDrawingContext})"/>
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _chart = chart;
        DrawOrMeasure(chart, false);

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
        DrawOrMeasure(chart, true);
    }

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Hide"/>
    public void Hide()
    {
        if (_chart is null) return;

        foreach (var item in _paints)
        {
            _chart.Canvas.RemovePaintTask(item);
        }
    }

    private void DrawOrMeasure(Chart<SkiaSharpDrawingContext> chart, bool draw)
    {
        float xi = 0f, yi = 0f;
        var p = 8f;

        if (draw)
        {
            xi = Location.X + s_controlPadding;
            yi = Location.Y + s_controlPadding;
        }
        else
        {
            xi = 0f;
            yi = 0f;
        }

        var toDeletePaints = new HashSet<IPaint<SkiaSharpDrawingContext>>(_paints);

        if (FontPaint is null)
        {
            CollectUnusedPaints(chart, toDeletePaints);
            return;
        }

        var drawing = draw ? chart.Canvas.Draw() : null;

        FontPaint.ClearGeometriesFromPaintTask(chart.Canvas);
        _ = toDeletePaints.Remove(FontPaint);
        if (BackgroundPaint is not null)
        {
            if (draw)
            {
                BackgroundPaint.ClearGeometriesFromPaintTask(chart.Canvas);
                BackgroundPaint.ZIndex = s_zIndex;

                _ = drawing!
                    .SelectPaint(BackgroundPaint)
                    .Draw(new RoundedRectangleGeometry
                    {
                        X = Location.X,
                        Y = Location.Y,
                        Rx = 5,
                        Ry = 5,
                        Width = Size.Width,
                        Height = Size.Height,
                    });
            }

            _ = toDeletePaints.Remove(BackgroundPaint);
        }

        var series = chart.ChartSeries.Where(x => x.IsVisible);
        var legendOrientation = chart.LegendOrientation;
        var legendPosition = chart.LegendPosition;

        var miniatureMaxSize = series
            .Aggregate(new LvcSize(), (current, s) =>
            {
                var maxScheduleSize = s.CanvasSchedule.PaintSchedules
                    .Aggregate(new LvcSize(), (current, schedule) =>
                    {
                        var maxGeometrySize = schedule.Geometries.OfType<IGeometry<SkiaSharpDrawingContext>>()
                            .Aggregate(new LvcSize(), (current, geometry) =>
                            {
                                var size = geometry.Measure(schedule.PaintTask);
                                var t = schedule.PaintTask.StrokeThickness;

                                return GetMaxSize(current, new LvcSize(size.Width + t, size.Height + t + s_geometryPadding));
                            });

                        return GetMaxSize(current, maxGeometrySize);
                    });

                return GetMaxSize(current, maxScheduleSize);
            });

        var labelMaxSize = series
            .Aggregate(new LvcSize(), (current, s) =>
            {
                var label = new LabelGeometry
                {
                    HorizontalAlign = Align.Start,
                    VerticalAlign = Align.Start,
                    Text = s.Name ?? string.Empty,
                    TextSize = (float)FontSize
                };

                return GetMaxSize(current, label.Measure(FontPaint));
            });

        // set a padding
        miniatureMaxSize = new LvcSize(miniatureMaxSize.Width, miniatureMaxSize.Height);
        labelMaxSize = new LvcSize(labelMaxSize.Width + p, labelMaxSize.Height + p);

        var maxY = miniatureMaxSize.Height > labelMaxSize.Height ? miniatureMaxSize.Height : labelMaxSize.Height;
        var y = yi;

        foreach (var s in series)
        {
            var x = xi;

            foreach (var miniatureSchedule in s.CanvasSchedule.PaintSchedules)
            {
                if (draw)
                {
                    _ = drawing!.SelectPaint(miniatureSchedule.PaintTask);
                    miniatureSchedule.PaintTask.ZIndex = s_zIndex + 1;
                    _ = _paints.Add(miniatureSchedule.PaintTask);
                    _ = toDeletePaints.Remove(miniatureSchedule.PaintTask);
                }

                foreach (var geometry in miniatureSchedule.Geometries.Cast<IGeometry<SkiaSharpDrawingContext>>())
                {
                    var size = geometry.Measure(miniatureSchedule.PaintTask);
                    var t = miniatureSchedule.PaintTask.StrokeThickness;

                    // distance to center (in miniatureMaxSize) in the x and y axis
                    //var cx = 0;// (miniatureMaxSize.Width - (size.Width + t)) * 0.5f;
                    var cy = (maxY - (size.Height + t)) * 0.5f;

                    geometry.X = x; // + cx; // this is already centered by the LiveCharts API
                    geometry.Y = y + cy;

                    if (draw) _ = drawing!.Draw(geometry);
                }
            }

            x += miniatureMaxSize.Width + p;

            if (FontPaint is not null)
            {
                var label = new LabelGeometry
                {
                    X = x,
                    Y = y,
                    HorizontalAlign = Align.Start,
                    VerticalAlign = Align.Start,
                    Text = s.Name ?? string.Empty,
                    TextSize = (float)FontSize
                };
                var size = label.Measure(FontPaint);

                var cy = (maxY - size.Height) * 0.5f;
                label.Y = y + cy;

                if (draw)
                {
                    FontPaint.ZIndex = s_zIndex + 1;

                    _ = drawing!
                        .SelectPaint(FontPaint)
                        .Draw(label);
                }

                x += labelMaxSize.Width + p;
            }

            y += maxY;
            if (!draw) Size = GetMaxSize(Size, new LvcSize(x - xi, y - yi + 2 * s_controlPadding));
        }

        CollectUnusedPaints(chart, toDeletePaints);
    }

    private void CollectUnusedPaints(
        Chart<SkiaSharpDrawingContext> chart, IEnumerable<IPaint<SkiaSharpDrawingContext>> paints)
    {
        foreach (var item in paints)
        {
            chart.Canvas.RemovePaintTask(item);
        }
    }

    private LvcSize GetMaxSize(LvcSize size1, LvcSize size2)
    {
        var w = size1.Width;
        var h = size1.Height;

        if (size2.Width > w) w = size2.Width;
        if (size2.Height > h) h = size2.Height;

        return new LvcSize(w, h);
    }
}


/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public class SKDefaultTooltip2 : IChartTooltip<SkiaSharpDrawingContext>, IImageControl
{
    private static readonly int s_zIndex = 10050;
    private static readonly float s_controlPadding = 8f;
    private static readonly float s_geometryPadding = 4f;
    private Chart<SkiaSharpDrawingContext>? _chart;
    private readonly HashSet<IPaint<SkiaSharpDrawingContext>> _paints = new();
    private IPaint<SkiaSharpDrawingContext>? _backgroundPaint;
    private IPaint<SkiaSharpDrawingContext>? _fontPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultTooltip"/> class.
    /// </summary>
    public SKDefaultTooltip2()
    {
        FontPaint = new SolidColorPaint(new SKColor(35, 35, 35, 255));
        BackgroundPaint = new SolidColorPaint(new SKColor(255, 255, 255, 240));
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
    public double FontSize { get; set; } = 12;

    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Show(IEnumerable{ChartPoint}, Chart{TDrawingContext})"/>
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _chart = chart;

        var sp = _stackPanel ??= new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Orientation = ContainerOrientation.Vertical,
            HorizontalAlignment = Align.Middle,
            VerticalAlignment = Align.Middle,
            BackgroundPaint = new SolidColorPaint(SKColors.Gray),
        };

        foreach (var series in chart.ChartSeries)
        {
            _ = sp.Children.Add(new GeometryVisual<CircleGeometry>
            {
                Fill = new SolidColorPaint(SKColors.Red),
                Height = 50,
                Width = 50
            });

            _ = sp.Children.Add(new LabelVisual
            {
                Text = series.Name ?? string.Empty,
                Paint = FontPaint,
                TextSize = FontSize,
                Padding = new Padding(4),
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            });
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
        chart.RegisterAndInvalidateVisual(sp);
    }

    /// <inheritdoc cref="IChartTooltip{TDrawingContext}.Hide"/>
    public void Hide()
    {
        if (_chart is null) return;

        foreach (var item in _paints)
        {
            _chart.Canvas.RemovePaintTask(item);
        }
    }
}
