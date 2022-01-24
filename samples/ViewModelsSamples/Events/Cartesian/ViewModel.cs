using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

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

        var stockSeries = new StepLineSeries<Fruit>
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

        salesPerDaysSeries.ChartPointPointerDown += SalesPerDaysSeries_ChartPointPointerDown;
        stockSeries.ChartPointPointerDown += StockSeries_ChartPointPointerDown;

        Series = new ISeries[] { salesPerDaysSeries, stockSeries };
    }

    private void SalesPerDaysSeries_ChartPointPointerDown(
        IChartView chart,
        ChartPoint<Fruit, BezierPoint<CircleGeometry>, LabelGeometry> point)
    {
        Trace.WriteLine(
            $"[salesPerDay ChartPointPointerDown] clicked on {point.Model.Name}, {point.Model.SalesPerDay} items sold per day");
    }


    private void StockSeries_ChartPointPointerDown(
        IChartView chart,
        ChartPoint<Fruit, StepPoint<CircleGeometry>, LabelGeometry> point)
    {
        Trace.WriteLine(
            $"[stock ChartPointPointerDown] clicked on {point.Model.Name},  current stock {point.Model.Stock}");
    }

    public IEnumerable<ISeries> Series { get; set; }

    // XAML platforms also support ICommands
    public ICommand DataPointerDownCommand { get; set; } = new RelayCommand(); // mark
}
