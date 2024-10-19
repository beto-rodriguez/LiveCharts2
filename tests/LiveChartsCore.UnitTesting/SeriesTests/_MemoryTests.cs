using System;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.UnitTesting.CoreObjectsTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.SeriesTests;

[TestClass]
public class _MemoryTests
{
    [TestMethod]
    public void ObservableValuesChangingTest()
    {
        TestObservablesChanging(new CartesianSut(new BoxSeries<ObservableValue>(), "box"));
        TestObservablesChanging(new CartesianSut(new ColumnSeries<ObservableValue>(), "colum"));
        TestObservablesChanging(new CartesianSut(new CandlesticksSeries<ObservableValue>(), "candle"));
        TestObservablesChanging(new CartesianSut(new HeatSeries<ObservableValue>(), "heat"));
        TestObservablesChanging(new CartesianSut(new LineSeries<ObservableValue>(), "line"));
        TestObservablesChanging(new CartesianSut(new RowSeries<ObservableValue>(), "row"));
        TestObservablesChanging(new CartesianSut(new ScatterSeries<ObservableValue>(), "scatter"));
        TestObservablesChanging(new CartesianSut(new StepLineSeries<ObservableValue>(), "step line"));
        TestObservablesChanging(new PieSut(new PieSeries<ObservableValue>(), "pie"));
        TestObservablesChanging(new PolarSut(new PolarLineSeries<ObservableValue>(), "polar"));
        // stacked series are irrelevant for this test because they inherit from some type above.
    }

    private abstract class ChartSut
    {
        public InMemorySkiaSharpChart Chart { get; set; }
        public ISeries Series { get; set; }
        public ObservableCollection<ObservableValue> Values { get; set; }

        protected ChartSut(
            InMemorySkiaSharpChart chart,
            ISeries series,
            string name)
        {
            series.Name = name;
            series.Values = Values = [];
            Series = series;
            Chart = chart;
        }
    }

    private class CartesianSut(
        ISeries series,
        string name)
            : ChartSut(new SKCartesianChart
            {
                Series = [series],
                AnimationsSpeed = TimeSpan.FromMilliseconds(10),
                EasingFunction = EasingFunctions.Lineal,
                Width = 1000,
                Height = 1000
            },
            series,
            name)
    { }

    private class PieSut(
        ISeries series,
        string name)
            : ChartSut(new SKPieChart
            {
                Series = [series],
                AnimationsSpeed = TimeSpan.FromMilliseconds(10),
                EasingFunction = EasingFunctions.Lineal,
                Width = 1000,
                Height = 1000
            },
            series,
            name)
    { }

    private class PolarSut(
        ISeries series,
        string name)
            : ChartSut(new SKPolarChart
            {
                Series = [series],
                AnimationsSpeed = TimeSpan.FromMilliseconds(10),
                EasingFunction = EasingFunctions.Lineal,
                Width = 1000,
                Height = 1000
            },
            series,
            name)
    { }

    private void TestObservablesChanging(ChartSut sut)
    {
        // this test adds, removes, clears, and changes the visibility multiple times
        // the memory and gemeotries count should not increase significantly

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(true);

        _ = ChangingPaintTasks.DrawChart(sut.Chart, true);

        var canvas = sut.Chart.CoreCanvas;
        var geometries = canvas.CountGeometries();
        var deltas = 0;
        var totalFramesDrawn = 0;

        for (var i = 0; i < 100; i++)
        {
            sut.Series.IsVisible = false;
            totalFramesDrawn += ChangingPaintTasks.DrawChart(sut.Chart, true);

            sut.Series.IsVisible = true;
            totalFramesDrawn += ChangingPaintTasks.DrawChart(sut.Chart, true);

            for (var j = 0; j < 5000; j++)
                sut.Values.Add(new(2));
            totalFramesDrawn += ChangingPaintTasks.DrawChart(sut.Chart, true);

            sut.Values.RemoveAt(0);
            sut.Values.RemoveAt(0);
            sut.Values.RemoveAt(0);
            sut.Values.RemoveAt(0);
            totalFramesDrawn += ChangingPaintTasks.DrawChart(sut.Chart, true);

            sut.Values.Clear();
            totalFramesDrawn += ChangingPaintTasks.DrawChart(sut.Chart, true);

            var newCount = canvas.CountGeometries();
            if (newCount > geometries)
            {
                deltas++;
                geometries = newCount;
            }
        }

        Assert.IsTrue(deltas <= 1);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert that there is no significant increase in memory usage
        var finalMemory = GC.GetTotalMemory(true);

        // 2MB is a reasonable threshold for this test
        // it adds 5,000 points 100 times, which is 500,000 points
        // removes 4 points 100 times, which is 400 points
        // clears the collection and changes visibility 100 times

        // this test also simulates the chart animation, it makes a change,
        // then enters a loop until animations finish.

        Assert.IsTrue(
            finalMemory - initialMemory < 2 * 1024 * 1024,
            $"Potential memory leak detected {(finalMemory - initialMemory) / (1024d * 1024):N2}MB, {totalFramesDrawn} frames drawn.");
    }
}
