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
public class RelativePanelTest
{
    [TestMethod]
    public void BasicCase()
    {
        var relativePanel = new RelativePanel<RectangleGeometry, SkiaSharpDrawingContext>
        {
            X = 100,
            Y = 200
        };

        GeometryVisual<RectangleGeometry> g1, g2;

        _ = relativePanel.Children.Add(
            g1 = new GeometryVisual<RectangleGeometry>
            {
                X = 10,
                Y = 15,
                Width = 15,
                Height = 25,
                Fill = new SolidColorPaint(SKColors.Red)
            });

        _ = relativePanel.Children.Add(
            g2 = new GeometryVisual<RectangleGeometry>
            {
                X = 20,
                Y = 30,
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
                relativePanel
            }
        };

        _ = chart.GetImage();

        var g1Geometry = (RectangleGeometry)g1.GetDrawnGeometries()[0];
        var g2Geometry = (RectangleGeometry)g2.GetDrawnGeometries()[0];

        Assert.IsTrue(
            g1Geometry.X == 110 &&
            g1Geometry.Y == 215 &&
            g2Geometry.X == 120 &&
            g2Geometry.Y == 230);
    }
}
