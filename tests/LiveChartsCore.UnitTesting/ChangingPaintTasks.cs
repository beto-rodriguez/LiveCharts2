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
        public void SeriesFillChanged()
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
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap()))
                        { IsTest = true });
                }
            }

            chart.Core.Update(false);
            DrawChart();

            var drawables = canvas.DrawablesCount;

            // on changing the fill task, the previouns instance should be removed.
            series.Fill = new SolidColorPaintTask();

            chart.Core.Update(false);
            DrawChart();

            Assert.IsTrue(drawables == canvas.DrawablesCount);
        }

        [TestMethod]
        public void SeriesStrokeChanged()
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
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap()))
                        { IsTest = true });
                }
            }

            chart.Core.Update(false);
            DrawChart();

            var drawables = canvas.DrawablesCount;

            series.Stroke = new SolidColorPaintTask();

            chart.Core.Update(false);
            DrawChart();

            Assert.IsTrue(drawables == canvas.DrawablesCount);
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
                            new SKImageInfo(100, 100),
                            SKSurface.CreateNull(100, 100),
                            new SKCanvas(new SKBitmap()))
                        { IsTest = true });
                }
            }

            chart.Core.Update(false);
            DrawChart();

            var drawables = canvas.DrawablesCount;
            seriesCollection.Add(new LineSeries<int> { Values = new List<int> { 1, 6, 4, 2 } });

            chart.Core.Update(false);
            DrawChart();

            seriesCollection.RemoveAt(0);
            chart.Core.Update(false);
            DrawChart();

            Assert.IsTrue(drawables == canvas.DrawablesCount);
        }
    }
}
