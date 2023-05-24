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

using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting;

[TestClass]
public class TableLayoutTest
{
    [TestMethod]
    public void StackPanelHorizontalStart()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            //VerticalAlignment = Drawing.Align.Start,
            //Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 10,
            Height = 20,
            Fill = new SolidColorPaint(SKColors.Red)
        }, 0, 0);

        table.AddChild(g1 = new GeometryVisual<RectangleGeometry>
        {
            Width = 15,
            Height = 10,
            Fill = new SolidColorPaint(SKColors.Green)
        }, 0, 1);

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 20,
            Height = 30,
            Fill = new SolidColorPaint(SKColors.Yellow)
        }, 1, 0);

        table.AddChild(
        g2 = new GeometryVisual<RectangleGeometry>
        {
            Width = 25,
            Height = 20,
            Fill = new SolidColorPaint(SKColors.Blue)
        }, 1, 1);

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
            VisualElements = new VisualElement<SkiaSharpDrawingContext>[]
            {
                table
            }
        };

        chart.SaveImage("holdda.png");

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var tableSize = table.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            // 100 left + 10 padding + 20 column width
            g1Geometry.X == 100 + px + 20 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + py &&

            // 100 left + 10 padding + 20 column width
            g2Geometry.X == 100 + px + 20 &&

            // 200 top + 20 padding + 20 previous row height
            g2Geometry.Y == 200 + py + 20 &&

            // 10 padding + columns width + 10 padding
            tableSize.Width == px + 20 + 25 + px &&
            // 10 padding + rows width + 10 padding
            tableSize.Height == py + 20 + 30 + py);
    }
}
