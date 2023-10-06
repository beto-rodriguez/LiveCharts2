using System.Text.Json;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;

// -------------------------------------------------------------------
// EXAMPLE 1 ---------------------------------------------------------
// -------------------------------------------------------------------
// Map X and Y values using a mapper
// -------------------------------------------------------------------

// The data.json file represents the temperature of a CPU at a given time,
using var streamReader = new StreamReader("data.json");

// we will use the System.Text.Json library to deserialize the json file.
var samples = JsonSerializer.Deserialize<TempSample[]>(streamReader.ReadToEnd())!;

var chart = new SKCartesianChart
{
    Width = 900,
    Height = 600,
    Series = new[]
    {
        new LineSeries<TempSample>
        {
            // we set the X coordinate to the Time property
            // and the Y coordinate to the Temperature property
            Mapping = (tempSample, index) => new(tempSample.Time, tempSample.Temperature),
            Values = samples
        }
    },
    XAxes = new[] { new Axis { Labeler = value => $"{value} seconds" } },
    YAxes = new[] { new Axis { Labeler = value => $"{value} °C" } }
};

chart.SaveImage("using mappers.png");

// LiveCharts will skip null points.
var data = samples.ToList();
data.Add(null!);
data.Add(new TempSample(10, 100, "°C"));
data.Add(new TempSample(15, 80, "°C"));
chart.Series.First().Values = data;

chart.SaveImage("using mappers and nulls.png");

// -------------------------------------------------------------------
// EXAMPLE 2 ---------------------------------------------------------
// -------------------------------------------------------------------
// Map Y and use the index of the item in the array as X
// -------------------------------------------------------------------

// The cities.json file contains the population of some cities
using var citiesStreamReader = new StreamReader("cities.json");

// we will use the System.Text.Json library to deserialize the json file.
var cities = JsonSerializer.Deserialize<City[]>(citiesStreamReader.ReadToEnd())!;
cities = cities.OrderByDescending(x => x.Population).ToArray();

var citiesChart = new SKCartesianChart
{
    Width = 900,
    Height = 600,
    Series = new[]
    {
        new ColumnSeries<City>
        {
            // we set the X coordinate to the index of the item in the array
            // and the Y coordinate to the Population property
            Mapping = (city, index) => new(index, city.Population),
            Values = cities
        }
    },
    XAxes = new[]
    {
        new Axis
        {
            Labels = cities.Select(x => x.Name).ToArray(),
            LabelsRotation = 90
        }
    },
    YAxes = new[] { new Axis { Labeler = value => value.ToString("N2") } }
};

citiesChart.SaveImage("cities.png");

// -------------------------------------------------------------------
// EXAMPLE 3 ---------------------------------------------------------
// -------------------------------------------------------------------
// Register mappers globally
// -------------------------------------------------------------------

// you can remove the mapping from the series and register the mappers globally
// this is useful when you have a lot of series and you don't want to repeat the mapping for each series.
LiveCharts.Configure(config =>
    config
        .HasMap<TempSample>((tempSample, index) => new(tempSample.Time, tempSample.Temperature))
        .HasMap<City>((city, index) => new(index, city.Population)));

Console.WriteLine("chart saved");

public record TempSample(int Time, double Temperature, string Unit);
public record City(string Name, double Population);

// -------------------------------------------------------------------
// IMPORTANT NOTE
// -------------------------------------------------------------------
// There are some plots that use more than X and Y coordinates.

// Weighted plots: HeatMaps and Bubble charts use 3 coordinates, X, Y and Weight.
// Mapping = (sample, chartPoint) =>
// {
//     chartPoint.Coordinate = new(x, y, weight);
// }

// While financial Points use 5.
// Coordinate = new Coordinate(High, X, Open, Close, Low);
// Mapping = (sample, chartPoint) =>
// {
//     chartPoint.Coordinate = new(x, high, open, close, low);
// }
