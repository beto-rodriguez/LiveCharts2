using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Events.Cartesian;

public partial class ViewModel : ObservableObject
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

        var salesPerDaysSeries = new ColumnSeries<Fruit>
        {
            Values = data,
            YToolTipLabelFormatter = point => $"{point.Model?.SalesPerDay} {point.Model?.Name}",
            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
            DataLabelsFormatter = point => $"{point.Model?.SalesPerDay} {point.Model?.Name}",
            DataLabelsPosition = DataLabelsPosition.End,
            // use the SalesPerDay property in this in the Y axis // mark
            // and the index of the fruit in the array in the X axis // mark
            Mapping = (fruit, index) => new(index, fruit.SalesPerDay)
        };

        // notice that the event signature is different for every series
        // use the  IDE intellisense to help you (see more bellow in this article). // mark
        salesPerDaysSeries.ChartPointPointerDown += OnPointerDown; // mark
        salesPerDaysSeries.ChartPointPointerHover += OnPointerHover; // mark
        salesPerDaysSeries.ChartPointPointerHoverLost += OnPointerHoverLost; // mark

        Series = new ISeries[] { salesPerDaysSeries };
    }

    public ISeries[] Series { get; set; }

    private void OnPointerDown(IChartView chart, ChartPoint<Fruit, RoundedRectangleGeometry, LabelGeometry>? point)
    {
        if (point?.Visual is null) return;
        point.Visual.Fill = new SolidColorPaint(SKColors.Red);
        chart.Invalidate(); // <- ensures the canvas is redrawn after we set the fill
        Trace.WriteLine($"Clicked on {point.Model?.Name}, {point.Model?.SalesPerDay} items sold per day");
    }

    private void OnPointerHover(IChartView chart, ChartPoint<Fruit, RoundedRectangleGeometry, LabelGeometry>? point)
    {
        if (point?.Visual is null) return;
        point.Visual.Fill = new SolidColorPaint(SKColors.Yellow);
        chart.Invalidate();
        Trace.WriteLine($"Pointer entered on {point.Model?.Name}");
    }

    private void OnPointerHoverLost(IChartView chart, ChartPoint<Fruit, RoundedRectangleGeometry, LabelGeometry>? point)
    {
        if (point?.Visual is null) return;
        point.Visual.Fill = null;
        chart.Invalidate();
        Trace.WriteLine($"Pointer left {point.Model?.Name}");
    }
}
