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
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.SeriesTests;

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

    [TestMethod]
    public void ShouldPlaceToolTipsCorrectly()
    {
        var sutSeries = new HeatSeries<WeightedPoint>
        {
            HeatMap = new[]
            {
                new SKColor(255, 241, 118).AsLvcColor(),
                SKColors.DarkSlateGray.AsLvcColor(),
                SKColors.Blue.AsLvcColor()
            },
            Values = new ObservableCollection<WeightedPoint>
            {
                // Charles
                new(0, 0, 150), // Jan
                new(0, 1, 123), // Feb
                new(0, 2, 310), // Mar
                new(0, 3, 225), // Apr
                new(0, 4, 473), // May

                // Richard
                new(1, 0, 432), // Jan
                new(1, 1, 312), // Feb
                new(1, 2, 135), // Mar
                new(1, 3, 78), // Apr
                new(1, 4, 124), // May

                // Ana
                new(2, 0, 543), // Jan
                new(2, 1, 134), // Feb
                new(2, 2, 524), // Mar
                new(2, 3, 315), // Apr
                new(2, 4, 145), // May

                // Mari
                new(3, 0, 90), // Jan
                new(3, 1, 123), // Feb
                new(3, 2, 70), // Mar
                new(3, 3, 123), // Apr
                new(3, 4, 432), // May

                // Mari
                new(4, 0, 90), // Jan
                new(4, 1, 123), // Feb
                new(4, 2, 70), // Mar
                new(4, 3, 123), // Apr
                new(4, 4, 432), // May
            },
        };

        var tooltip = new SKDefaultTooltip();

        var chart = new SKCartesianChart
        {
            Width = 300,
            Height = 300,
            Tooltip = tooltip,
            TooltipPosition = TooltipPosition.Top,
            Series = new[] { sutSeries },
            XAxes = new[] { new Axis { IsVisible = false } },
            YAxes = new[] { new Axis { IsVisible = false } }
        };

        chart.Core._isPointerIn = true;
        chart.Core._isToolTipOpen = true;
        chart.Core._pointerPosition = new(150, 150);

        chart.TooltipPosition = TooltipPosition.Top;
        _ = chart.GetImage();
        var tp = tooltip._panel.BackgroundGeometry;
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
            Math.Abs(tp.Y - (150 - tp.Height)) < 0.1,
            "Tool tip on top failed");

        chart.TooltipPosition = TooltipPosition.Bottom;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
            Math.Abs(tp.Y - 150) < 0.1,
            "Tool tip on bottom failed");

        chart.TooltipPosition = TooltipPosition.Left;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - (150 - tp.Width)) < 0.1 &&
            Math.Abs(tp.Y + tp.Height * 0.5f - 150) < 0.1,
            "Tool tip on left failed");

        chart.TooltipPosition = TooltipPosition.Right;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - 150) < 0.1 &&
            Math.Abs(tp.Y + tp.Height * 0.5f - 150) < 0.1,
            "Tool tip on right failed");

        chart.TooltipPosition = TooltipPosition.Center;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
            Math.Abs(tp.Y + tp.Height * 0.5f - 150) < 0.1,
            "Tool tip on center failed");

        chart.TooltipPosition = TooltipPosition.Auto;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
            Math.Abs(tp.Y - (150 - tp.Height)) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Top,
            "Tool tip on top failed [AUTO]");

        chart.Core._pointerPosition = new(150, 10);
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - (150 - tp.Width * 0.5f)) < 0.1 &&
            Math.Abs(tp.Y - 300 * 1 / 5d * 0.5f) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Bottom,
            "Tool tip on bottom failed [AUTO]");

        chart.Core._pointerPosition = new(295, 5);
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - (300 - 300 * 1 / 5d * 0.5 - tp.Width)) < 0.1 &&
            Math.Abs(tp.Y - -(tp.Height * 0.5f - 300 * 1 / 5d * 0.5)) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Left,
            "Tool tip on left failed [AUTO]");

        chart.Core._pointerPosition = new(5, 295);
        chart.SaveImage("HOLAAA.PNG");
        Assert.IsTrue(
            Math.Abs(tp.X) < 0.1 &&
            Math.Abs(tp.Y - (300 - tp.Height * 0.5f)) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Right,
            "Tool tip on left failed [AUTO]");
    }
}
