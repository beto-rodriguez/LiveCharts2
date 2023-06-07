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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class EventsTests
{
    [TestMethod]
    public void CartesianBasicCase()
    {
        var chart = new SKCartesianChart
        {
            Width = 500,
            Height = 500,
            Series = new[]
            {
                new ScatterSeries<double>
                {
                    Values = new double[] { 0, 1, 2 },
                    DataPadding = new Drawing.LvcPoint(0, 0)
                }
            },
            XAxes = new[] { new Axis { IsVisible = false, } },
            YAxes = new[] { new Axis { IsVisible = false, } },
            VisualElements = new VisualElement<SkiaSharpDrawingContext>[]
            {
                new LabelVisual
                {
                    X = 1,
                    Y = 1,
                    Text = "Hello world",
                    TextSize = 20,
                    Paint = new SolidColorPaint(SKColors.Red),
                    LocationUnit = Measure.MeasureUnit.ChartValues
                },
                new GeometryVisual<RectangleGeometry>
                {
                    X = 1,
                    Y = 1,
                    Width = 0.5,
                    Height = 0.5,
                    Fill = new SolidColorPaint(SKColors.Blue),
                    LocationUnit = Measure.MeasureUnit.ChartValues,
                    SizeUnit = Measure.MeasureUnit.ChartValues
                }
            }
        };

        _ = chart.GetImage();

        // Test points.
        // Charts use the Series.FindHitPoints method to check if the mouse is over a point.
        var strategy = chart.Series.GetTooltipFindingStrategy();
        var s = chart.Series
            .SelectMany(x => x.FindHitPoints(chart.Core, new LvcPoint(251, 251), strategy))
            .ToArray();
        Assert.IsTrue(s.Length == 1);

        // Test visual elements.
        // Charts use the VisualElement.IsHitBy method to check if the mouse is over a visual element.
        var v = chart.VisualElements
            .Cast<VisualElement<SkiaSharpDrawingContext>>()
            .SelectMany(x => x.IsHitBy(chart.Core, new LvcPoint(251, 251)))
            .ToArray();
        Assert.IsTrue(v.Length == 2);
    }
}
