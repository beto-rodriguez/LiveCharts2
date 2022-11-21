using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.General.TemplatedTooltips;

public partial class CustomTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
{
    private readonly Font _tooltipFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);

    public CustomTooltip()
    {
        InitializeComponent();
    }

    public void Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        var size = DrawAndMesure(tooltipPoints);
        var location = tooltipPoints.GetTooltipLocation(new LvcSize(size.Width, size.Height), chart);

        BackColor = Color.FromArgb(255, 30, 30, 30);
        Height = (int)size.Height;
        Width = (int)size.Width;

        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + (int)location.X;
        var y = l.Y + (int)location.Y;
        Location = new Point(x, y);
        Show();

        wfChart.CoreCanvas.Invalidate();
    }

    private SizeF DrawAndMesure(IEnumerable<ChartPoint> tooltipPoints)
    {
        SuspendLayout();
        Controls.Clear();

        var h = 0f;
        var w = 0f;
        foreach (var point in tooltipPoints)
        {
            using var g = CreateGraphics();
            var text = point.AsTooltipString;
            var size = g.MeasureString(text, _tooltipFont);

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
                Font = _tooltipFont,
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
