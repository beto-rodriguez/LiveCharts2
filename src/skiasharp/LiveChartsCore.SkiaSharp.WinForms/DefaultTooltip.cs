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

/* Unmerged change from project 'LiveChartsCore.SkiaSharpView.WinForms (netcoreapp3.1)'
Before:
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
After:
using LiveChartsCore.Collections.Generic;
using System.Drawing;
using LiveChartsCore.Linq;
*/
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
    public partial class DefaultTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
    {
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

            var size = DrawAndMesure(tooltipPoints, wfChart);
            LvcPoint? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height));
            }
            if (location is null) throw new Exception("location not supported");

            BackColor = wfChart.TooltipBackColor;
            Height = (int)size.Height;
            Width = (int)size.Width;

            var l = wfChart.PointToScreen(Point.Empty);
            var x = l.X + (int)location.Value.X;
            var y = l.Y + (int)location.Value.Y;
            Location = new Point(x, y);
            Show();
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
                    Font = chart.TooltipFont,
                    ForeColor = chart.TooltipTextColor,
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
            if (disposing && (components is not null))
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
