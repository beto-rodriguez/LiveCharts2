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
using Eto.Forms;
using Eto.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
public class DefaultTooltip : FloatingForm, IChartTooltip<SkiaSharpDrawingContext>
{
    private IEnumerable<ChartPoint>? _tooltipPoints;
    private Chart<SkiaSharpDrawingContext>? _chart;
    private readonly Font _tooltipFont = Fonts.Sans(11);
    private readonly Color _tooltipTextColor = Color.FromArgb(255, 35, 35, 35);
    private readonly Color _tooltipBackColor = Color.FromArgb(255, 250, 250, 250);

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
    /// </summary>
    public DefaultTooltip()
    {
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
        Resizable = false;
        AutoSize = true;
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _chart = chart;
        _tooltipPoints = tooltipPoints;
        DrawAndMeasure(tooltipPoints);
        SetLocation();
        Show();
    }

    /// <summary>
    /// Called when the size changes.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        SetLocation();
    }

    private void SetLocation()
    {
        if (Height < 1 || Width < 1) return;
        if (_chart is null || _tooltipPoints is null) return;

        var location = _tooltipPoints.GetTooltipLocation(new LvcSize(Width, Height), _chart);
        //var wfChart = (Chart)_chart.View;
        //var canvasPosition = wfChart.GetCanvasPosition();

        Location = new Point((int)location.X, (int)location.Y);
    }

    private void DrawAndMeasure(IEnumerable<ChartPoint> tooltipPoints)
    {
        var container = new DynamicLayout()
        {
            BackgroundColor = _tooltipBackColor,
            Padding = new global::Eto.Drawing.Padding(4)
        };

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
                Font = _tooltipFont,
                TextColor = _tooltipTextColor,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _ = container.AddRow(marker, label);
        }
        _ = container.AddRow(); // workaround ! empty row else last label renders incorrectly

        Content = new Scrollable { Content = container }; // wrap inside a scrollable just to get a border !
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        Visible = false;
    }
}
