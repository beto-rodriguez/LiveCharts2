<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## HeatMap property

This property defines the gradient colors, it is an array of [`LvcColor`]({{ website_url }}/api/{{ version }}/LiveChartsCore.Drawing.LvcColor) 
where the first element in the array is the the smallest or coldest and the last item in the array  is the greatest or hottest, 
any value between the chart limits will be interpolated lineally to create a new color, you can add as many colors as you need 
to define the gradient.

![image]({{ assets_url }}/docs/_assets/heathm.png)

<pre><code>using SkiaSharp;
using LiveChartsCore.SkiaSharpView;

Series = new ISeries[]
{
    new HeatSeries&lt;WeightedPoint>
    {
        HeatMap = new[] // mark
        { // mark
            SKColors.Yellow.AsLvcColor(), // the first element is the "coldest" // mark
            SKColors.Black.AsLvcColor(), // mark
            SKColors.Blue.AsLvcColor() // the last element is the "hottest" // mark
        }, // mark
        Values = new ObservableCollection&lt;WeightedPoint> { ... }
    }
};</code></pre>

## ColorStops property

By default all the colors in the `HeatMap` property are separated equidistantly, you can define the distance 
between each color using the `ColorStops` property, it is an array of double, every item in the array must
go from 0 to 1, where 0 is the "coldest" and 1 the "hottest", notice in the following sample how the 
black to blue gradient is only used in the last 10 percent of the gradient, while the yellow to black is
used in the remaining 90% of the gradient.

![image]({{ assets_url }}/docs/_assets/heatcs.png)

<pre><code>using SkiaSharp;
using LiveChartsCore.SkiaSharpView;

Series = new ISeries[]
{
    new HeatSeries&lt;WeightedPoint>
    {
        HeatMap = new[]
        {
            SKColors.Yellow.AsLvcColor(), // the first element is the "coldest" // mark
            SKColors.Black.AsLvcColor(), // mark
            SKColors.Blue.AsLvcColor() // the last element is the "hottest" // mark
        },
        ColorStops = new[] // mark
        { // mark
            0, // mark
            0.9, // mark
            1 // mark
        }, // mark
        Values = new ObservableCollection&lt;WeightedPoint> { ... }
    }
};</code></pre>

## PointPadding property

Defines the padding for every point in the series.

![image]({{ assets_url }}/docs/_assets/heatp.png)

<pre><code>Series = new ISeries[]
{
    new HeatSeries&lt;WeightedPoint>
    {
        PointPadding = new LiveChartsCore.Drawing.Common.Padding(20), // mark
        HeatMap = new[]
        {
            Color.FromArgb(255, 255, 241, 118), // the first element is the "coldest"
            Color.DarkSlateGray,
            Color.Blue // the last element is the "hottest"
        },
        Values = new ObservableCollection&lt;WeightedPoint>
        {
            // Charles
            new WeightedPoint(0, 0, 150), // Jan
            new WeightedPoint(0, 1, 123), // Feb
            new WeightedPoint(0, 2, 310), // Mar
            new WeightedPoint(0, 3, 225), // Apr
            new WeightedPoint(0, 4, 473), // May
            new WeightedPoint(0, 5, 373), // Jun

            // Richard
            new WeightedPoint(1, 0, 432), // Jan
            new WeightedPoint(1, 1, 312), // Feb
            new WeightedPoint(1, 2, 135), // Mar
            new WeightedPoint(1, 3, 78), // Apr
            new WeightedPoint(1, 4, 124), // May
            new WeightedPoint(1, 5, 423), // Jun

            // Ana
            new WeightedPoint(2, 0, 543), // Jan
            new WeightedPoint(2, 1, 134), // Feb
            new WeightedPoint(2, 2, 524), // Mar
            new WeightedPoint(2, 3, 315), // Apr
            new WeightedPoint(2, 4, 145), // May
            new WeightedPoint(2, 5, 80), // Jun

            // Mari
            new WeightedPoint(3, 0, 90), // Jan
            new WeightedPoint(3, 1, 123), // Feb
            new WeightedPoint(3, 2, 70), // Mar
            new WeightedPoint(3, 3, 123), // Apr
            new WeightedPoint(3, 4, 432), // May
            new WeightedPoint(3, 5, 142), // Jun
        }
    }
};

XAxes = new ObservableCollection&lt;Axis>
{
    new Axis
    {
        Labels = new [] { "Charles", "Richard", "Ana", "Mari" }
    }
};

YAxes = new ObservableCollection&lt;Axis>
{
    new Axis
    {
        Labels = new [] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" }
    }
};</code></pre>

## Plotting custom types

You can teach LiveCharts to plot anything, imagine the case where we have an array of the `City` class defined bellow:

<pre><code>public class City
{
    public double Population { get; set; }
    public int Year { get; set; }
    public int CityId { get; set; }
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

            point.PrimaryValue = city.CityId; // city id for the Y axis (primary)
            point.SecondaryValue = city.Year; // the year as the X axis (secondary)

            // finally the population as the tertiary value, for the case of a heat series
            // it could be also called the "Weighted axis", colors for every point will be
            // generated based on this value (Tertiary).
            point.TertiaryValue = (float)city.Population;
        })
        .HasMap&lt;Foo>(...) // you can register more types here using our fluent syntax
        .HasMap&lt;Bar>(...)
    );</code></pre>

Now we are ready to plot cities all over our application:

<pre><code>var cities = new[]
{
    new City { Population = 4.5, CityId = 0, Year = 2019 },
    new City { Population = 5.1, CityId = 0, Year = 2020 },
    new City { Population = 5.8, CityId = 0, Year = 2021 },
    new City { Population = 3.5, CityId = 1, Year = 2019 },
    new City { Population = 4, CityId = 1, Year = 2020 },
    new City { Population = 4.8, CityId = 1, Year = 2021 }
};

XAxes = new[]
{
    new Axis { Labels = new [] { "2019", "2020", "2021" } }
};

YAxes = new[]
{
    new Axis { Labels = new [] { "Tokyo", "Moscow" } }
};

Series = new[]
{
    new HeatSeries&lt;City>
    {
        TooltipLabelFormatter = (point) => $"{YAxes[0].Labels[point.Model.CityId]} {point.Model.Year} {point.TertiaryValue}M",
        Values = cities
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/heatct.png)

Alternatively you could create a **local** mapper that will only work for a specific series, global mappers will be 
ignored when the series `Mapping` property is not null.

<pre><code>Series = new[]
{
    new HeatSeries&lt;City>
    {
        TooltipLabelFormatter = (point) => $"{YAxes[0].Labels[point.Model.CityId]} {point.Model.Year} {point.TertiaryValue}M",
        Mapping = (city, point) =>
        {
            point.PrimaryValue = city.CityId;
            point.SecondaryValue = city.Year;
            point.TertiaryValue = (float)city.Population;
        },
        Values = cities
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}