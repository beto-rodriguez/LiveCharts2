using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.General.TemplatedTooltips;

public class CustomTooltip : FloatingForm, IChartTooltip<SkiaSharpDrawingContext>
{
    IEnumerable<ChartPoint> tooltipPoints;
    Chart<SkiaSharpDrawingContext> chart;

    public CustomTooltip()
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
        var container = new DynamicLayout() { BackgroundColor = Color.FromArgb(30, 30, 30), Padding = new Eto.Drawing.Padding(4) };

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
                TextColor = Color.FromArgb(250, 250, 250),
                Font = chart.TooltipFont,
            };

            _ = container.AddRow(marker, label);
        }

        Content = new Scrollable { Content = container }; // wrap inside a scrollable just to get a border !
    }
    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        Visible = false;
    }
}
