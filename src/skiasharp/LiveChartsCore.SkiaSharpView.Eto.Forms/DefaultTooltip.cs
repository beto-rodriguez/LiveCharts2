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
using Eto.Forms;
using Eto.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Eto.Forms;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public class DefaultTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
    /// </summary>
    public DefaultTooltip()
    {
        BackgroundColor = Colors.Transparent;
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        var size = DrawAndMeasure(tooltipPoints, wfChart);
        LvcPoint? location = null;

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize(size.Width, size.Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize(size.Width, size.Height));
        }
        if (location is null) throw new Exception("location not supported");

        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + location.Value.X;
        var y = l.Y + location.Value.Y;
        Location = new Point((int)x, (int)y);

        Show();
    }
    private SizeF DrawAndMeasure(IEnumerable<ChartPoint> tooltipPoints, Chart chart)
    {
        var h = 0f;
        var w = 0f;

        var container = new DynamicLayout() { BackgroundColor = chart.BackgroundColor, Padding = new global::Eto.Drawing.Padding(4) };

        foreach (var point in tooltipPoints)
        {
            var text = point.AsTooltipString;
            var size = chart.TooltipFont.MeasureString(text);

            var drawableSeries = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            var marker = new MotionCanvas
            {
                PaintTasks = drawableSeries.CanvasSchedule.PaintSchedules,
                Width = (int)drawableSeries.CanvasSchedule.Width,
                Height = (int)drawableSeries.CanvasSchedule.Height
            };
            var label = new Label
            {
                Text = text,
                Font = chart.TooltipFont,
                TextColor = chart.TooltipTextColor,
            };

            _ = container.AddRow(marker, label);

            var thisW = size.Width + (float) drawableSeries.CanvasSchedule.Width;

            h += Math.Max(marker.Height, size.Height);
            w = Math.Max(thisW, w);
        }

        Content = container;

        return new SizeF(w + 25, h + 25);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        Visible = false;
    }
}
