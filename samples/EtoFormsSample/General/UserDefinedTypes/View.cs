using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.General.UserDefinedTypes;

public class City
{
    public string Name { get; set; } = string.Empty;
    public double Population { get; set; }
}

public class View : Panel
{
    public View()
    {
        // Register the mapping for City type
        LiveCharts.Configure(config =>
            config.HasMap<City>((city, index) => new(index, city.Population)));

        var values = new City[]
        {
            new() { Name = "Tokyo", Population = 4 },
            new() { Name = "New York", Population = 6 },
            new() { Name = "Seoul", Population = 2 },
            new() { Name = "Moscow", Population = 8 },
            new() { Name = "Shanghai", Population = 3 },
            new() { Name = "Guadalajara", Population = 4 }
        };

        static string tooltipFormatter(ChartPoint point)
        {
            var city = (City)point.Context.DataSource!;
            return $"{city.Population}M people in {city.Name}";
        }

        var series = new ISeries[]
        {
            new LineSeries<City>
            {
                Name = "Population",
                Values = values,
                YToolTipLabelFormatter = (Func<ChartPoint, string>)tooltipFormatter
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
        };

        Content = cartesianChart;
    }
}
