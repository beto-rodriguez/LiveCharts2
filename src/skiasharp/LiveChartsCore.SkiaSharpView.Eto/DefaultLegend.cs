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
using Eto.Drawing;
using Eto.Forms;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IChartLegend{TDrawingContext}" />
public class DefaultLegend : DynamicLayout, IChartLegend<SkiaSharpDrawingContext>
{
    private readonly Font _legendFont = Fonts.Sans(11);
    private readonly Color _legendTextColor = Color.FromArgb(255, 35, 35, 35);

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
    /// </summary>
    public DefaultLegend()
    {
        Padding = 4;
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
        Orientation = chart.LegendPosition is LegendPosition.Top or LegendPosition.Bottom
            ? LegendOrientation.Horizontal
            : LegendOrientation.Vertical;

        BackgroundColor = new Color(255, 255, 255);
        DrawAndMeasure(chart.ChartSeries);
    }

    private void DrawAndMeasure(IEnumerable<IChartSeries<SkiaSharpDrawingContext>> series)
    {
        Clear();

        foreach (var s in series)
        {
            if (!s.IsVisibleAtLegend) continue;

            var marker = new MotionCanvas
            {
                PaintTasks = s.CanvasSchedule.PaintSchedules,
                Width = (int)s.CanvasSchedule.Width,
                Height = (int)s.CanvasSchedule.Height
            };
            var label = new Label
            {
                Text = s.Name,
                Font = _legendFont,
                TextColor = _legendTextColor,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _ = AddRow(marker, label);
        }
        _ = AddRow(); // workaround ! empty row else last label renders incorrectly

        Create();
    }
}
