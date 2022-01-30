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

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public class DefaultTooltip : FloatingForm, IChartTooltip<SkiaSharpDrawingContext>
{
    IEnumerable<ChartPoint> tooltipPoints;
    Chart<SkiaSharpDrawingContext> chart;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
    /// </summary>
    public DefaultTooltip()
    {
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
        Resizable = false;
        AutoSize = true;
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        this.chart = chart;
        this.tooltipPoints = tooltipPoints;

        var wfChart = (Chart)chart.View;

        DrawAndMeasure(tooltipPoints, wfChart);

        SetLocation();

        Show();
    }
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        SetLocation();
    }
    private void SetLocation()
    {
        if (Height < 1 || Width < 1)
            return;

        LvcPoint? location = null;

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize(Width, Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize(Width, Height));
        }
        if (location is null) throw new Exception("location not supported");

        var wfChart = (Chart)chart.View;
        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + location.Value.X;
        var y = l.Y + location.Value.Y;

        Location = new Point((int)x, (int)y);
    }
    private void DrawAndMeasure(IEnumerable<ChartPoint> tooltipPoints, Chart chart)
    {
        var container = new DynamicLayout() { BackgroundColor = chart.TooltipBackColor, Padding = new global::Eto.Drawing.Padding(4) };

        foreach (var point in tooltipPoints)
        {
            var drawableSeries = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            var marker = new MotionCanvas
            {
                PaintTasks = drawableSeries.CanvasSchedule.PaintSchedules,
                Width = (int)drawableSeries.CanvasSchedule.Width,
                Height = (int)drawableSeries.CanvasSchedule.Height
            };
            var label = new Label
            {
                Text = point.AsTooltipString,
                Font = chart.TooltipFont,
                TextColor = chart.TooltipTextColor,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _ = container.AddRow(marker, label);
        }
        _ = container.AddRow(); // workaround ! empty row else last label renders incorrectly

        Content = new Scrollable { Content = container }; // wrap inside a scrollable just to get a border !
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        Visible = false;
    }
}
