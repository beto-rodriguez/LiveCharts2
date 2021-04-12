using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public partial class DefaultTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
    {
        private readonly Dictionary<ChartPoint, object> activePoints = new();
        private const int CS_DROPSHADOW = 0x00020000;

        public DefaultTooltip()
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
                    activePoints.Remove(key);
                }

                return;
            }

            var size = DrawAndMesure(tooltipPoints, wfChart);
            PointF? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new SizeF((float)size.Width, (float)size.Height));
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new SizeF((float)size.Width, (float)size.Height));
            }
            if (location == null) throw new Exception("location not supported");

            BackColor = wfChart.TooltipBackColor;
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
                activePoints.Remove(key);
            }

            wfChart.CoreCanvas.Invalidate();
        }

        private SizeF DrawAndMesure(IEnumerable<TooltipPoint> tooltipPoints, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;

            using Graphics g = CreateGraphics();
            foreach (var point in tooltipPoints)
            {
                var text = point.Point.AsTooltipString;
                var size = g.MeasureString(text, chart.TooltipFont);

                var drawableSeries = (IDrawableSeries<SkiaSharpDrawingContext>)point.Series;
                
                Controls.Add(new MotionCanvas
                {
                    Location = new Point(6, (int)h + 6),
                    PaintTasks = drawableSeries.DefaultPaintContext.PaintTasks,
                    Width = (int)drawableSeries.DefaultPaintContext.Width,
                    Height = (int)drawableSeries.DefaultPaintContext.Height
                });
                Controls.Add(new Label 
                { 
                    Text = text,
                    Font = chart.TooltipFont,
                    Location = new Point(6 + (int)drawableSeries.DefaultPaintContext.Width + 6, (int)h + 6)
                });

                var thisW = size.Width + 18 + (int)drawableSeries.DefaultPaintContext.Width;
                h += size.Height + 6;
                w = thisW > w ? thisW : w;
            }

            h += 6;

            ResumeLayout();
            return new SizeF(w, h);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
    }
}