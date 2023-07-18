using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;

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
            Values = samples
        }
    },
    XAxes = new[] { new Axis { Labeler = value => $"{value} seconds" } },
    YAxes = new[] { new Axis { Labeler = value => $"{value} °C" } }
};

// save the chart.
chart.SaveImage("IChartEntity.png");

// LiveCharts will skip null points.
var data = samples.ToList();
data.Add(null!);
data.Add(new TempSample { Time = 10, Temperature = 100 });
data.Add(new TempSample { Time = 15, Temperature = 80 });
chart.Series.First().Values = data;

chart.SaveImage("IChartEntity and nulls.png");

Console.WriteLine("chart saved");

// -------------------------------------------------------------------
// the following objects are examples of the recommended way to
// implement the IChartEntity interface.
// -------------------------------------------------------------------

// This is the class before implementing the IChartEntity interface.
public class TempSampleBase
{
    public int Time { get; set; }
    public double Temperature { get; set; }
}

// Now the recommended way to implement the IChartEntity interface.
// it requires CommunityToolkit.Mvvm
// this object notifies the chart to update the UI when a property changes and also caches the coordinate.
// To reduce the amount of boilerplate this sample uses the ObservableProperty attribute:
// https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
public partial class TempSample : ObservableObject, IChartEntity
{
    [ObservableProperty]
    private int _time;

    [ObservableProperty]
    private double _temperature;

    // Use the coordinate property to let LiveCharts know the position of the point.
    public Coordinate Coordinate { get; set; }

    // The meta data property is used by LiveCharts to store info about the plot.
    public ChartEntityMetaData? MetaData { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        Coordinate = new(Time, Temperature);
        base.OnPropertyChanged(e);
    }
}

// Imagine the case were we have an array of Cities, and we want to plot the population of each city.
// In this case we can use the index of the item in the array as X and the population as Y.
public partial class City : ObservableObject, IChartEntity
{
    [ObservableProperty]
    private double _population;

    public City()
    {
        MetaData = new ChartEntityMetaData(OnCoordinateChanged);
    }

    public Coordinate Coordinate { get; set; }

    public ChartEntityMetaData? MetaData { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (MetaData is not null) OnCoordinateChanged(MetaData.EntityIndex);
        base.OnPropertyChanged(e);
    }

    protected void OnCoordinateChanged(int index)
    {
        Coordinate = new(index, Population);
    }
}

// DateTime object
public partial class DateSample : ObservableObject, IChartEntity
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private double _temperature;

    public Coordinate Coordinate { get; set; }

    public ChartEntityMetaData? MetaData { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        // The objects defined by LiveCharts use the Ticks property to plot the X axis.
        // You can create your own implementation in this class, but if you go this way,
        // the docs will not be valid for you.
        Coordinate = new(Date.Ticks, Temperature);
        base.OnPropertyChanged(e);
    }
}

// Just a null-able property
public partial class TempNullableSample : ObservableObject, IChartEntity
{
    [ObservableProperty]
    private int _time;

    [ObservableProperty]
    private double? _temperature;

    public Coordinate Coordinate { get; set; }

    public ChartEntityMetaData? MetaData { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        // LiveCharts will skip Coordinate.Empty.
        Coordinate = Temperature is null
            ? Coordinate.Empty
            : new(Time, Temperature.Value);

        base.OnPropertyChanged(e);
    }
}

// -------------------------------------------------------------------
// IMPORTANT NOTE
// -------------------------------------------------------------------
// There are 2 special plots that use more than X and Y coordinates.

// Weited plots: HeatMaps and Bubble charts use 3 coordinates, X, Y and Weight.
// Coordinate = new Coordinate(X, Y, Weight);
// https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/LiveChartsCore/Defaults/WeightedPoint.cs

// While financial Points use 5.
// Coordinate = new Coordinate(High, X, Open, Close, Low);
// https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/LiveChartsCore/Defaults/FinancialPoint.cs
