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

using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IChartLegend{TDrawingContext}" />
public class SKDefaultLegend : IChartLegend<SkiaSharpDrawingContext>
{
    void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var series = chart.ChartSeries.Where(x => x.IsVisibleAtLegend);
        var legendOrientation = chart.LegendOrientation;
        var legendPosition = chart.LegendPosition;

        //var p = new SolidColorPaint(SKColors.AliceBlue) { IsFill = true };
        //p.AddGeometryToPaintTask(new RectangleGeometry() {  })
        //chart.Canvas.AddDrawableTask(p);

        var drawing = chart.Canvas.Draw()
            .SelectColor(SKColors.Blue)
            .DrawGeometry(new RectangleGeometry { X = 0, Y = 0, Width = 50, Height = 50 });
    }
}
