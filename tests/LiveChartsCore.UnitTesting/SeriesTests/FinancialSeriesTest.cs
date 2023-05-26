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
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.SeriesTests;

[TestClass]
public class FinancialSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var down = new SolidColorPaint();
        var up = new SolidColorPaint();

        var sutSeries = new CandlesticksSeries<FinancialPoint>
        {
            Values = new FinancialPoint[]
            {
                new (new(2022, 1, 1), 1, 0.75, 0.25, 0),
                new (new(2022, 1, 2), 64, 32, 64, 32),
                new (new(2022, 1, 3), 512, 511.75, 511.25, 511),
            },
            DownFill = down,
            UpFill = up
        };

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new[] { sutSeries },
            XAxes = new[] { new Axis { UnitWidth = TimeSpan.FromDays(1).Ticks } },
            YAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 512 } }
        };

        _ = chart.GetImage();
        // chart.SaveImage("test.png"); // use this method to see the actual tested image

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var unit = points.First(x => x.PrimaryValue == 1);
        var typedUnit = sutSeries.ConvertToTypedChartPoint(unit);

        var toCompareGuys = points.Where(x => x != unit).Select(sutSeries.ConvertToTypedChartPoint);

        bool IsInCorrectPaint(ChartPoint<FinancialPoint, CandlestickGeometry, LabelGeometry> point)
        {
            var paint = point.Model.Open > point.Model.Close
                ? down
                : up;

            foreach (var geometry in paint.GetGeometries(chart.CoreCanvas))
                if (point.Visual == geometry) return true;

            return false;
        }

        // ensure the unit has valid dimensions
        var unitH = typedUnit.Visual.Low - typedUnit.Visual.Y;
        Assert.IsTrue(typedUnit.Visual.Width > 1 && unitH > 1);
        Assert.IsTrue(IsInCorrectPaint(typedUnit));

        var previous = typedUnit;
        float? previousX = null;

        foreach (var sutPoint in toCompareGuys)
        {
            // test paint
            Assert.IsTrue(IsInCorrectPaint(sutPoint));

            // test height
            var h = sutPoint.Visual.Low - sutPoint.Visual.Y;
            var sutH = sutPoint.Visual.Low - sutPoint.Visual.Y;
            var sutDH = sutPoint.Model.High - sutPoint.Model.Low;
            Assert.IsTrue(Math.Abs(unitH - sutH / (float)sutDH) < 0.001);

            // test width
            Assert.IsTrue(Math.Abs(typedUnit.Visual.Width - sutPoint.Visual.Width) < 0.001);

            // test x
            var currentDeltaX = previous.Visual.X - sutPoint.Visual.X;
            Assert.IsTrue(
                previousX is null
                ||
                Math.Abs(previousX.Value - currentDeltaX) < 0.001);

            // test y
            var p = 1f - sutPoint.PrimaryValue / 512f;
            Assert.IsTrue(
                Math.Abs(p * chart.Core.DrawMarginSize.Height - sutPoint.Visual.Y + chart.Core.DrawMarginLocation.Y) < 0.001);

            previousX = previous.Visual.X - sutPoint.Visual.X;
            previous = sutPoint;
        }
    }
}
