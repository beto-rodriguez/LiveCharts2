using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.UnitTesting.MockedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.Collections.Generic;

namespace LiveChartsCore.UnitTesting
{
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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            // on changing the fill task, the previous instance should be removed.
            series.Fill = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            series.Stroke = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            series.GeometryFill = new SolidColorPaintTask();
            series.GeometryStroke = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            axis.LabelsPaint = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            axis.SeparatorsPaint = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
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
                Fill = new SolidColorPaintTask()
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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
            }

            chart.Core.Update(new ChartUpdateParams { Throttling = false });
            DrawChart();

            var drawables = canvas.DrawablesCount;
            var geometries = canvas.CountGeometries();

            section.Fill = new SolidColorPaintTask();

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
                {
                    canvas.DrawFrame(
                        new SkiaSharpDrawingContext(
                            canvas,
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap())));
                }
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
}
