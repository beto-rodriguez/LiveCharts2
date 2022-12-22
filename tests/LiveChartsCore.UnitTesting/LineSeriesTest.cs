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
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class LineSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var sutSeries = new LineSeries<double>
        {
            Values = new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 },
            GeometrySize = 10
        };

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new[] { sutSeries },
            XAxes = new[] { new Axis { MinLimit = -1, MaxLimit = 10 } },
            YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 512 } }
        };

        _ = chart.GetImage();
        // chart.SaveImage("test.png"); // use this method to see the actual tested image

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var unit = points.First(x => x.PrimaryValue == 1);
        var typedUnit = sutSeries.ConvertToTypedChartPoint(unit);

        var toCompareGuys = points.Where(x => x != unit).Select(sutSeries.ConvertToTypedChartPoint);

        // ensure the unit has valid dimensions
        Assert.IsTrue(typedUnit.Visual.Geometry.Width == 10 && typedUnit.Visual.Geometry.Height == 10);

        var previous = typedUnit;
        float? previousX = null;
        float? previousXArea = null;

        foreach (var sutPoint in toCompareGuys)
        {
            // test x
            var currentDeltaX = previous.Visual.Geometry.X - sutPoint.Visual.Geometry.X;
            var currentDeltaAreaX = previous.Visual.Bezier.Xj - sutPoint.Visual.Bezier.Xj;
            Assert.IsTrue(
                previousX is null
                ||
                Math.Abs(previousX.Value - currentDeltaX) < 0.001);
            Assert.IsTrue(
                previousXArea is null
                ||
                Math.Abs(previousXArea.Value - currentDeltaX) < 0.001);

            // test y
            var p = 1f - sutPoint.PrimaryValue / 512f;
            Assert.IsTrue(
                Math.Abs(p * chart.Core.DrawMarginSize.Height - sutPoint.Visual.Geometry.Y + chart.Core.DrawMarginLocation.Y) < 0.001);
            Assert.IsTrue(
                Math.Abs(p * chart.Core.DrawMarginSize.Height - sutPoint.Visual.Bezier.Yj + chart.Core.DrawMarginLocation.Y) < 0.001);

            previousX = previous.Visual.Geometry.X - sutPoint.Visual.Geometry.X;
            previousXArea = previous.Visual.Bezier.Xj - sutPoint.Visual.Bezier.Xj;
            previous = sutPoint;
        }
    }
}
