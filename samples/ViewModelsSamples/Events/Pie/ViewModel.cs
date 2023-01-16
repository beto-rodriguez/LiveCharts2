using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Events.Pie;

[ObservableObject]
public partial class ViewModel
{
    public ViewModel()
    {
        var data = new[]
        {
            new City { Name = "Tokyo", Population = 4 },
            new City { Name = "New York", Population = 6 },
            new City { Name = "Seoul", Population = 2 },
            new City { Name = "Moscow", Population = 8 },
            new City { Name = "Shanghai", Population = 3 },
            new City { Name = "Guadalajara", Population = 4 }
        };

        // the parameter in the AsLiveChartsSeries() function is optional
        // and is usefull to customize each series
        // it is a function that takes the city and the series assigned to the city as parameters
        var seriesCollection = data.AsLiveChartsPieSeries(
            (city, series) =>
            {
                series.Name = city.Name;
                series.Mapping = (cityMapper, point) =>
                {
                    point.PrimaryValue = cityMapper.Population; // use the population property in this series // mark
                    point.SecondaryValue = point.Context.Index;
                };
                series.DataPointerDown += Series_DataPointerDown;
            });

        Series = seriesCollection.ToArray();
    }

    public ISeries[] Series { get; set; }

    private void Series_DataPointerDown(
        IChartView chart,
        IEnumerable<ChartPoint<City, DoughnutGeometry, LabelGeometry>> points)
    {
        // the event passes a collection of the points that were triggered by the pointer down event.
        foreach (var point in points)
        {
            Trace.WriteLine($"[series.dataPointerDownEvent] clicked on {point.Model?.Name}");
        }
    }

    [RelayCommand]
    public void DataPointerDown(IEnumerable<ChartPoint>? points)
    {
        if (points is null) return;

        // notice in the chart command we are not able to use strongly typed points
        // but we can cast the point.Context.DataSource to the actual type.

        foreach (var point in points)
        {
            if (point.Context.DataSource is City city)
            {
                Trace.WriteLine($"[chart.dataPointerDownCommand] clicked on {city.Name}");
                continue;
            }

            if (point.Context.DataSource is int integer)
            {
                Trace.WriteLine($"[chart.dataPointerDownCommand] clicked on number {integer}");
                continue;
            }

            // handle more possible types here...
            // if (point.Context.DataSource is Foo foo)
            // {
            //     ...
            // }
        }
    }
}
