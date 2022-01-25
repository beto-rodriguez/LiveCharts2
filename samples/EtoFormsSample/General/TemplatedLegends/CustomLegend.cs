using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto.Forms;

namespace EtoFormsSample.General.TemplatedLegends;

public class CustomLegend : DynamicLayout, IChartLegend<SkiaSharpDrawingContext>
{
    public CustomLegend()
    {
        Padding = 4;
    }

    public LegendOrientation Orientation { get; set; }

    public void Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        if (chart.LegendOrientation == LegendOrientation.Auto)
            Orientation = LegendOrientation.Vertical;

        wfChart.LegendPosition = LegendPosition.Right;

        BackgroundColor = wfChart.LegendBackColor;
        DrawAndMeasure(chart.ChartSeries, wfChart);
    }

    private void DrawAndMeasure(IEnumerable<IChartSeries<SkiaSharpDrawingContext>> series, Chart chart)
    {
        Clear();

        BackgroundColor = Color.FromArgb(245, 245, 220);

        foreach (var s in series)
        {
            var marker = new MotionCanvas
            {
                PaintTasks = s.CanvasSchedule.PaintSchedules,
                Width = (int)s.CanvasSchedule.Width,
                Height = (int)s.CanvasSchedule.Height
            };
            var label = new Label
            {
                Text = s.Name,
                TextColor = Colors.Black,
                Font = chart.LegendFont,
            };

            _ = AddRow(marker, label);
        }

        Create();
    }
}
