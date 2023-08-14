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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class DrawMarginAndLegendsTests
{
    [TestMethod]
    public void CartesianBasicCase()
    {
        var legend = new SKDefaultLegend();

        var chart = new SKCartesianChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { new LineSeries<double> { Values = new double[] { 1, 2, 3 } } },
            XAxes = new[] { new Axis { IsVisible = false, } },
            YAxes = new[] { new Axis { IsVisible = false, } },
            DrawMarginFrame = new DrawMarginFrame() { Stroke = new SolidColorPaint(SKColors.Yellow, 5) },
            LegendPosition = LegendPosition.Top,
            Legend = legend
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 0
        Assert.IsTrue(Math.Abs(y - 0) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }

    [TestMethod]
    public void CartesianWithTitleCase()
    {
        var legend = new SKDefaultLegend();
        legend._stackPanel.BackgroundPaint = new SolidColorPaint(SKColors.LightGray);

        var title = new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new Padding(10),
            Paint = new SolidColorPaint(SKColors.White),
            BackgroundColor = SKColors.Purple.AsLvcColor()
        };

        var chart = new SKCartesianChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { new LineSeries<double> { Values = new double[] { 1, 2, 3 } } },
            XAxes = new[] { new Axis { IsVisible = false, } },
            YAxes = new[] { new Axis { IsVisible = false, } },
            DrawMarginFrame = new DrawMarginFrame() { Stroke = new SolidColorPaint(SKColors.Yellow, 5) },
            LegendPosition = LegendPosition.Top,
            Title = title,
            Legend = legend
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var titleSize = title.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be titleSize.Height
        Assert.IsTrue(Math.Abs(y - titleSize.Height) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - titleSize.Height) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - titleSize.Height) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }

    [TestMethod]
    public void PieBasicCase()
    {
        var legend = new SKDefaultLegend();

        var chart = new SKPieChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { 1, 2, 3 }.AsPieSeries(),
            LegendPosition = LegendPosition.Top,
            Legend = legend
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 0
        Assert.IsTrue(Math.Abs(y - 0) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }

    [TestMethod]
    public void PieWithTitleCase()
    {
        var legend = new SKDefaultLegend();
        legend._stackPanel.BackgroundPaint = new SolidColorPaint(SKColors.LightGray);

        var title = new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new Padding(10),
            Paint = new SolidColorPaint(SKColors.White),
            BackgroundColor = SKColors.Purple.AsLvcColor()
        };

        var chart = new SKPieChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { 1, 2, 3 }.AsPieSeries(),
            LegendPosition = LegendPosition.Top,
            Title = title,
            Legend = legend
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var titleSize = title.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be titleSize.Height
        Assert.IsTrue(Math.Abs(y - titleSize.Height) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - titleSize.Height) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - titleSize.Height) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }

    [TestMethod]
    public void PolarBasicCase()
    {
        var legend = new SKDefaultLegend();

        var chart = new SKPolarChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { new PolarLineSeries<double> { Values = new double[] { 1, 2, 3 } } },
            LegendPosition = LegendPosition.Top,
            Legend = legend,
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        var m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 0
        Assert.IsTrue(Math.Abs(y - 0) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m - legendSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m - legendSize.Width) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }

    [TestMethod]
    public void PolarWithTitleCase()
    {
        var legend = new SKDefaultLegend();
        legend._stackPanel.BackgroundPaint = new SolidColorPaint(SKColors.LightGray);

        var title = new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new Padding(10),
            Paint = new SolidColorPaint(SKColors.White),
            BackgroundColor = SKColors.Purple.AsLvcColor()
        };

        var chart = new SKPolarChart
        {
            Width = 500,
            Height = 500,
            Series = new[] { new PolarLineSeries<double> { Values = new double[] { 1, 2, 3 } } },
            LegendPosition = LegendPosition.Top,
            Title = title,
            Legend = legend
        };

        // TEST TOP LEGEND ================================================
        chart.LegendPosition = LegendPosition.Top;
        _ = chart.GetImage();

        var legendSize = legend.Measure(chart.Core);
        var titleSize = title.Measure(chart.Core);
        var dml = chart.Core.DrawMarginLocation;
        var dms = chart.Core.DrawMarginSize;
        var x = legend._stackPanel.X;
        var y = legend._stackPanel.Y;

        var m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be titleSize.Height
        Assert.IsTrue(Math.Abs(y - titleSize.Height) < 0.01);

        // TEST BOTTOM LEGEND ================================================
        chart.LegendPosition = LegendPosition.Bottom;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - m) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - m - legendSize.Height - titleSize.Height) < 0.01);

        // x should be centered
        Assert.IsTrue(Math.Abs(x - (500 * 0.5f - legendSize.Width * 0.5f)) < 0.01);

        // y should be 500 - legendsSize.Height
        Assert.IsTrue(Math.Abs(y - (500 - legendSize.Height)) < 0.01);

        // TEST LEFT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Left;
        _ = chart.GetImage();

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;
        var dx = dml.X - legendSize.Width; // <- why this distance?

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - dml.X - dx) < 0.01);

        // all the height should be used
        Assert.IsTrue(Math.Abs(500 - dms.Height - titleSize.Height - dx * 2) < 0.01);

        // x should be 0
        Assert.IsTrue(Math.Abs(x) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);

        // TEST RIGHT LEGEND ================================================
        chart.LegendPosition = LegendPosition.Right;
        _ = chart.GetImage();
        chart.SaveImage("mamá.png");

        legendSize = legend.Measure(chart.Core);
        dml = chart.Core.DrawMarginLocation;
        dms = chart.Core.DrawMarginSize;
        x = legend._stackPanel.X;
        y = legend._stackPanel.Y;

        m = Math.Min(dml.X, dml.Y) * 2;
        dx = dml.X + legendSize.Width; // <- why this distance?

        // all the width should be used
        Assert.IsTrue(Math.Abs(500 - dms.Width - dml.X - dx) < 0.01);

        // all the height should be used
        //Assert.IsTrue(Math.Abs(500 - dms.Height - dml.Y) < 0.01);

        // x should be 500 - legendsSize.Width
        Assert.IsTrue(Math.Abs(x - (500 - legendSize.Width)) < 0.01);

        // y should be centered
        Assert.IsTrue(Math.Abs(y - (500 * 0.5f - legendSize.Height * 0.5f)) < 0.01);
    }
}
