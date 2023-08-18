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

namespace LiveChartsCore.UnitTesting.LayoutTests;

[TestClass]
public class TableLayoutTest
{
    [TestMethod]
    public void TableStart()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Start,
            HorizontalAlignment = Drawing.Align.Start
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

        _ = chart.GetImage();

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

    [TestMethod]
    public void TableStartFirstEmpty()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Start,
            HorizontalAlignment = Drawing.Align.Start
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 10,
            Height = 20,
            Fill = new SolidColorPaint(SKColors.Red)
        }, 1, 1);

        table.AddChild(g1 = new GeometryVisual<RectangleGeometry>
        {
            Width = 15,
            Height = 10,
            Fill = new SolidColorPaint(SKColors.Green)
        }, 1, 2);

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 20,
            Height = 30,
            Fill = new SolidColorPaint(SKColors.Yellow)
        }, 2, 1);

        table.AddChild(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 25,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Blue)
            }, 2, 2);

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

        _ = chart.GetImage();

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

    [TestMethod]
    public void TableStartSomeEmpty()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Start,
            HorizontalAlignment = Drawing.Align.Start
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 5,
            Height = 20,
            Fill = new SolidColorPaint(SKColors.Green.WithAlpha(100))
        }, 0, 0);

        // ignore the 0,1 cell

        table.AddChild(g1 = new GeometryVisual<RectangleGeometry>
        {
            Width = 15,
            Height = 10,
            Fill = new SolidColorPaint(SKColors.Green)
        }, 0, 2);

        // also ignore the row 1

        //table.AddChild(new GeometryVisual<RectangleGeometry>
        //{
        //    Width = 10,
        //    Height = 30,
        //    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100))
        //}, 2, 0);

        table.AddChild(new GeometryVisual<RectangleGeometry>
        {
            Width = 10,
            Height = 30,
            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(200))
        }, 2, 1);

        table.AddChild(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 25,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Blue)
            }, 2, 2);

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

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var tableSize = table.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            // 100 left + 10 padding + 15 column width
            g1Geometry.X == 100 + px + 15 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + py &&

            // 100 left + 10 padding + 15 column width
            g2Geometry.X == 100 + px + 15 &&

            // 200 top + 20 padding + 20 previous row height
            g2Geometry.Y == 200 + py + 20 &&

            // 10 padding + columns width + 10 padding
            tableSize.Width == px + 5 + 10 + 25 + px &&
            // 10 padding + rows width + 10 padding
            tableSize.Height == py + 20 + 30 + py);
    }

    [TestMethod]
    public void TableMiddle()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Middle,
            HorizontalAlignment = Drawing.Align.Middle
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

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var tableSize = table.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            g1Geometry.X == 135 &&

            // 200 top + 20 padding
            g1Geometry.Y == 225 &&

            // 100 left + 10 padding + 20 column width
            g2Geometry.X == 130 &&

            // 200 top + 20 padding + 20 previous row height
            g2Geometry.Y == 245 &&

            // 10 padding + columns width + 10 padding
            tableSize.Width == px + 20 + 25 + px &&
            // 10 padding + rows width + 10 padding
            tableSize.Height == py + 20 + 30 + py);
    }

    [TestMethod]
    public void TableEnd()
    {
        var px = 10;
        var py = 20;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.End,
            HorizontalAlignment = Drawing.Align.End
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

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var tableSize = table.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            g1Geometry.X == 140 &&

            // 200 top + 20 padding
            g1Geometry.Y == 230 &&

            // 100 left + 10 padding + 20 column width
            g2Geometry.X == 130 &&

            // 200 top + 20 padding + 20 previous row height
            g2Geometry.Y == 250 &&

            // 10 padding + columns width + 10 padding
            tableSize.Width == px + 20 + 25 + px &&
            // 10 padding + rows width + 10 padding
            tableSize.Height == py + 20 + 30 + py);
    }

    [TestMethod]
    public void UnevenColumns()
    {
        var px = 0;
        var py = 0;

        var table = new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(px, py),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Start,
            HorizontalAlignment = Drawing.Align.Start
        };

        LabelVisual g00, g01, g10, g11;

        table.AddChild(g00 = new LabelVisual
        {
            Text = "xxxx",
            Paint = new SolidColorPaint(SKColors.Red)
        }, 0, 0);

        table.AddChild(g01 = new LabelVisual
        {
            Text = "xxxxxxxxxxxxxxxxxxxxxxxx",
            Paint = new SolidColorPaint(SKColors.Green)
        }, 0, 1);

        table.AddChild(g10 = new LabelVisual
        {
            Text = "xxxxxxxxxxxxxxxxxxxxxxxx",
            Paint = new SolidColorPaint(SKColors.Yellow)
        }, 1, 0);

        table.AddChild(g11 = new LabelVisual
        {
            Text = "xxxx",
            Paint = new SolidColorPaint(SKColors.Blue)
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

        _ = chart.GetImage();

        var g00g = (LabelGeometry)g00.GetDrawnGeometries()[0];
        var g01g = (LabelGeometry)g01.GetDrawnGeometries()[0];
        var g10g = (LabelGeometry)g10.GetDrawnGeometries()[0];
        var g11g = (LabelGeometry)g11.GetDrawnGeometries()[0];

        var g00m = g00g.Measure(g00.Paint);
        var g01m = g01g.Measure(g01.Paint);
        var g10m = g10g.Measure(g10.Paint);
        var g11m = g11g.Measure(g11.Paint);

        var wc0 = g00m.Width > g01m.Width ? g00m.Width : g10m.Width;
        var wc1 = g10m.Width > g11m.Width ? g01m.Width : g11m.Width;

        var tableSize = table.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            // 10 padding + columns width + 10 padding
            tableSize.Width == wc0 + wc1);
    }
}
