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
using LiveChartsCore.Measure;
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

    [TestMethod]
    public void ShouldPlaceToolTipsCorrectly()
    {
        var sutSeries = new CandlesticksSeries<FinancialPointI>
        {
            Values = new FinancialPointI[]
            {
                new(1,0.75,0.25,0),
                new(2,1.75,1.25,1),
                new(3,2.75,2.25,2),
                new(4,3.75,3.25,3),
                new(5,4.75,4.25,4),
            },
            MaxBarWidth = 1000,
            YToolTipLabelFormatter = x => x.PrimaryValue.ToString(),
            DataPadding = new Drawing.LvcPoint(0, 0)
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
            Math.Abs(tp.Y - (150 - tp.Height - 1 / 5d * 300 * 0.5d)) < 0.1,
            "Tool tip on top failed");

        chart.TooltipPosition = TooltipPosition.Bottom;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
            Math.Abs(tp.Y - (150 + 1 / 5d * 300 * 0.5d)) < 0.1,
            "Tool tip on bottom failed");

        chart.TooltipPosition = TooltipPosition.Left;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - (150 - tp.Width - 1 / 5d * 300 * 0.5d)) < 0.1 &&
            Math.Abs(tp.Y + tp.Height * 0.5f - 150) < 0.1,
            "Tool tip on left failed");

        chart.TooltipPosition = TooltipPosition.Right;
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - 150 - 1 / 5d * 300 * 0.5d) < 0.1 &&
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
            Math.Abs(tp.Y - (150 - tp.Height - 1 / 5d * 300 * 0.5d)) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Top,
            "Tool tip on top failed [AUTO]");

        chart.Core._pointerPosition = new(300 * 4 / 5d - 10, 300 * 1 / 5d + 10);
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X + tp.Width * 0.5f - 1 / 5d * 300 - 150) < 0.1 &&
            Math.Abs(tp.Y - 300 * 1/5d) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Bottom,
            "Tool tip on bottom failed [AUTO]");

        chart.Core._pointerPosition = new(299, 150);
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - (300 - 300 * (1 / 5d) * 0.5 - tp.Width)) < 0.0001 &&
            Math.Abs(tp.Y - -tp.Height * 0.5f) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Left,
            "Tool tip on left failed [AUTO]");

        chart.Core._pointerPosition = new(1, 150);
        _ = chart.GetImage();
        Assert.IsTrue(
            Math.Abs(tp.X - 300 * (1 / 5d) * 0.5) < 0.0001 &&
            Math.Abs(tp.Y - (300 - tp.Height * 0.5f - 300 * (1 / 5d))) < 0.1 &&
            chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Right,
            "Tool tip on left failed [AUTO]");
    }
}
