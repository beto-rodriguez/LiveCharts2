<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scatterstroke.png)

<pre><code>Series = new ISeries[]
{
    new ScatterSeries&lt;ObservablePoint>
    {
        Stroke = new SolidColorPaintTask(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
        Values = new ObservableCollection&lt;ObservablePoint>
        {
            new ObservablePoint(2.2, 5.4),
            new ObservablePoint(4.5, 2.5),
            new ObservablePoint(4.2, 7.4),
            new ObservablePoint(6.4, 9.9),
            new ObservablePoint(4.2, 9.2),
            new ObservablePoint(5.8, 3.5),
            new ObservablePoint(7.3, 5.8),
            new ObservablePoint(8.9, 3.9),
            new ObservablePoint(6.1, 4.6),
            new ObservablePoint(9.4, 7.7),
            new ObservablePoint(8.4, 8.5),
            new ObservablePoint(3.6, 9.6),
            new ObservablePoint(4.4, 6.3),
            new ObservablePoint(5.8, 4.8),
            new ObservablePoint(6.9, 3.4),
            new ObservablePoint(7.6, 1.8),
            new ObservablePoint(8.3, 8.3),
            new ObservablePoint(9.9, 5.2),
            new ObservablePoint(8.1, 4.7),
            new ObservablePoint(7.4, 3.9),
            new ObservablePoint(6.8, 2.3),
            new ObservablePoint(5.3, 7.1),
        }
    }
};</code></pre>

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scatterfill.png)

<pre><code>Series = new ISeries[]
{
    new ScatterSeries&lt;ObservablePoint>
    {
        Fill = new SolidColorPaintTask(SKColors.Blue), // mark
        Stroke = null,
        Values = new ObservableCollection&lt;ObservablePoint>
        {
            new ObservablePoint(2.2, 5.4),
            new ObservablePoint(4.5, 2.5),
            new ObservablePoint(4.2, 7.4),
            new ObservablePoint(6.4, 9.9),
            new ObservablePoint(4.2, 9.2),
            new ObservablePoint(5.8, 3.5),
            new ObservablePoint(7.3, 5.8),
            new ObservablePoint(8.9, 3.9),
            new ObservablePoint(6.1, 4.6),
            new ObservablePoint(9.4, 7.7),
            new ObservablePoint(8.4, 8.5),
            new ObservablePoint(3.6, 9.6),
            new ObservablePoint(4.4, 6.3),
            new ObservablePoint(5.8, 4.8),
            new ObservablePoint(6.9, 3.4),
            new ObservablePoint(7.6, 1.8),
            new ObservablePoint(8.3, 8.3),
            new ObservablePoint(9.9, 5.2),
            new ObservablePoint(8.1, 4.7),
            new ObservablePoint(7.4, 3.9),
            new ObservablePoint(6.8, 2.3),
            new ObservablePoint(5.3, 7.1),
        }
    }
};</code></pre>

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scattergeometrysize.png)

<pre><code>
var r = new Random();
var values1 = new ObservableCollection&lt;ObservablePoint>();
var values2 = new ObservableCollection&lt;ObservablePoint>();

for (var i = 0; i < 20; i++)
{
    values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
    values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
}

Series = new ISeries[]
{
    new ScatterSeries&lt;ObservablePoint, RectangleGeometry>
    {
        Values = values1,
        GeometrySize = 10, // mark
    },
    new ScatterSeries&lt;ObservablePoint, CircleGeometry>
    {
        Values = values2,
        GeometrySize = 30 // mark
    }
};</code></pre>

## MinGeometrySize property

This property specifies the minimum size a geometry can take when the `Weight` plane is enabled, to enable this plane
you could use the `WeightedPoint` class, the library is ready to plot this instance, alternatively you can register 
a new type using mappers, and use the `TertiaryValue` property of the `ChartPoint` instance to specify
the weight of each point.

Notice in the following image how every shape has a different size, the size of each geometry represents the `Weight` 
of each point, in this case the weight takes a random integer from 0 to 20, so when the `Weight` is `0` the 
size of the geometry will be `15` pixels as specified in the `MinGeometrySize` property, when the `Weight` is `20`
the geometry size will be `40` defined by the `GeometrySize` property, for any `Weight` between this range the library
will interpolate lineally to determine the corresponding size.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scattermgs.png)

<pre><code>var r = new Random();
var values1 = new ObservableCollection&lt;WeightedPoint>();
var values2 = new ObservableCollection&lt;WeightedPoint>();

for (var i = 0; i < 20; i++)
{
    values1.Add(new WeightedPoint(r.Next(0, 20), r.Next(0, 20), r.Next(0, 20)));
    values2.Add(new WeightedPoint(r.Next(0, 20), r.Next(0, 20), r.Next(0, 20)));
}

Series = new ObservableCollection&lt;ISeries>
{
    new ScatterSeries&lt;WeightedPoint, RoundedRectangleGeometry>
    {
        Values = values1,
        GeometrySize = 40,
        MinGeometrySize = 15 // mark
    },

    new ScatterSeries&lt;WeightedPoint, CircleGeometry>
    {
        Values = values2,
        GeometrySize = 40,
        MinGeometrySize = 15 // mark
    }
};</code></pre>

## Plotting custom types

You can teach LiveCharts to plot anything, imagine the case where we have an array of the `City` class defined bellow:

<pre><code>public class City
{
    public string Name { get; set; }
    public double Population { get; set; }
    public double LandArea { get; set; }
}</code></pre>

You can register this type **globally**, this means that every time LiveCharts finds a `City` instance in a chart
it will use the mapper we registered, global mappers are unique for a type, if you need to plot multiple
properties then you should use local mappers.

<pre><code>// Ideally you should call this when your application starts
// If you need help to decide where to add this code
// please see the installation guide in this docs.

// in this case we have an array of the City class
// we need to compare the Population property of every city in our array

LiveCharts.Configure(config =>
    config
        .HasMap&lt;City>((city, point) =>
        {
            // in this lambda function we take an instance of the City class (see city parameter)
            // and the point in the chart for that instance (see point parameter)
            // LiveCharts will call this method for every instance of our City class array,
            // now we need to populate the point coordinates from our City instance to our point

            // in this case we will use the Population property as our primary value (normally the Y coordinate)
            point.PrimaryValue = (float)city.Population;

            // then the secondary value (normally the X coordinate)
            // will be the index of the given dog class in our array
            point.SecondaryValue = point.Context.Index;

            // but you can use another property of the city class as the X coordinate
            // for example lets use the LandArea property to create a plot that compares
            // Population and LandArea in chart:

            // point.SecondaryValue = (float)city.LandArea;

            // OPTIONAL
            // Scatter series supports "weight" for every point
            // The weight creates different geometry sizes for every point
            // based on this value.
            // the sizes of the geometries depend on MinGeometrySize to GeometrySize properties.
            point.TertiaryValue = (float)city.LandArea;
        })
        .HasMap&lt;Foo>(...) // you can register more types here using our fluent syntax
        .HasMap&lt;Bar>(...)
    );</code></pre>

Now we are ready to plot cities all over our application:

<pre><code>Series = new[]
{
    new ScatterSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = (point) => $"{point.Model.Name} population: {point.PrimaryValue:N2}M, area: {point.TertiaryValue}KM2",
        GeometrySize = 35,
        MinGeometrySize = 10,
        Values = new[]
        {
            new City { Name = "Tokyo", Population = 4, LandArea = 3 },
            new City { Name = "New York", Population = 6, LandArea = 4 },
            new City { Name = "Seoul", Population = 2, LandArea = 1 },
            new City { Name = "Moscow", Population = 8, LandArea = 7 },
            new City { Name = "Shanghai", Population = 3, LandArea = 2 },
            new City { Name = "Guadalajara", Population = 4, LandArea = 5 }
        }
    }
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scatterct.png)

Alternatively you could create a **local** mapper that will only work for a specific series, global mappers will be 
ignored when the series `Mapping` property is not null.

<pre><code>var cities = new[]
{
    new City { Name = "Tokyo", Population = 4, LandArea = 3 },
    new City { Name = "New York", Population = 6, LandArea = 4 },
    new City { Name = "Seoul", Population = 2, LandArea = 1 },
    new City { Name = "Moscow", Population = 8, LandArea = 7 },
    new City { Name = "Shanghai", Population = 3, LandArea = 2 },
    new City { Name = "Guadalajara", Population = 4, LandArea = 5 }
};

Series = new[]
{
    // compares Population (Y), LandArea (Y) and Density (weight)
    new LineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = 
            (point) => $"{point.Model.Name} population: {point.PrimaryValue:N2}M, area: {point.SecondaryValue}KM2, density: {point.TertiaryValue:N2}Millions/KM2",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.Population;
            point.SecondaryValue = (float)city.LandArea;
            point.TertiaryValue = (float)(city.Population/city.LandArea);
        }
    }
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scatterctl.png)

## Custom geometries

You can use any geometry to represent a point in a line series.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/scattercustom.png)

<pre><code>var r = new Random();
var values1 = new ObservableCollection&lt;ObservablePoint>();
var values2 = new ObservableCollection&lt;ObservablePoint>();

for (var i = 0; i < 20; i++)
{
    values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
    values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
}

Series = new ObservableCollection&lt;ISeries>
{
    // use the second type argument to specify the geometry to draw for every point
    // there are already many predefined geometries in the
    // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
    new ScatterSeries&lt;ObservablePoint, RoundedRectangleGeometry>
    {
        Values = values1,
        Stroke = null,
        GeometrySize = 40,
    },

    // Or Define your own SVG geometry
    new ScatterSeries&lt;ObservablePoint, MyGeomeometry>
    {
        Values = values2,
        GeometrySize = 40
    }
};</code></pre>

Where `MyGeometry` class is our custom shape, you can draw anything `SkiaSharp` supports at this point,
but in this case we will draw an SVG path, we inherit from `SVGPathGeometry`, and for performance reasons
we use a static variable to parse the SVG path, this ways the parse operation only runs once.

<pre><code>public class MyGeometry : SVGPathGeometry
{
    // the static field is important to prevent the svg path is parsed multiple times // mark
    // Icon from Google Material Icons font.
    // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Amy_location%3A
    public static SKPath svgPath = SKPath.ParseSvgPathData(
        "M12 8c-2.21 0-4 1.79-4 4s1.79 4 4 4 4-1.79 4-4-1.79-4-4-4zm8.94 3c-.46-4.17-3.77-7.48-7.94-7.94V1h-2v2.06C6.83 3.52 3.52 6.83 3.06 " +
        "11H1v2h2.06c.46 4.17 3.77 7.48 7.94 7.94V23h2v-2.06c4.17-.46 7.48-3.77 7.94-7.94H23v-2h-2.06zM12 19c-3.87 0-7-3.13-7-7s3.13-7 7-7 7 " +
        "3.13 7 7-3.13 7-7 7z");

    public MyGeometry()
        : base(svgPath)
    {

    }
}</code></pre>

{{ render this "~/shared/series2.md" }}