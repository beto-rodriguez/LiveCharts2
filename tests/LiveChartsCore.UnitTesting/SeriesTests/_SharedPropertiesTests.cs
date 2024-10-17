using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.UnitTesting.CoreObjectsTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiveChartsCore.UnitTesting.SeriesTests;

[TestClass]
public class _SharedPropertiesTests
{
    [TestMethod]
    public void Visibility()
    {
        var values = new ObservableCollection<ObservableValue>();
        var lineSeries = new LineSeries<ObservableValue>(values);

        var chart = new SKCartesianChart
        {
            Series = [lineSeries],
            Width = 1000,
            Height = 1000
        };

        _ = chart.GetImage();

        var changes = 0;

        values.CollectionChanged +=
            (object? sender, NotifyCollectionChangedEventArgs e) =>
                changes++;

        values.Add(new(1));
        _ = chart.GetImage();

        Assert.IsTrue(changes == 1);

        lineSeries.IsVisible = false;
        _ = chart.GetImage();

        lineSeries.IsVisible = true;
        _ = chart.GetImage();

        values.Add(new(1));
        _ = chart.GetImage();

        Assert.IsTrue(changes == 2);
    }

    [TestMethod]
    public void VisbilityAndPaintsTasks()
    {
        var values = new ObservableCollection<DateTimePoint>() { new() };
        var lineSeries = new ColumnSeries<DateTimePoint>(values);
        var chart = new SKCartesianChart
        {
            Series = [lineSeries],
            Width = 1000,
            Height = 1000
        };

        _ = ChangingPaintTasks.DrawChart(chart, true);

        var canvas = chart.CoreCanvas;
        var drawables = canvas.DrawablesCount;
        var geometries = canvas.CountGeometries();

        lineSeries.IsVisible = false;
        _ = ChangingPaintTasks.DrawChart(chart, true);

        lineSeries.IsVisible = true;
        _ = ChangingPaintTasks.DrawChart(chart, true);

        Assert.IsTrue(
           drawables == canvas.DrawablesCount &&
           geometries == canvas.CountGeometries());
    }
}
