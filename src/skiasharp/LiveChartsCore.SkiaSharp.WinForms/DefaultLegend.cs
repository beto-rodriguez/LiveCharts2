using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IChartLegend{TDrawingContext}" />
    public partial class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
        /// </summary>
        public DefaultLegend()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public LegendOrientation Orientation { get; set; }

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var wfChart = (Chart)chart.View;

            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;

            switch (legendPosition)
            {
                case LegendPosition.Hidden:
                    Visible = false;
                    break;
                case LegendPosition.Top:
                    Visible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Horizontal;
                    Dock = DockStyle.Top;
                    break;
                case LegendPosition.Left:
                    Visible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Vertical;
                    Dock = DockStyle.Left;
                    break;
                case LegendPosition.Right:
                    Visible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Vertical;
                    Dock = DockStyle.Right;
                    break;
                case LegendPosition.Bottom:
                    Visible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Horizontal;
                    Dock = DockStyle.Bottom;
                    break;
                default:
                    break;
            }

            DrawAndMesure(series, wfChart);

            BackColor = wfChart.LegendBackColor;
        }

        private void DrawAndMesure(IEnumerable<IDrawableSeries<SkiaSharpDrawingContext>> series, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;

            try
            {
                if (Orientation == LegendOrientation.Vertical)
                {
                    var parent = new Panel();
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
                }
                else
                {
                    var parent = new Panel();
                    Controls.Add(parent);
                    using var g = CreateGraphics();
                    foreach (var s in series)
                    {
                        var size = g.MeasureString(s.Name, chart.LegendFont);

                        var p = new Panel();
                        p.Location = new Point((int)w, 0);
                        parent.Controls.Add(p);

                        p.Controls.Add(new MotionCanvas
                        {
                            Location = new Point(0, 6),
                            //PaintTasks = s.DefaultPaintContext.PaintTasks,
                            Width = (int)s.CanvasSchedule.Width,
                            Height = (int)s.CanvasSchedule.Height
                        });
                        p.Controls.Add(new Label
                        {
                            Text = s.Name,
                            Font = chart.LegendFont,
                            Location = new Point(6 + (int)s.CanvasSchedule.Width + 6, 6)
                        });

                        var thisW = size.Width + 36 + (int)s.CanvasSchedule.Width;
                        p.Width = (int)thisW;
                        p.Height = (int)size.Height + 6 + 6;
                        h = size.Height + 6;
                        w += thisW;
                    }
                    h += 6;
                    parent.Height = (int)h;
                    parent.Width = (int)w;

                    Height = (int)h;

                    parent.Location = new Point((int)(Width / 2 - w / 2), 0);
                }
            }
            catch { }

            ResumeLayout();
        }
    }
}
