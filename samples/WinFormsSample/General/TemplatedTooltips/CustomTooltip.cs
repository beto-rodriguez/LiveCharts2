using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsSample.General.TemplatedTooltips
{
    public partial class CustomTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
    {
        private readonly Dictionary<ChartPoint, object> activePoints = new();

        public CustomTooltip()
        {
            InitializeComponent();
        }

        public void Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var wfChart = (Chart)chart.View;

            if (!tooltipPoints.Any())
            {
                foreach (var key in activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    _ = activePoints.Remove(key);
                }

                return;
            }

            if (activePoints.Count > 0 && tooltipPoints.All(x => activePoints.ContainsKey(x.Point))) return;

            var size = DrawAndMesure(tooltipPoints, wfChart);
            PointF? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new SizeF((float)size.Width, (float)size.Height), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new SizeF((float)size.Width, (float)size.Height));
            }

            BackColor = Color.FromArgb(255, 30, 30, 30);
            Height = (int)size.Height;
            Width = (int)size.Width;

            var l = wfChart.PointToScreen(Point.Empty);
            var x = l.X + (int)location.Value.X;
            var y = l.Y + (int)location.Value.Y;
            Location = new Point(x, y);
            Show();

            var o = new object();
            foreach (var tooltipPoint in tooltipPoints)
            {
                tooltipPoint.Point.AddToHoverState();
                activePoints[tooltipPoint.Point] = o;
            }

            foreach (var key in activePoints.Keys.ToArray())
            {
                if (activePoints[key] == o) continue;
                key.RemoveFromHoverState();
                _ = activePoints.Remove(key);
            }

            wfChart.CoreCanvas.Invalidate();
        }

        private SizeF DrawAndMesure(IEnumerable<TooltipPoint> tooltipPoints, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;
            foreach (var point in tooltipPoints)
            {
                using var g = CreateGraphics();
                var text = point.Point.AsTooltipString;
                var size = g.MeasureString(text, chart.TooltipFont);

                var drawableSeries = (IChartSeries<SkiaSharpDrawingContext>)point.Series;

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
}
