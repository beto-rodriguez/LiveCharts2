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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.WinForms;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public partial class DefaultTooltip : Form, IChartTooltip<SkiaSharpDrawingContext>, IDisposable
{
    private Panel? _tooltipContainer;
    private const int BorderThickness = 2;
    private readonly Font _tooltipFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);
    private readonly Color _tooltipTextColor = Color.FromArgb(255, 35, 35, 35);
    private readonly Color _tooltipBackColor = Color.FromArgb(255, 250, 250, 250);

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
    /// </summary>
    public DefaultTooltip()
    {
        InitializeComponent();
        ShowInTaskbar = false;
        Paint += DefaultTooltip_Paint;
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var wfChart = (Chart)chart.View;

        var size = DrawAndMesure(tooltipPoints);
        LvcPoint? location = null;

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height), chart.DrawMarginSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height));
        }
        if (location is null) throw new Exception("location not supported");
        Height = (int)size.Height;
        Width = (int)size.Width;
        if (_tooltipContainer is not null) _tooltipContainer.BackColor = _tooltipBackColor;

        var l = wfChart.PointToScreen(Point.Empty);
        var x = l.X + (int)location.Value.X;
        var y = l.Y + (int)location.Value.Y;

        var canvasLocation = wfChart.GetCanvasPosition();

        Location = new Point(x + canvasLocation.X, y + canvasLocation.Y);
        Show();
    }

    private SizeF DrawAndMesure(IEnumerable<ChartPoint> tooltipPoints)
    {
        SuspendLayout();
        Controls.Clear();

        var h = 0f;
        var w = 0f;

        using var g = CreateGraphics();

        var container = new Panel { Location = new Point(BorderThickness, BorderThickness) };
        _tooltipContainer = container;
        Controls.Add(container);

        foreach (var point in tooltipPoints)
        {
            var text = point.AsTooltipString;
            var size = g.MeasureString(text, _tooltipFont);

            var drawableSeries = (IChartSeries<SkiaSharpDrawingContext>)point.Context.Series;

            container.Controls.Add(new MotionCanvas
            {
                Location = new Point(8, (int)h + 6),
                PaintTasks = drawableSeries.CanvasSchedule.PaintSchedules,
                Width = (int)drawableSeries.CanvasSchedule.Width,
                Height = (int)drawableSeries.CanvasSchedule.Height
            });

            var dh = (int)((drawableSeries.CanvasSchedule.Height - size.Height) * 0.5f);
            if (dh < 0) dh = 0;

            container.Controls.Add(new Label
            {
                Text = text,
                Font = _tooltipFont,
                ForeColor = _tooltipTextColor,
                Location = new Point(6 + (int)drawableSeries.CanvasSchedule.Width + 6, (int)h + 6 + dh),
                AutoSize = true
            });

            var thisW = size.Width + 18 + (int)drawableSeries.CanvasSchedule.Width;
            h += (int)(drawableSeries.CanvasSchedule.Height > size.Height ? drawableSeries.CanvasSchedule.Height : size.Height) + 6;
            w = thisW > w ? thisW : w;
        }

        h += 12;
        container.Width = (int)w + 4;
        container.Height = (int)h;

        ResumeLayout();
        return new SizeF(container.Width + 2 * BorderThickness, container.Height + 2 * BorderThickness);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        var isCursorInTooltip =
            Cursor.Position.X >= Location.X && Cursor.Position.X <= Location.X + Width &&
            Cursor.Position.Y >= Location.Y && Cursor.Position.Y <= Location.Y + Height;

        if (isCursorInTooltip) return;

        Location = new Point(10000, 10000);
    }

    private void DefaultTooltip_Paint(object sender, PaintEventArgs e)
    {
        if (_tooltipContainer is null) return;
        using var brush = new SolidBrush(Color.FromArgb(220, 220, 220));
        e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
    }

    /// <summary>
    /// Paints the form background.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaintBackground(PaintEventArgs e) { /* Ignore */ }

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
}
