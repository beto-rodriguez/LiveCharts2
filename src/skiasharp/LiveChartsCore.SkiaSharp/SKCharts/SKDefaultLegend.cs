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

using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IChartLegend{TDrawingContext}" />
public class SKDefaultLegend : IChartLegend<SkiaSharpDrawingContext>, IImageControl
{
    /// <inheritdoc cref="IImageControl.Size"/>
    public LvcSize Size { get; set; }

    void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        DrawOrMeasure(chart, false);
        DrawOrMeasure(chart, true);
    }

    private void DrawOrMeasure(Chart<SkiaSharpDrawingContext> chart, bool draw)
    {
        float xi, yi;
        var p = 8f;

        if (draw)
        {
            var actualChartSize = chart.View.ControlSize;
            xi = actualChartSize.Width;
            yi = actualChartSize.Height * 0.5f - Size.Height * 0.5f;
        }
        else
        {
            xi = 0f;
            yi = 0f;
        }

        var drawnLegendChart = (IDrawnLegend)chart.View;
        if (drawnLegendChart.LegendFontPaint is null) return;

        var series = chart.ChartSeries.Where(x => x.IsVisibleAtLegend);
        var legendOrientation = chart.LegendOrientation;
        var legendPosition = chart.LegendPosition;

        var drawing = draw ? chart.Canvas.Draw() : null;

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

                                return GetMaxSize(current, new LvcSize(size.Width + t, size.Height + t));
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
                    TextSize = (float)drawnLegendChart.LegendFontSize
                };

                return GetMaxSize(current, label.Measure(drawnLegendChart.LegendFontPaint));
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
                if (draw) _ = drawing!.SelectPaint(miniatureSchedule.PaintTask);

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

            if (drawnLegendChart.LegendFontPaint is not null)
            {
                var label = new LabelGeometry
                {
                    X = x,
                    Y = y,
                    HorizontalAlign = Align.Start,
                    VerticalAlign = Align.Start,
                    Text = s.Name ?? string.Empty,
                    TextSize = (float)drawnLegendChart.LegendFontSize
                };
                var size = label.Measure(drawnLegendChart.LegendFontPaint);

                var cy = (maxY - size.Height) * 0.5f;
                label.Y = y + cy;

                if (draw)
                    _ = drawing!
                        .SelectPaint(drawnLegendChart.LegendFontPaint)
                        .Draw(label);

                x += labelMaxSize.Width + p;
            }

            y += maxY;
            if (!draw) Size = GetMaxSize(Size, new LvcSize(x - xi, y - yi));
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
