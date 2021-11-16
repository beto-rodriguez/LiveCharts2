<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedstepstroke.png)

<pre><code>Series = new ISeries[]
{
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 3, 2, 3, 5, 3, 4, 6 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 6, 5, 6, 3, 8, 5, 2 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 }, // mark
        Fill = null
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 4, 8, 2, 8, 9, 5, 3 },
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 12 }, // mark
        Fill = null
    }
};</code></pre>

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

The alpha channel enables transparency, it goes from 0 to 255, 0 is transparent and 255 disables transparency completely.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedstepfill.png)

<pre><code>Series = new ISeries[]
{
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 3, 2, 3, 5, 3, 4, 6 },
        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90)), // mark
        Stroke = null
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 6, 5, 6, 3, 8, 5, 2 },
        Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90)), // mark
        Stroke = null
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List<double> { 4, 8, 2, 8, 9, 5, 3 },
        Fill = new SolidColorPaint(SKColors.Green.WithAlpha(90)), // mark
        Stroke = null
    }
};</code></pre>

## GeometryFill and GeometryStroke properties

The geometry is the circle shape (by default) that the line series draws for every point, you can customize
the fill and stroke of this shape, if none of these properties are set then LiveCharts will create them based on 
the series position in your series collection and the current theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedstepgs.png)

<pre><code>Series = new ISeries[]
{
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 3, 2, 3, 5, 3, 4, 6 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 }, // mark
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 6, 5, 6, 3, 8, 5, 2 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 6 }, // mark
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 4, 8, 2, 8, 9, 5, 3 },
        GeometrySize = 20,
        GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 10 }, // mark
    }
};</code></pre>

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stackedstepgss.png)

<pre><code>Series = new ISeries[]
{
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 3, 2, 3, 5, 3, 4, 6 },
        GeometryFill = new SolidColorPaint(SKColors.Black),
        GeometrySize = 10, // mark
    },
    new StackedStepAreaSeries&lt;double>
    {
        Values = new List&lt;double> { 6, 5, 6, 3, 8, 5, 2 },
        GeometryFill = new SolidColorPaint(SKColors.Black),
        GeometrySize = 20, // mark
    }
};</code></pre>

## Plotting custom types

:::info
this sample uses the StepLineSeries class, notice StackedStepAreaSeries inherits from StepLineSeries, this sample also applies for the StackedLineSeries class.
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
    new StepLineSeries&lt;City>
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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stepct.png)

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
    new StepLineSeries&lt;City>
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
    new StepLineSeries&lt;City>
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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/stepctl.png)

## EnableNullSplitting property

Even the property is visible, this feature is not supported by now.

## Custom geometries

You can use any geometry to represent a point in a line series, if you want this feature, please check the sample in the 
step line series article.