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
using System.Collections.Generic;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class LabelsMeasureTest
{
    [TestMethod]
    public void BasicCase()
    {
        var visuls = new List<ChartElement<SkiaSharpDrawingContext>>();

        var y = 10;
        var h = 20;
        for (var i = 0; i < 10; i++)
        {
            visuls.Add(new LabelVisual
            {
                X = 10,
                Y = y,
                LocationUnit = Measure.MeasureUnit.Pixels,
                VerticalAlignment = Drawing.Align.Start,
                HorizontalAlignment = Drawing.Align.Start,
                TextSize = h,
                Text = "X-█-X",
                Paint = new SolidColorPaint(SKColors.Black),
                BackgroundColor = new Drawing.LvcColor(0, 0, 0, 50)
            });

            y += h + 10;
            h += 20;
        }

        var chart = new SKCartesianChart
        {
            Width = 600,
            Height = y + 10,
            Series = Array.Empty<ISeries>(),
            XAxes = new[] { new Axis { IsVisible = false } },
            YAxes = new[] { new Axis { IsVisible = false } },
            VisualElements = visuls
        };

        _ = chart.GetImage();
        //chart.SaveImage("test.png"); // use this method to see the actual tested image
    }

    [TestMethod]
    public void MultiLine()
    {
        var visuls = new List<ChartElement<SkiaSharpDrawingContext>>();

        var y = 10f;
        var h = 25;
        var lines = 5;
        var lineHeight = 0.1f;

        for (var i = 0; i < 20; i++)
        {
            var t = "";
            for (var j = 0; j < lines; j++)
                t += $"{(j % 2 == 0 ? "-" : "")}█-{lineHeight:N2}-█{(j != lines - 1 ? Environment.NewLine : "")}";

            var th = h * lines;

            visuls.Add(new LabelVisual
            {
                X = 10,
                Y = y,
                LocationUnit = Measure.MeasureUnit.Pixels,
                VerticalAlignment = Drawing.Align.Start,
                HorizontalAlignment = Drawing.Align.Start,
                TextSize = 25,
                Text = t,
                Paint = new SolidColorPaint(SKColors.Black),
                BackgroundColor = new Drawing.LvcColor(0, 0, 0, 50),
                LineHeight = lineHeight
            });

            y += th * lineHeight + 10;
            lineHeight += 1f;
        }

        var chart = new SKCartesianChart
        {
            Width = 900,
            Height = (int)(y + 10),
            Series = Array.Empty<ISeries>(),
            XAxes = new[] { new Axis { IsVisible = false } },
            YAxes = new[] { new Axis { IsVisible = false } },
            VisualElements = visuls
        };

        _ = chart.GetImage();
        //chart.SaveImage("multi line labels test.png"); // use this method to see the actual tested image
    }
}

