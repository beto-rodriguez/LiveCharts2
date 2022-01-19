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

public partial class CustomTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
{
    public CustomTooltip()
    {
        InitializeComponent();
    }

    public void Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        var size = DrawAndMesure(tooltipPoints, wfChart);
        LvcPoint? location = null;

        if (chart is CartesianChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height));
        }

        BackColor = Color.FromArgb(255, 30, 30, 30);
        Height = (int)size.Height;
        Width = (int)size.Width;

        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + (int)location.Value.X;
        var y = l.Y + (int)location.Value.Y;
        Location = new Point(x, y);
        Show();

        wfChart.CoreCanvas.Invalidate();
    }

    private SizeF DrawAndMesure(IEnumerable<ChartPoint> tooltipPoints, Chart chart)
    {
        SuspendLayout();
        Controls.Clear();

        var h = 0f;
        var w = 0f;
        foreach (var point in tooltipPoints)
        {
            using var g = CreateGraphics();
            var text = point.AsTooltipString;
            var size = g.MeasureString(text, chart.TooltipFont);

            var drawableSeries = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            Controls.Add(new MotionCanvas
            {
                Location = new Point(6, (int)h + 6),
                PaintTasks = drawableSeries.CanvasSchedule.PaintSchedules,
                Width = (int)drawableSeries.CanvasSchedule.Width,
                Height = (int)drawableSeries.CanvasSchedule.Height
            });
            Controls.Add(new Label
            {
                Text = text,
                ForeColor = Color.FromArgb(255, 250, 250, 250),
                Font = chart.TooltipFont,
                Location = new Point(6 + (int)drawableSeries.CanvasSchedule.Width + 6, (int)h + 6),
                AutoSize = true
            });

            var thisW = size.Width + 18 + (int)drawableSeries.CanvasSchedule.Width;
            h += size.Height + 6;
            w = thisW > w ? thisW : w;
        }

        h += 6;

        ResumeLayout();
        return new SizeF(w, h);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }
}
