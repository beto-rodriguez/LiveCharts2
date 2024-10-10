using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.UserDefinedTypes;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<City>
        {
            Name = "Population",
            // use the Population property as the Y coordinate
            // and the index of the city in our collection as the X coordinate
            Mapping = (city, index) => new(index, city.Population),
            Values = [
                new City { Name = "Tokyo", Population = 4 },
                new City { Name = "New York", Population = 6 },
                new City { Name = "Seoul", Population = 2 },
                new City { Name = "Moscow", Population = 8 },
                new City { Name = "Shanghai", Population = 3 },
                new City { Name = "Guadalajara", Population = 4 }
            ]
        }
    ];
}
