using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using SkiaSharp;

namespace ViewModelsSamples.Events.Cartesian;

public class ViewModel
{
    public ViewModel()
    {
        var data = new[]
        {
            new Fruit { Name = "Apple", SalesPerDay = 4, Stock = 6 },
            new Fruit { Name = "Orange", SalesPerDay = 6, Stock = 4 },
            new Fruit { Name = "Pinaple", SalesPerDay = 2, Stock = 2 },
            new Fruit { Name = "Potoato", SalesPerDay = 8, Stock = 4 },
            new Fruit { Name = "Lettuce", SalesPerDay = 3, Stock = 6 },
            new Fruit { Name = "Cherry", SalesPerDay = 4, Stock = 8 }
        };

        var salesPerDaysSeries = new LineSeries<Fruit>
        {
            Name = "Items sold per day",
            Values = data,
            TooltipLabelFormatter = point => $"{point.Model.Name}, sold {point.Model.SalesPerDay} items",
            Mapping = (fruit, point) =>
            {
                point.PrimaryValue = fruit.SalesPerDay; // use the SalesPerDay property in this series // mark
                point.SecondaryValue = point.Context.Index;
            }
        };

        var stockSeries = new LineSeries<Fruit>
        {
            Name = "Active stock",
            Values = data,
            TooltipLabelFormatter = point => $"{point.Model.Stock} items remaining",
            Mapping = (fruit, point) =>
            {
                point.PrimaryValue = fruit.Stock; // use the Stock property in this series // mark
                point.SecondaryValue = point.Context.Index;
            }
        };

        salesPerDaysSeries.ChartPointPointerDown += SalesPerDaysSeries_ChartPointPointerDown1;

        Series = new ISeries[] { salesPerDaysSeries, stockSeries };
    }

    private void SalesPerDaysSeries_ChartPointPointerDown1(IChartView chart, ChartPoint<Fruit, BezierPoint<CircleGeometry>, LabelGeometry> point)
    {
        throw new System.NotImplementedException();
    }

    private void SalesPerDaysSeries_ChartPointPointerDown(
        IChartView chart, ChartPoint<
            Fruit,
            LineBezierVisualPoint<SkiaSharpDrawingContext, CircleGeometry, CubicBezierSegment, SKPath>,
            LabelGeometry> point)
    {
        throw new System.NotImplementedException();
    }

    private void ColumnSeries_ChartPointPointerDown(
        IChartView chart,
        ChartPoint<Fruit, CircleGeometry, LabelGeometry> point)
    {
        // the ChartPointPointerDown event is called when the user clicks over a chart point in the UI.

        Trace.WriteLine($"[series.ChartPointPointerDownEvent] clicked on {point.Model.Name}");
    }

    private void ColumnSeries_DataPointerDown(
        IChartView chart,
        IEnumerable<ChartPoint<Fruit, CircleGeometry, LabelGeometry>> points)
    {
        // When points overlap you can use the DataPointerDown event,
        // it will pass all the points clicked even if they overlapped.

        foreach (var point in points)
        {
            Trace.WriteLine($"[series.DataPointerDownEvent] clicked on {point.Model.Name}");
        }
    }

    public IEnumerable<ISeries> Series { get; set; }

    // XAML platforms also support ICommands
    public ICommand DataPointerDownCommand { get; set; } = new RelayCommand(); // mark
}
