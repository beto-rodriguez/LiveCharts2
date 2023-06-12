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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.SeriesTests;

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
                new ObservablePolarPoint(360 * 1 / 10d, 1),
                new ObservablePolarPoint(360 * 2 / 10d, 2),
                new ObservablePolarPoint(360 * 3 / 10d, 3),
                new ObservablePolarPoint(360 * 4 / 10d, 4),
                new ObservablePolarPoint(360 * 5 / 10d, 5),
                new ObservablePolarPoint(360 * 6 / 10d, 6),
                new ObservablePolarPoint(360 * 7 / 10d, 7),
                new ObservablePolarPoint(360 * 8 / 10d, 8),
                new ObservablePolarPoint(360 * 9 / 10d, 9),
                new ObservablePolarPoint(360 * 10 / 10d, 10)
            },
            GeometrySize = 0,
            IsClosed = false,
            Fill = null
        };

        var chart = new SKPolarChart
        {
            Width = 300,
            Height = 300,
            Series = new[] { sutSeries },
            AngleAxes = new[] { new PolarAxis { MinLimit = 0, MaxLimit = 360, IsVisible = false } },
            RadiusAxes = new[] { new PolarAxis { MinLimit = 0, MaxLimit = 10, IsVisible = false } }
        };

        _ = chart.GetImage();
        //chart.SaveImage("test.png"); // use this method to see the actual tested image

        var datafactory = sutSeries.DataFactory;
        var points = datafactory.Fetch(sutSeries, chart.Core).ToArray();

        var unit = points.First();
        var typedUnit = sutSeries.ConvertToTypedChartPoint(unit);
        var tuv = typedUnit.Visual;

        var toCompareGuys = points.Where(x => x != unit).Select(sutSeries.ConvertToTypedChartPoint);

        var uHyp = Math.Sqrt(Math.Pow(tuv.X - 150, 2) + Math.Pow(tuv.Y - 150, 2));

        foreach (var sutPoint in toCompareGuys)
        {
            var sutHyp = Math.Sqrt(Math.Pow(sutPoint.Visual.X - 150, 2) + Math.Pow(sutPoint.Visual.Y - 150, 2));
            Assert.IsTrue(Math.Abs(uHyp * (1 + sutPoint.Context.Index) - sutHyp) < 0.0001);
        }
    }

    [TestMethod]
    public void ShouldPlaceToolTipsCorrectly()
    {
        var sutSeries = new PolarLineSeries<double>
        {
            GeometrySize = 10,
            Values = new double[] { 1, 2, 3, 4, 5 },
            DataPadding = new LvcPoint(0, 0)
        };

        var tooltip = new SKDefaultTooltip();

        var chart = new SKPolarChart
        {
            Width = 300,
            Height = 300,
            Tooltip = tooltip,
            TooltipPosition = TooltipPosition.Top,
            Series = new[] { sutSeries },
            AngleAxes = new[] { new PolarAxis { IsVisible = false } },
            RadiusAxes = new[] { new PolarAxis { IsVisible = true, MinLimit = 1, MaxLimit = 5 } }
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

        // ToDo? test auto pos?

        //chart.TooltipPosition = TooltipPosition.Auto;
        //_ = chart.GetImage();
        //Assert.IsTrue(
        //    Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
        //    Math.Abs(tp.Y - (150 - tp.Height)) < 0.1 &&
        //    chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Top,
        //    "Tool tip on top failed [AUTO]");

        //sutSeries.Values = new double[] { -1, -2, -3, -4, -5 };
        //_ = chart.GetImage();
        //Assert.IsTrue(
        //    Math.Abs(tp.X + tp.Width * 0.5f - 150) < 0.1 &&
        //    Math.Abs(tp.Y - 150) < 0.1 &&
        //    chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Bottom,
        //    "Tool tip on bottom failed [AUTO]");

        //sutSeries.Values = new double[] { 1, 2, 3, 4, 5 };
        //chart.Core._pointerPosition = new(299, 150);
        //_ = chart.GetImage();
        //Assert.IsTrue(
        //    // that 2... it seems that the lineseries.DataPadding takes more space than expected
        //    Math.Abs(tp.X - (300 - tp.Width)) < 2 &&
        //    Math.Abs(tp.Y - -tp.Height * 0.5f) < 2 &&
        //    chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Left,
        //    "Tool tip on left failed [AUTO]");

        //chart.Core._pointerPosition = new(1, 150);
        //_ = chart.GetImage();
        //Assert.IsTrue(
        //    Math.Abs(tp.X) < 2 &&
        //    Math.Abs(tp.Y - (300 - tp.Height * 0.5f)) < 2 &&
        //    chart.Core.AutoToolTipsInfo.ToolTipPlacement == PopUpPlacement.Right,
        //    "Tool tip on left failed [AUTO]");
    }
}
