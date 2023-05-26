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
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.SeriesTests;

[TestClass]
public class ScatterSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var values = new WeightedPoint[]
        {
            new(0, 1, 0),
            new(1, 2, 1),
            new(2, 4, 2),
            new(3, 8, 3),
            new(4, 16, 4),
            new(5, 32, 5),
            new(6, 64, 6),
            new(7, 128, 7),
            new(8, 256, 8),
            new(9, 512, 9)
        };

        var maxW = values.Max(x => x.Weight)!.Value;

        var sutSeries = new ScatterSeries<WeightedPoint>
        {
            Values = values,
            MinGeometrySize = 10,
            GeometrySize = 100
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
        Assert.IsTrue(typedUnit.Visual.Width > 1 && typedUnit.Visual.Height > 1);
        Assert.IsTrue(typedUnit.Visual.Width == sutSeries.MinGeometrySize && typedUnit.Visual.Height == sutSeries.MinGeometrySize);

        var previous = typedUnit;
        float? previousX = null;

        foreach (var sutPoint in toCompareGuys)
        {
            var w = sutPoint.Model.Weight!.Value / maxW;
            var targetSize = sutSeries.MinGeometrySize + (sutSeries.GeometrySize - sutSeries.MinGeometrySize) * w;

            // test height
            Assert.IsTrue(Math.Abs(sutPoint.Visual.Height - targetSize) < 0.001);

            // test width
            Assert.IsTrue(Math.Abs(sutPoint.Visual.Width - targetSize) < 0.001);

            // test x
            var currentDeltaX = previous.Visual.X - sutPoint.Visual.X;
            Assert.IsTrue(
                previousX is null
                ||
                Math.Abs(previousX.Value - currentDeltaX) < 0.001);

            // test y
            var p = 1f - sutPoint.PrimaryValue / 512f;
            Assert.IsTrue(
                Math.Abs(
                    p * chart.Core.DrawMarginSize.Height - sutPoint.Visual.Y -
                    sutPoint.Visual.Height * 0.5f + chart.Core.DrawMarginLocation.Y) < 0.001);

            previousX = previous.Visual.X - sutPoint.Visual.X;
            previous = sutPoint;
        }
    }
}
