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
public class StackPanelTest
{
    [TestMethod]
    public void StackPanelHorizontalStart()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.Start,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // 100 left + 10 padding + 15 previous width
            g2Geometry.X == 100 + 10 + 15 &&

            // preserve the previous y...
            g2Geometry.Y == 200 + 20);
    }

    [TestMethod]
    public void StackPanelHorizontalMiddle()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            // also default should be middle...
            //VerticalAlignment = Drawing.Align.Middle,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // 100 left + 10 padding + 15 previous width
            g2Geometry.X == 100 + 10 + 15 &&

            // preserve the previous y... + half of the difference between the heights
            g2Geometry.Y == 200 + 20 + 5);
    }

    [TestMethod]
    public void StackPanelHorizontalEnd()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            VerticalAlignment = Drawing.Align.End,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // 100 left + 10 padding + 15 previous width
            g2Geometry.X == 100 + 10 + 15 &&

            // preserve the previous y... + difference between the heights
            g2Geometry.Y == 200 + 20 + 10);
    }

    [TestMethod]
    public void StackPanelVerticalStart()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            HorizontalAlignment = Drawing.Align.Start,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 25,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding 
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // preserve the previous x...
            g2Geometry.X == 110 &&

            // 200 top + 20 padding + 25 previous height
            g2Geometry.Y == 200 + 20 + 25);
    }

    [TestMethod]
    public void StackPanelVerticalMiddle()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            // also default should be middle...
            //HorizontalAlignment = Drawing.Align.Middle,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 25,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding + 7.5 half of the widths difference
            g1Geometry.X == 100 + 10 + 7.5 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // preserve the previous x...
            g2Geometry.X == 110 &&

            // 200 top + 20 padding + 25 previous height
            g2Geometry.Y == 200 + 20 + 25);
    }

    [TestMethod]
    public void StackPanelVerticalEnd()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(10, 20),
            X = 100,
            Y = 200,
            HorizontalAlignment = Drawing.Align.End,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 25,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

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
                stackPanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            // 100 left + 10 padding + the widths difference
            g1Geometry.X == 100 + 10 + 15 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // preserve the previous x...
            g2Geometry.X == 110 &&

            // 200 top + 20 padding + 25 previous height
            g2Geometry.Y == 200 + 20 + 25);
    }
}
