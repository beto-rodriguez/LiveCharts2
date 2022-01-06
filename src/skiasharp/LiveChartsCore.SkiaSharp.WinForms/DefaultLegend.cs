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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.WinForms;

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

        var series = chart.ChartSeries;
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

    private void DrawAndMesure(IEnumerable<IChartSeries<SkiaSharpDrawingContext>> series, Chart chart)
    {
        SuspendLayout();
        Controls.Clear();

        var h = 0f;
        var w = 0f;

        try
        {
            if (Orientation == LegendOrientation.Vertical)
            {
                var parent = new Panel { BackColor = chart.LegendBackColor };
                Controls.Add(parent);
                using var g = CreateGraphics();
                foreach (var s in series)
                {
                    var size = g.MeasureString(s.Name, chart.LegendFont);

                    var p = new Panel { Location = new Point(0, (int)h) };
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
                        ForeColor = chart.LegendTextColor,
                        Location = new Point(6 + (int)s.CanvasSchedule.Width + 6, 0),
                        AutoSize = true
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

                    var p = new Panel { Location = new Point((int)w, 0) };
                    parent.Controls.Add(p);

                    p.Controls.Add(new MotionCanvas
                    {
                        Location = new Point(0, 6),
                        PaintTasks = s.CanvasSchedule.PaintSchedules,
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
