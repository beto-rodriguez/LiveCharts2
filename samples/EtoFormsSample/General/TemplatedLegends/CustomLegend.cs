using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto.Forms;

namespace EtoFormsSample.General.TemplatedLegends;

public partial class CustomLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
{
    public CustomLegend()
    {
        InitializeComponent();
    }

    public LegendOrientation Orientation { get; set; }

    public void Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        var series = chart.ChartSeries;
        var legendOrientation = chart.LegendOrientation;

        Visible = true;
        if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Vertical;
        Dock = DockStyle.Right;

        DrawAndMesure(series, wfChart);

        BackColor = wfChart.LegendBackColor;
    }

    private void DrawAndMesure(IEnumerable<IChartSeries<SkiaSharpDrawingContext>> series, Chart chart)
    {
        SuspendLayout();
        Controls.Clear();

        var h = 0f;
        var w = 0f;

        var parent = new Panel();
        parent.BackColor = Color.FromArgb(255, 245, 245, 220);
        Controls.Add(parent);
        using var g = CreateGraphics();
        foreach (var s in series)
        {
            var size = g.MeasureString(s.Name, chart.LegendFont);

            var p = new Panel();
            p.Location = new Point(0, (int)h);
            parent.Controls.Add(p);

            p.Controls.Add(new MotionCanvas
            {
                Location = new Point(6, 0),
                PaintTasks = s.CanvasSchedule.PaintSchedules,
                Width = (int)s.CanvasSchedule.Width,
                Height = (int)s.CanvasSchedule.Height
            });
            p.Controls.Add(new Label
            {
                Text = s.Name,
                ForeColor = Color.Black,
                Font = chart.LegendFont,
                Location = new Point(6 + (int)s.CanvasSchedule.Width + 6, 0)
            });

            var thisW = size.Width + 36 + (int)s.CanvasSchedule.Width;
            p.Width = (int)thisW + 6;
            p.Height = (int)size.Height + 6;
            h += size.Height + 6;
            w = thisW > w ? thisW : w;
        }
        h += 6;
        parent.Height = (int)h;

        Width = (int)w;
        parent.Location = new Point(0, (int)(Height / 2 - h / 2));

        ResumeLayout();
    }
}
