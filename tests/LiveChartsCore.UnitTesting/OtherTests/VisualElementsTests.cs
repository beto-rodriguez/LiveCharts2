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

using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.OtherTests;

[TestClass]
public class VisualElementsTests
{
    [TestMethod]
    public void Dispose()
    {
        var suts = new List<VisualElement<SkiaSharpDrawingContext>>
        {
            new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>(),
            new RelativePanel<RectangleGeometry, SkiaSharpDrawingContext>(),
            new TableLayout<RectangleGeometry, SkiaSharpDrawingContext>(),
            new GeometryVisual<RectangleGeometry>(),
            new VariableGeometryVisual<SkiaSharpDrawingContext>(new RectangleGeometry()),
            new NeedleVisual
            {
                Fill = new SolidColorPaint(SKColors.Red)
            },
            new AngularTicksVisual
            {
                Stroke = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Blue)
            }
        };

        var chart = new SKPieChart
        {
            Width = 1000,
            Height = 1000,
            Total = 100
        };

        void Draw()
        {
            var coreChart = chart.Core;

            chart.CoreCanvas.DisableAnimations = true;
            coreChart.IsLoaded = true;
            coreChart._isFirstDraw = true;
            coreChart.Measure();

            using var surface = SKSurface.Create(new SKImageInfo(chart.Width, chart.Height));
            using var canvas = surface.Canvas;

            chart.CoreCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    chart.CoreCanvas,
                    new SKImageInfo(chart.Width, chart.Height),
                    null!,
                    canvas,
                    SKColors.White,
                    true));
        }

        Draw();

        // get the count of geometries and paint tasks without the visuals
        var g = chart.CoreCanvas.CountGeometries();
        var p = chart.CoreCanvas._paintTasks.Count;

        // add trhe visuals and ensure that the visuals were drawn
        chart.VisualElements = suts;
        Draw();
        Assert.IsTrue(
            chart.CoreCanvas.CountGeometries() > g &&
            chart.CoreCanvas._paintTasks.Count > p);

        // clear the visuals and ensure that all the geometries and paints were removed
        chart.VisualElements = new List<VisualElement<SkiaSharpDrawingContext>>();
        Draw();
        Assert.IsTrue(
            chart.CoreCanvas.CountGeometries() == g &&
            chart.CoreCanvas._paintTasks.Count == p);
    }
}
