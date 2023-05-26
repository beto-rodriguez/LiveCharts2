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

namespace LiveChartsCore.UnitTesting.SeriesTests;

[TestClass]
public class PieSeriesTest
{
    [TestMethod]
    public void ShouldScaleProperly()
    {
        var values = new double[] { 100, 50, 25, 25 };
        var total = values.Sum();
        var seriesCollection = values.AsLiveChartsPieSeries((value, series) =>
        {
            series.HoverPushout = 50;
        });

        var chart = new SKPieChart
        {
            Width = 1000,
            Height = 1000,
            Series = seriesCollection,
        };

        _ = chart.GetImage();
        // chart.SaveImage("test.png"); // use this method to see the actual tested image

        var toCompareGuys = seriesCollection.Select(sutSeries =>
        {
            var datafactory = sutSeries.DataFactory;
            var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

            var unit = points.First();
            return sutSeries.ConvertToTypedChartPoint(unit);
        });

        var startAngle = 0f;
        foreach (var sutPoint in toCompareGuys)
        {
            var series = (PieSeries<double>)sutPoint.Context.Series;

            // test the angle of the slice
            Assert.IsTrue(
                Math.Abs(sutPoint.Visual.SweepAngle / 360f - sutPoint.Model / total) < 0.001);

            // test the start rotation
            Assert.IsTrue(
                Math.Abs(sutPoint.Visual.StartAngle - startAngle) < 0.001);

            // test the width and height
            Assert.IsTrue(Math.Abs(sutPoint.Visual.Width - (chart.Width - series.HoverPushout * 2)) < 0.001);
            Assert.IsTrue(Math.Abs(sutPoint.Visual.Height - (chart.Height - series.HoverPushout * 2)) < 0.001);

            // test the center
            Assert.IsTrue(Math.Abs(sutPoint.Visual.CenterX - chart.Width * 0.5f) < 0.001);
            Assert.IsTrue(Math.Abs(sutPoint.Visual.CenterY - chart.Width * 0.5f) < 0.001);

            startAngle += sutPoint.Visual.SweepAngle;
        }
    }
}
