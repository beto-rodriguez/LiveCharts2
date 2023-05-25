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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            // 100 left + 10 padding
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // 100 left + 10 padding + 15 previous width
            g2Geometry.X == 100 + 10 + 15 &&

            // preserve the previous y...
            g2Geometry.Y == 200 + 20 &&

            // 10 padding + 15 first shape + 15 second shape + 10 padding
            panelSize.Width == 10 + 15 + 15 + 10 &&
            panelSize.Height == 20 + 25 + 20);
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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.Core);

        Assert.IsTrue(
            // 100 left + 10 padding 
            g1Geometry.X == 100 + 10 &&

            // 200 top + 20 padding
            g1Geometry.Y == 200 + 20 &&

            // preserve the previous x...
            g2Geometry.X == 110 &&

            // 200 top + 20 padding + 25 previous height
            g2Geometry.Y == 200 + 20 + 25 &&

            panelSize.Width == 10 + 25 + 10 &&
            panelSize.Height == 20 + 25 + 25 + 20);
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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        stackPanel.Children.Add(
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

    [TestMethod]
    public void StackPanelHorizontalStartWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 100,
            Y = 200,
            MaxWidth = 12 * 3,
            VerticalAlignment = Drawing.Align.Start,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 100 + 25 &&
            g0Geometry.Y == 200 &&
            g1Geometry.X == 100 + 20 &&
            g1Geometry.Y == 200 + 15 &&
            g2Geometry.X == 100 + 24 &&
            g2Geometry.Y == 200 + 15 + 15 &&
            panelSize.Width == 36 &&
            panelSize.Height == 50);
    }

    [TestMethod]
    public void StackPanelHorizontalMiddleWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 100,
            Y = 200,
            MaxWidth = 12 * 3,
            VerticalAlignment = Drawing.Align.Middle,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 100 + 25 &&
            g0Geometry.Y == 205 &&
            g1Geometry.X == 100 + 20 &&
            g1Geometry.Y == 220 &&
            g2Geometry.X == 100 + 24 &&
            g2Geometry.Y == 237.5 &&
            panelSize.Width == 36 &&
            panelSize.Height == 50);
    }

    [TestMethod]
    public void StackPanelHorizontalEndWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 100,
            Y = 200,
            MaxWidth = 12 * 3,
            VerticalAlignment = Drawing.Align.End,
            Orientation = Drawing.ContainerOrientation.Horizontal
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 15,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 10,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 20,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 12,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 100 + 25 &&
            g0Geometry.Y == 210 &&
            g1Geometry.X == 100 + 20 &&
            g1Geometry.Y == 225 &&
            g2Geometry.X == 100 + 24 &&
            g2Geometry.Y == 245 &&
            panelSize.Width == 36 &&
            panelSize.Height == 50);
    }

    [TestMethod]
    public void StackPanelVerticalStartWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 200,
            Y = 100,
            MaxHeight = 12 * 3,
            HorizontalAlignment = Drawing.Align.Start,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 200 &&
            g0Geometry.Y == 125 &&
            g1Geometry.X == 215 &&
            g1Geometry.Y == 120 &&
            g2Geometry.X == 230 &&
            g2Geometry.Y == 124 &&
            panelSize.Width == 50 &&
            panelSize.Height == 36);
    }

    [TestMethod]
    public void StackPanelVerticalMiddleWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 200,
            Y = 100,
            MaxHeight = 12 * 3,
            HorizontalAlignment = Drawing.Align.Middle,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 205 &&
            g0Geometry.Y == 125 &&
            g1Geometry.X == 220 &&
            g1Geometry.Y == 120 &&
            g2Geometry.X == 237.5 &&
            g2Geometry.Y == 124 &&
            panelSize.Width == 50 &&
            panelSize.Height == 36);
    }

    [TestMethod]
    public void StackPanelVerticalEndWrap()
    {
        var stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Drawing.Padding(0),
            X = 200,
            Y = 100,
            MaxHeight = 12 * 3,
            HorizontalAlignment = Drawing.Align.End,
            Orientation = Drawing.ContainerOrientation.Vertical
        };

        GeometryVisual<RectangleGeometry> g0, g1, g2;

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            g0 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 10,
                Fill = new SolidColorPaint(SKColors.Yellow)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Green)
            });

        stackPanel.Children.Add(
            new GeometryVisual<RectangleGeometry>
            {
                Width = 20,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Blue)
            });

        stackPanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                Width = 5,
                Height = 12,
                Fill = new SolidColorPaint(SKColors.Yellow)
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

        var g0Geometry = (RectangleGeometry)g0.GetDrawnGeometries()[0];
        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        var panelSize = stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart.CoreChart);

        Assert.IsTrue(
            g0Geometry.X == 210 &&
            g0Geometry.Y == 125 &&
            g1Geometry.X == 225 &&
            g1Geometry.Y == 120 &&
            g2Geometry.X == 245 &&
            g2Geometry.Y == 124 &&
            panelSize.Width == 50 &&
            panelSize.Height == 36);
    }
}
