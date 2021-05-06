using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
    public partial class DefaultTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
    {
        private readonly Dictionary<ChartPoint, object> _activePoints = new();
        private const int CS_DROPSHADOW = 0x00020000;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
        /// </summary>
        public DefaultTooltip()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var wfChart = (Chart)chart.View;

            if (!tooltipPoints.Any())
            {
                foreach (var key in _activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    _ = _activePoints.Remove(key);
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
                _activePoints[tooltipPoint.Point] = o;
            }

            foreach (var key in _activePoints.Keys.ToArray())
            {
                if (_activePoints[key] == o) continue;
                key.RemoveFromHoverState();
                _ = _activePoints.Remove(key);
            }

            wfChart.CoreCanvas.Invalidate();
        }

        private SizeF DrawAndMesure(IEnumerable<TooltipPoint> tooltipPoints, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;

            using var g = CreateGraphics();
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

        void IChartTooltip<SkiaSharpDrawingContext>.Hide()
        {
            Location = new Point(10000, 10000);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        /// <returns></returns>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the create parameters.
        /// </summary>
        /// <value>
        /// The create parameters.
        /// </value>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
    }
}
