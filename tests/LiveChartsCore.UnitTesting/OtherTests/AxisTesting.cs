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

using System.Collections.ObjectModel;
using System;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using System.Diagnostics;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class AxisTesting
{
    [TestMethod]
    public void BasicCase()
    {
        var x = new Axis();
        var y = new Axis();

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 1, 2, 3 }
                }
            },
            XAxes = new[] { x },
            YAxes = new[] { y }
        };

        _ = chart.GetImage();

        // ensure the separators are created
        Assert.IsTrue(x.activeSeparators[chart.Core].Values.Count > 0);
        Assert.IsTrue(y.activeSeparators[chart.Core].Values.Count > 0);
    }

    [TestMethod]
    public void RejectUnnecessaryCases()
    {
        // The axis class throws when ~10,000 separators are drawn in the same axis (only in DEBUG mode).

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 1, 2, 3 }
                },
            },
            XAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 100000, ForceStepToMin = true, MinStep = 1, LabelsPaint = new SolidColorPaint(SKColors.Red) } }
        };

        bool succeed;
        try
        {
            _ = chart.GetImage();
            succeed = true;
        }
        catch
        {
            succeed = false;
        }

        Assert.IsTrue(!succeed);
    }

    [TestMethod]
    public void AutoLimits()
    {
        // based on https://github.com/beto-rodriguez/LiveCharts2/issues/755
        // when the range in the axis is zero, there is a change that the for instruction that
        // draws the separators never ends, this is because the step is calculated as 0
        // this test ensures the step is never 0.

        var x = new Axis { MinLimit = 10, MaxLimit = 10, LabelsPaint = new SolidColorPaint(SKColors.Red) };

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 1, 2, 3 }
                }
            },
            XAxes = new[] { x }
        };

        var sw = new Stopwatch();
        sw.Start();

        _ = chart.GetImage();
        Assert.IsTrue(x.activeSeparators[chart.CoreChart].Values.Count > 0 && sw.ElapsedMilliseconds < 1000);
    }

    [TestMethod]
    public void DateTimeWithInvalidUnitWidth()
    {
        // based on https://github.com/beto-rodriguez/LiveCharts2/issues/763

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new ISeries[]
            {
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(2021, 1, 1), 3)
                    }
                }
            }
        };

        var sw = new Stopwatch();
        sw.Start();
        _ = chart.GetImage();
        Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
    }

    [TestMethod]
    public void DoublePrecisionPossibleError()
    {
        // I have seen cases (not able to reproduce yet) where the calculated step in the axis is smaller than the double type precision.
        // this results in an infinite loop when drawing the separators.
        // I am not completely sure if that issue is already fixed, also not able to reproduce it.

        var chart = new SKCartesianChart
        {
            Width = 1000,
            Height = 1000,
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 1, 2, 3 }
                }
            },
            XAxes = new[] { new Axis { MinLimit = 0, MaxLimit = 1e100, LabelsPaint = new SolidColorPaint(SKColors.Red) } }
        };

        var sw = new Stopwatch();
        sw.Start();
        _ = chart.GetImage();
        Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
    }
}
