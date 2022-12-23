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
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class HeatSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var sutSeries = new HeatSeries<WeightedPoint>
        {
            Values = new WeightedPoint[]
            {
                new(0, 0, 0),
                new(0, 1, 1),
                new(0, 2, 2),
                new(0, 3, 3),
                new(0, 4, 4),
                new(0, 5, 5),
                new(0, 5, 6),
                new(0, 5, 7),
                new(0, 5, 8),
                new(0, 5, 9),
                new(0, 5, 10),

                new(1, 0, 0),
                new(1, 1, 1),
                new(1, 2, 2),
                new(1, 3, 3),
                new(1, 4, 4),
                new(1, 5, 5),
                new(1, 5, 6),
                new(1, 5, 7),
                new(1, 5, 8),
                new(1, 5, 9),
                new(1, 5, 10)
            },
            HeatMap = new[]
            {
                new LvcColor(0, 0, 0, 0), // the first element is the "coldest"
                new LvcColor(255, 255, 255, 255) // the last element is the "hottest"
            }
        };

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new[] { sutSeries },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() }
        };

        _ = chart.GetImage();
        // chart.SaveImage("test.png"); // use this method to see the actual tested image

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var unit = points.First(x => x.PrimaryValue == 1);
        var typedUnit = sutSeries.ConvertToTypedChartPoint(unit);

        var toCompareGuys = points.Where(x => x != unit).Select(sutSeries.ConvertToTypedChartPoint);

        // ensure the unit has valid dimensions
        Assert.IsTrue(typedUnit.Visual.Width > 1 && typedUnit.Visual.Height > 1);

        var previous = typedUnit;
        float? previousX = null;

        foreach (var sutPoint in toCompareGuys)
        {
            // test height
            Assert.IsTrue(
                Math.Abs(typedUnit.Visual.Height - sutPoint.Visual.Height) < 0.001);

            // test width
            Assert.IsTrue(
                Math.Abs(typedUnit.Visual.Width - sutPoint.Visual.Width) < 0.001);

            // test gradient
            var p = (byte)(255 * sutPoint.Model.Weight / 10f);
            Assert.IsTrue(
                sutPoint.Visual.Color.R == p &&
                sutPoint.Visual.Color.G == p &&
                sutPoint.Visual.Color.B == p &&
                sutPoint.Visual.Color.A == p);

            previousX = previous.Visual.X - sutPoint.Visual.X;
            previous = sutPoint;
        }
    }
}
