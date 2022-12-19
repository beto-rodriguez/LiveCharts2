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

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class AxisTesting
{
    // The axis class throws when ~10,000 separators are drawn in the same axis (only in DEBUG mode).

    [TestMethod]
    public void BasicCase()
    {
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
            }
        };

        _ = chart.GetImage();
    }

    [TestMethod]
    public void RejectUnesessaryCases()
    {
        // the code always throws 

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

        bool succeded;
        try
        {
            _ = chart.GetImage();
            succeded = true;
        }
        catch
        {
            succeded = false;
        }

        Assert.IsTrue(!succeded);
    }

    [TestMethod]
    public void AutoLimits()
    {
        // based on https://github.com/beto-rodriguez/LiveCharts2/issues/755

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
            XAxes = new[] { new Axis { MinLimit = 10, MaxLimit = 10, LabelsPaint = new SolidColorPaint(SKColors.Red) } }
        };

        _ = chart.GetImage();
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

        _ = chart.GetImage();
    }

    [TestMethod]
    public void DoublePresicionPosibleError()
    {
        // I have seen cases (not able to reproduce yet) where the calculated step in the axis is lower than the double type precision.
        // this results in an infinite loop.
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

        _ = chart.GetImage();
    }
}
