<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/stackedcolstroke.png)

<pre><code>Series = new ISeries[]
{
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 3, 5, 3, 2, 5, 4, 2 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4 }, // mark
        Fill = null,
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 2, 3, 2, 3, 4, 2 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 8 }, // mark
        Fill = null,
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 6, 6, 5, 4, 3 , 2 },
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 12 }, // mark
        Fill = null,
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/stackedcolfill.png)

<pre><code>Series = new ISeries[]
{
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 3, 5, 3, 2, 5, 4, 2 },
        Fill = new SolidColorPaint(SKColors.Red), // mark
        Stroke = null,
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 2, 3, 2, 3, 4, 2 },
        Fill = new SolidColorPaint(SKColors.Blue), // mark
        Stroke = null,
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 6, 6, 5, 4, 3 , 2 },
        Fill = new SolidColorPaint(SKColors.Green), // mark
        Stroke = null,
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Rx and Ry properties

These properties define the corners radius in the rectangle geometry.

![image]({{ assets_url }}/docs/_assets/stackedcolcr.png)

<pre><code>Series = new ISeries[]
{
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 3, 5, 3, 2, 5, 4, 2 },
        Rx = 50, // mark
        Ry = 50 // mark
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 2, 3, 2, 3, 4, 2 },
        Rx = 50, // mark
        Ry = 50 // mark
    },
    new StackedColumnSeries&lt;int>
    {
        Values = new List&lt;int> { 4, 6, 6, 5, 4, 3 , 2 },
        Rx = 50, // mark
        Ry = 50 // mark
    }
};</code></pre>

## MaxBarWidth property

:::info
this section uses the `ColumnSeries` class, but it works the same for the `StackedColumnSeries`.
:::

Specifies the maximum width a column can take, take a look at the following sample, where the max width is `10`.

![image]({{ assets_url }}/docs/_assets/columnmw10.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        MaxBarWidth = 10 // mark
    }
};</code></pre>

But now lets use `double.MaxValue` to see the difference.

![image]({{ assets_url }}/docs/_assets/columnmwmax.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        MaxBarWidth = double.MaxValue // mark
    }
};</code></pre>

Finally we could aso set the padding to `0`.

![image]({{ assets_url }}/docs/_assets/columnmwmaxp0.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        MaxBarWidth = double.MaxValue,
        GroupPadding = 0 // mark
    }
};</code></pre>

## GroupPadding property

:::info
this section uses the `ColumnSeries` class, but it works the same for the `StackedColumnSeries`.
:::

Defines the distance between every group of columns in the plot, a group of columns is all the column that share the same
secondary value coordinate, in the following image there are 5 groups of columns, the first one the columns that share the 
`0` coordinate, the second one shares the `1`, the third group shares the `2` coordinate, the forth group shares the `3` coordinate,
finally the fifth group shares the `4` coordinate.

![image]({{ assets_url }}/docs/_assets/columngp.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GroupPadding = 50 // mark
    },
    new ColumnSeries&lt;int>
    {
        Values = new [] { 2, 3,1, 4, 6 },
        GroupPadding = 50 // mark
    },
    new ColumnSeries&lt;int>
    {
        Values = new [] { 6, 3, 6, 9, 4 },
        GroupPadding = 50 // mark
    }
};</code></pre>

## Plotting custom types

:::info
this sample uses the ColumnSeries class, notice StackedLColumnSeries inherits from ColumnSeries, this sample also applies for the StackedColumnSeries class.
:::

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
            // will be the index of city in our cities array
            point.SecondaryValue = point.Context.Index;

            // but you can use another property of the city class as the X coordinate
            // for example lets use the LandArea property to create a plot that compares
            // Population and LandArea in chart:

            // point.SecondaryValue = (float)city.LandArea;
        })
        .HasMap&lt;Foo>(...) // you can register more types here using our fluent syntax
        .HasMap&lt;Bar>(...)
    );</code></pre>

Now we are ready to plot cities all over our application:

<pre><code>Series = new[]
{
    new ColumnSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = point => $"{point.Model.Name} {point.PrimaryValue:N2}M",
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

![image]({{ assets_url }}/docs/_assets/columnct.png)

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
    // this series draws the Population property in the Y axis
    new ColumnSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = (point) => $"{point.Model.Name} population: {point.PrimaryValue:N2}M",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.Population;
            point.SecondaryValue = point.Context.Index;
        }
    },

    // draws the LandArea property in the Y axis
    new ColumnSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = (point) => $"{point.Model.Name} area: {point.PrimaryValue:N2}KM2",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.LandArea;
            point.SecondaryValue = point.Context.Index;
        }
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/columnctl.png)

## Custom geometries

:::info
this sample uses the ColumnSeries class, notice StackedLColumnSeries inherits from ColumnSeries, this sample also applies for the StackedColumnSeries class.
:::

You can use any geometry to represent a point in a line series.

![image]({{ assets_url }}/docs/_assets/barscustom.png)

<pre><code>Series = new List&lt;ISeries>
{
    // use the second type argument to specify the geometry to draw for every point
    // there are already many predefined geometries in the
    // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
    new ColumnSeries&lt;double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.OvalGeometry>
    {
        Values = new List&lt;double> { 4, 2, 0, 5, 2, 6 },
        Fill = new SolidColorPaint(SKColors.CornflowerBlue)
    },

    // you can also define your own geometry using SVG
    new ColumnSeries&lt;double, MyGeometry>
    {
        Values = new List&lt;double> { 3, 2, 3, 4, 5, 3 },
        Stroke = null,
        Fill = new SolidColorPaint(SKColors.Coral, 5)
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