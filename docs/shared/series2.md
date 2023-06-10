## Plotting custom types

You can teach LiveCharts to plot any type as soon as you let the library how to handle that object, there are 
two ways of doing so: *Mappers* or *implementing `IChartEntity`*, mappers are quick to setup, implementing 
`IChartEntity` is more performant and is the recommended way.

**Mappers**

```c#
public record TempSample(int Time, double Temperature, string Unit);

var chart = new SKCartesianChart
{
    Width = 900,
    Height = 600,
    Series = new[]
    {
        new {{ name  | to_title_case_no_spaces }}<TempSample>
        {
            Mapping = (sample, chartPoint) =>
            {
                // use temperature as primary value (normally Y)
                chartPoint.PrimaryValue = sample.Temperature;
                // use time as secondary value (normally X)
                chartPoint.SecondaryValue = sample.Time;
            },
            Values = samples
        }
    },
    XAxes = new[] { new Axis { Labeler = value => $"{value} seconds" } },
    YAxes = new[] { new Axis { Labeler = value => $"{value} °C" } }
};

// -------------------------------------------------------------------
// IMPORTANT NOTE
// -------------------------------------------------------------------
// There are 2 special plots that use more than X and Y coordinates.

// Weighted plots: HeatMaps and Bubble charts use 3 coordinates, X, Y and Weight.
// Mapping = (sample, chartPoint) =>
// {
//    chartPoint.PrimaryValue = sample.X;
//    chartPoint.SecondaryValue = sample.Y;
//    chartPoint.TertiaryValue = sample.Weigth;
// }

// While financial Points use 5.
// Coordinate = new Coordinate(High, X, Open, Close, Low);
// Mapping = (sample, chartPoint) =>
// {
//    chartPoint.PrimaryValue = sample.High;
//    chartPoint.SecondaryValue = sample.X;
//    chartPoint.TertiaryValue = sample.Open;
//    chartPoint.QuaternaryValue = sample.Close;
//    chartPoint.QuinaryValue = sample.Low;
//}
```

**Implementing IChartEntity**
```c#
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

// this object uses the CommunityToolkit.Mvvm to implement INotifyPropertyChanged also
public partial class TempSample : ObservableObject, IChartEntity
{
    [ObservableProperty]
    private int _time;

    [ObservableProperty]
    private double _temperature;

    // Use the coordinate property to let LiveCharts know the position of the point.
    public Coordinate Coordinate { get; protected set; }

    // The meta data property is used by LiveCharts to store info about the plot.
    public ChartEntityMetaData? MetaData { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        Coordinate = new(Time, Temperature);
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
```

<a href="https://lvcharts.com/docs/WPF/{{ version }}/Overview.Mappers" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
<b>See the full custom types article</b>
</a>

## Custom geometries

You can use any geometry to represent a point in a series.

<pre><code>Series = new List&lt;ISeries>
{
    // use the second argument type to specify the geometry to draw for every point
    // there are already many predefined geometries in the
    // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
    new {{ name  | to_title_case_no_spaces }}&lt;double, RectangleGeometry>
    {
        Values = new double[] { 3, 3, -3, -2, -4, -3, -1 }
    },

    // you can also define your own SVG geometry
    // MyGeometry class let us change the Path at runtime
    // Click on the on any point to change the path.
    // You can find the MyGeometry.cs file below
    new {{ name  | to_title_case_no_spaces }}&lt;double, MyGeometry>
    {
        Values = new double[] { -2, 2, 1, 3, -1, 4, 3 }
    }

    // Note: Depending on the series type, the geometry could require to satisfy some constrains
};

public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
{
    public MyGeometry()
        : base(SVGPoints.Star)
    { 
        // the LiveChartsCore.SkiaSharpView.SVGPoints contains many predefined SVG paths
        // you can also pass your own path there.
    }
}</code></pre>

## ZIndex property

Indicates an order in the Z axis, this order controls which series is above or behind.

## IsVisible property

Indicates if the series is visible in the user interface.

## DataPadding

The data padding is the minimum distance from the edges of the series to the axis limits, it is of type `System.Drawing.PointF` 
both coordinates (X and Y) goes from 0 to 1, where 0 is nothing and 1 is the axis tick an axis tick is the separation between
every label or separator (even if they are not visible).

If this property is not set, the library will set it according to the series type, take a look at the following samples:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(0, 0),
    Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

Produces the following result:

![image]({{ assets_url }}/docs/_assets/1.8.padding00.png)

But you can remove the padding only from an axis, for example:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(0.5f, 0),
    Values = new ObservableCollection&lt;double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

![image]({{ assets_url }}/docs/_assets/1.8.padding50.png)

Or you can increase the distance:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(2, 2),
    Values = new ObservableCollection&lt;double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

![image]({{ assets_url }}/docs/_assets/1.8.padding22.png)