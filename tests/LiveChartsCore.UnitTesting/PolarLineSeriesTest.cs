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
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class PolarLineSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var sutSeries = new PolarLineSeries<ObservablePolarPoint>
        {
            Values = new[]
            {
                new ObservablePolarPoint(0, 10),
                new ObservablePolarPoint(45, 15),
                new ObservablePolarPoint(90, 20),
                new ObservablePolarPoint(135, 25),
                new ObservablePolarPoint(180, 30),
                new ObservablePolarPoint(225, 35),
                new ObservablePolarPoint(270, 40),
                new ObservablePolarPoint(315, 45),
                new ObservablePolarPoint(360, 50),
            },
            GeometrySize = 10,
            IsClosed = false,
            Fill = null
        };

        var chart = new SKPolarChart
        {
            Width = 1000,
            Height = 1000,
            Series = new[] { sutSeries },
            AngleAxes = new[] { new PolarAxis { MinLimit = 0, MaxLimit = 360 } }
        };

        _ = chart.GetImage();
        // chart.SaveImage("test.png"); // use this method to see the actual tested image

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var unit = points.First();
        var typedUnit = sutSeries.ConvertToTypedChartPoint(unit);

        var toCompareGuys = points.Where(x => x != unit).Select(sutSeries.ConvertToTypedChartPoint);

        // ensure the unit has valid dimensions
        Assert.IsTrue(typedUnit.Visual.Geometry.Width == 10 && typedUnit.Visual.Geometry.Height == 10);

        foreach (var sutPoint in toCompareGuys)
        {
            // ToDo
        }
    }
}
