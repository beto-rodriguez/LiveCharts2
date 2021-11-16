<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedareastroke.png)

<pre><code>Series = new ISeries[]
{
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaintTask(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaintTask(SKColors.Red) { StrokeThickness = 8 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Stroke = new SolidColorPaintTask(SKColors.Green) { StrokeThickness = 1 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

The alpha channel enables transparency, it goes from 0 to 255, 0 is transparent and 255 disables transparency completely.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedareafill.png)

<pre><code>Series = new ISeries[]
{
    new LineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Fill = new SolidColorPaintTask(SKColors.Blue.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new LineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Fill = new SolidColorPaintTask(SKColors.Red.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new LineSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Fill = new SolidColorPaintTask(SKColors.Green.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## GeometryFill and GeometryStroke properties

The geometry is the circle shape (by default) that the line series draws for every point, byt by default the size is `0` 
you can customize the fill and stroke of this shape, if none of these properties are set then LiveCharts will create them 
based on the series position in your series collection and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedareageometrystrokefill.png)

<pre><code>Series = new ISeries[]
{
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaintTask(SKColors.Red) { StrokeThickness = 3 }, // mark
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaintTask(SKColors.Green) { StrokeThickness = 6 }, // mark
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaintTask(SKColors.Blue) { StrokeThickness = 10 }, // mark
    }
};</code></pre>

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedareags.png)

<pre><code>Series = new ISeries[]
{
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GeometrySize = 10, // mark
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke),
        GeometryStroke = new SolidColorPaintTask(SKColors.Red),
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        GeometrySize = 20, // mark
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke),
        GeometryStroke = new SolidColorPaintTask(SKColors.Green),
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        GeometrySize = 30, // mark
        GeometryFill = new SolidColorPaintTask(SKColors.WhiteSmoke),
        GeometryStroke = new SolidColorPaintTask(SKColors.Blue),
    }
};</code></pre>

## LineSmoothness property

Determines if the series line is straight or curved, this property is of type `double` and goes from `0` to `1` any other
value will be ignored, where 0 is straight and 1 is the most curved line.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/linesmothness.png)

<pre><code>Series = new ISeries[]
{
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        LineSmoothness = 0, // mark
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        LineSmoothness = 0.5 // mark
    },
    new StackedAreaSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        LineSmoothness = 1 // mark
    }
};</code></pre>

## Plotting custom types

:::info
this sample uses the LineSeries class, notice StackedLineSeries inherits from LineSeries, this sample also applies for the StackedLineSeries class.
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
    new LineSeries&lt;City>
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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/linect.png)

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
    new LineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = point => $"{point.Model.Name} population: {point.PrimaryValue:N2}M",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.Population;
            point.SecondaryValue = point.Context.Index;
        }
    },

    // draws the LandArea property in the Y axis
    new LineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = (point) => $"{point.Model.Name} area: {point.PrimaryValue:N2}KM2",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.LandArea;
            point.SecondaryValue = point.Context.Index;
        }
    },

    // compares Population (Y) and LandArea (Y)
    //new LineSeries&lt;City>
    //{
    //    Name = "Population",
    //    TooltipLabelFormatter = (point) => $"{point.Model.Name} population: {point.PrimaryValue:N2}M, area: {point.SecondaryValue}KM2",
    //    Values = cities,
    //    Mapping = (city, point) =>
    //    {
    //        point.PrimaryValue = (float)city.Population;
    //        point.SecondaryValue = (float)city.LandArea;
    //    }
    //}
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/linectl.png)

## EnableNullSplitting property

Even the property is visible, this feature is not supported by now.

## Custom geometries

You can use any geometry to represent a point in a line series, if you want this feature, please check the sample in the 
[line series article]({{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Line%20Series).

{{ render this "~/shared/series2.md" }}