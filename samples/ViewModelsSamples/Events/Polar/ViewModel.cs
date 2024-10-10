using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Events.Polar;

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

        var polarLineSeries = new PolarLineSeries<City>
        {
            Values = data,
            RadiusToolTipLabelFormatter = point => $"{point.Model?.Name} {point.Model?.Population} Million",
            // use the Population property in the Y axis
            // and the index of the city in the array as the X axis
            Mapping = (city, index) => new(index, city.Population)
        };

        Series =
        [
            polarLineSeries,
            new PolarLineSeries<int> { Values = [6, 7, 2, 9, 6, 2] },
        ];
    }

    public ISeries[] Series { get; set; }

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
