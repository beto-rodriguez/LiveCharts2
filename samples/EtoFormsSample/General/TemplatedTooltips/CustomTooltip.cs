using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto.Forms;

namespace EtoFormsSample.General.TemplatedTooltips;

public class CustomTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>
{
    public CustomTooltip()
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

        if (chart is CartesianChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize(size.Width, size.Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize(size.Width, size.Height));
        }

        Content.BackgroundColor = Color.FromArgb(30, 30, 30);

        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + location.Value.X;
        var y = l.Y + location.Value.Y;
        Location = new Point((int)x, (int)y);

        Show();

        wfChart.CoreCanvas.Invalidate();
    }

    private SizeF DrawAndMeasure(IEnumerable<ChartPoint> tooltipPoints, Chart chart)
    {
        var h = 0f;
        var w = 0f;

        var container = new DynamicLayout() { BackgroundColor = chart.BackgroundColor, Padding = new Eto.Drawing.Padding(4) };

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
                TextColor = Color.FromArgb(250, 250, 250),
                Font = chart.TooltipFont,
            };

            _ = container.AddRow(marker, label);

            var thisW = size.Width + (float)drawableSeries.CanvasSchedule.Width;

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
