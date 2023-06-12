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
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.UnitTesting.MockedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace LiveChartsCore.UnitTesting.CoreObjectsTests;

[TestClass]
public class ChangingPaintTasks
{
    [TestMethod]
    public void DrawableSeriesFillChanged()
    {
        var series = new LineSeries<int>
        {
            Values = new List<int> { 1, 6, 4, 2 }
        };

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries> { series },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        // on changing the fill task, the previous instance should be removed.
        series.Fill = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void DrawableSeriesStrokeChanged()
    {
        var series = new LineSeries<int>
        {
            Values = new List<int> { 1, 6, 4, 2 }
        };

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries> { series },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        series.Stroke = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void LineSeriesGeometryPaintsChanged()
    {
        var series = new LineSeries<int>
        {
            Values = new List<int> { 1, 6, 4, 2 }
        };

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries> { series },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        series.GeometryFill = new SolidColorPaint();
        series.GeometryStroke = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void SeriesRemoved()
    {
        var series = new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } };

        var seriesCollection = new List<ISeries> { series };

        var chart = new TestCartesianChartView
        {
            Series = seriesCollection,
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();
        seriesCollection.Add(new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } });

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        seriesCollection.RemoveAt(0);
        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void SeriesCollectionInstanceChanged()
    {
        var series = new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } };

        var seriesCollection = new List<ISeries> { series };

        var chart = new TestCartesianChartView
        {
            Series = seriesCollection,
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        chart.Series = new List<ISeries>
            {
                new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } }
            };

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void AxisTextBrushChanged()
    {
        var axis = new Axis();

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries>
                {
                    new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } },
                },
            XAxes = new[] { axis },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        axis.LabelsPaint = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void AxisSeparatorBrushChanged()
    {
        var axis = new Axis();

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries>
                {
                    new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } },
                },
            XAxes = new[] { new Axis() },
            YAxes = new[] { axis },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        axis.SeparatorsPaint = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void AxisCollectionInstanceChanged()
    {
        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries>
                {
                    new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } },
                },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        chart.XAxes = new[] { new Axis() };

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void SectionBrushChanged()
    {
        var section = new RectangularSection
        {
            Fill = new SolidColorPaint()
        };

        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries>
                {
                    new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } },
                },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
            Sections = new[] { section }
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        section.Fill = new SolidColorPaint();

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }

    [TestMethod]
    public void SectionsInstanceChanged()
    {
        var chart = new TestCartesianChartView
        {
            Series = new List<ISeries>
                {
                    new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } },
                },
            XAxes = new[] { new Axis() },
            YAxes = new[] { new Axis() },
            Sections = new[] { new RectangularSection() }
        };

        var canvas = chart.CoreCanvas;

        void DrawChart()
        {
            while (!canvas.IsValid)
                canvas.DrawFrame(
                    new SkiaSharpDrawingContext(
                        canvas,
                        new SKImageInfo(100, 100),
                        SKSurface.CreateNull(100, 100),
                        new SKCanvas(new SKBitmap())));
        }

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        chart.Sections = new[] { new RectangularSection() };

        chart.Core.Update(new ChartUpdateParams { Throttling = false });
        DrawChart();

        Assert.IsTrue(
            drawables == canvas.DrawablesCount &&
            geometries == canvas.CountGeometries());
    }
}
