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
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class DataProviderTest
{
    [TestMethod]
    public void Fetch()
    {
        static void testType<T>(T[] values, Action<ChartPoint[]> test = null)
        {
            var sutSeries = new ColumnSeries<T>
            {
                Values = values
            };

            var chart = new SKCartesianChart
            {
                Width = 100,
                Height = 100,
                Series = new[] { sutSeries }
            };

            _ = chart.GetImage();

            var datafactory = sutSeries.DataFactory;
            var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

            if (test is not null)
            {
                test(points);
                return;
            }

            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var c = point.Coordinate;
                Assert.IsTrue(c.SecondaryValue == i && c.PrimaryValue == 1);
            }
        }

        testType(new short[] { 1, 1, 1, 1, 1 });
        testType(new int[] { 1, 1, 1, 1, 1 });
        testType(new long[] { 1, 1, 1, 1, 1 });
        testType(new float[] { 1, 1, 1, 1, 1 });
        testType(new double[] { 1, 1, 1, 1, 1 });
        testType(new decimal[] { 1, 1, 1, 1, 1 });
        testType(new short?[] { 1, 1, 1, 1, 1 });
        testType(new int?[] { 1, 1, 1, 1, 1 });
        testType(new long?[] { 1, 1, 1, 1, 1 });
        testType(new float?[] { 1, 1, 1, 1, 1 });
        testType(new double?[] { 1, 1, 1, 1, 1 });
        testType(new decimal?[] { 1, 1, 1, 1, 1 });

        testType(new ObservableValue[] { new(1), new(1), new(1), new(1), new(1) });
        testType(new ObservablePoint[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1), new(4, 1) });

        testType(
            new WeightedPoint[] { new(0, 1, 0), new(1, 1, 10), new(2, 1, 20), new(3, 1, 30), new(4, 1, 40) },
            points =>
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var c = point.Coordinate;
                    Assert.IsTrue(c.SecondaryValue == i && c.PrimaryValue == 1 && c.TertiaryValue == i * 10);
                }
            });

        var now = DateTime.Now;
        testType(
            new DateTimePoint[] { new(now.AddDays(0), 1), new(now.AddDays(1), 1), new(now.AddDays(2), 1) },
            points =>
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var c = point.Coordinate;
                    Assert.IsTrue(
                        c.SecondaryValue == now.AddDays(i).Ticks &&
                        c.PrimaryValue == 1);
                }
            });

        testType(
            new TimeSpanPoint[] {
                new(TimeSpan.FromMilliseconds(0), 1),
                new(TimeSpan.FromMilliseconds(1), 1),
                new(TimeSpan.FromMilliseconds(2), 1)
            },
            points =>
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var c = point.Coordinate;
                    Assert.IsTrue(
                        c.SecondaryValue == TimeSpan.FromMilliseconds(i).Ticks &&
                        c.PrimaryValue == 1);
                }
            });

        testType(new ObservablePolarPoint[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1), new(4, 1) });

        testType(
            new FinancialPoint[] {
                new(now.AddDays(0), 1, 2, 3, 4),
                new(now.AddDays(1), 1, 2, 3, 4),
                new(now.AddDays(2), 1, 2, 3, 4)
            },
            points =>
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var c = point.Coordinate;
                    Assert.IsTrue(
                        c.SecondaryValue == now.AddDays(i).Ticks &&
                        c.PrimaryValue == 1 && c.TertiaryValue == 2 &&
                        c.QuaternaryValue == 3 && c.QuinaryValue == 4);
                }
            });

        testType(
             new FinancialPoint[] {
                new(now.AddDays(0), 1, 2, 3, 4),
                new(now.AddDays(1), 1, 2, 3, 4),
                new(now.AddDays(2), 1, 2, 3, 4)
             },
             points =>
             {
                 for (var i = 0; i < points.Length; i++)
                 {
                     var point = points[i];
                     var c = point.Coordinate;
                     Assert.IsTrue(
                         c.SecondaryValue == now.AddDays(i).Ticks &&
                         c.PrimaryValue == 1 && c.TertiaryValue == 2 &&
                         c.QuaternaryValue == 3 && c.QuinaryValue == 4);
                 }
             });

        testType(
            new FinancialPointI[] {
                new(1, 2, 3, 4),
                new(1, 2, 3, 4),
                new(1, 2, 3, 4)
            },
            points =>
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var c = point.Coordinate;
                    Assert.IsTrue(
                        c.SecondaryValue == i &&
                        c.PrimaryValue == 1 && c.TertiaryValue == 2 &&
                        c.QuaternaryValue == 3 && c.QuinaryValue == 4);
                }
            });
    }

    [TestMethod]
    public void FetchCustomType()
    {
        // finally lets test a mapper
        var sutSeries = new ColumnSeries<City>
        {
            Values = new City[] { new(1), new(1), new(1), new(1), new(1) },
            Mapping = (city, index) => new(index, city.Population.Value)
        };

        var chart = new SKCartesianChart
        {
            Width = 100,
            Height = 100,
            Series = new[] { sutSeries }
        };

        _ = chart.GetImage();

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var c = point.Coordinate;
            Assert.IsTrue(c.SecondaryValue == i && c.PrimaryValue == 1);
        }
    }

    [TestMethod]
    public void FetchCoordinateEmpty()
    {
        // finally lets test a mapper
        var sutSeries = new ColumnSeries<City>
        {
            Values = new City[] { new(1), new(null), new(1), new(null), new(1) },
            Mapping = (city, index) =>
                city.Population is null
                    ? Coordinate.Empty
                    : new(index, city.Population.Value)
        };

        var chart = new SKCartesianChart
        {
            Width = 100,
            Height = 100,
            Series = new[] { sutSeries }
        };

        _ = chart.GetImage();

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var emptyCount = 0;
        for (var i = 0; i < points.Length; i++)
        {
            var point = sutSeries.ConvertToTypedChartPoint(points[i]);
            var c = point.Coordinate;

            if (c.IsEmpty)
            {
                Assert.IsTrue(point.Visual is null);
                emptyCount++;
            }
            else
            {
                Assert.IsTrue(c.SecondaryValue == i && c.PrimaryValue == 1);
            }
        }

        Assert.IsTrue(emptyCount == 2);
    }

    [TestMethod]
    public void FetchNull()
    {
        // finally lets test a mapper
        var sutSeries = new ColumnSeries<City>
        {
            Values = new City[] { new(1), null, new(1), null, new(1) },
            Mapping = (city, index) => new(index, city.Population.Value)
        };

        var chart = new SKCartesianChart
        {
            Width = 100,
            Height = 100,
            Series = new[] { sutSeries }
        };

        _ = chart.GetImage();

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var emptyCount = 0;
        for (var i = 0; i < points.Length; i++)
        {
            var point = sutSeries.ConvertToTypedChartPoint(points[i]);
            var c = point.Coordinate;

            if (point.Model is null)
            {
                Assert.IsTrue(point.Visual is null && c.IsEmpty);
                emptyCount++;
            }
            else
            {
                Assert.IsTrue(c.SecondaryValue == i && c.PrimaryValue == 1);
            }
        }

        Assert.IsTrue(emptyCount == 2);
    }

    public class City
    {
        public City(double? population)
        {
            Population = population;
        }

        public double? Population { get; set; }
    }
}
