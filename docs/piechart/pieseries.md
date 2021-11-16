<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

## Pushout property

It is the distance in pixels between the center of the control and the pie slice, notice the 
`HoverPushout` property defines the push-out when the pointer is above the pie slice shape.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    Pushout = 40 // mark
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/piepushout.png)

## InnerRadius property

The inner radius of the slice in pixels.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    InnerRadius = 50 // mark 
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/pieInnerRadius.png)

## MaxOuterRadius property

Specifies the max radius (in percentage) the slice can take, the value goes from 0 to 1, where 1 is the full available radius and 0 is none, default is 1.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    MaxOuterRadius = 0.8 // mark
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/piemaxoutter.png)

## DataLabels

Data labels are labels for every point in a series, there are multiple properties to customize them, take a look at the 
following sample:

<pre><code>Series { get; set; } = new List&lt;ISeries>
{
    new PieSeries&lt;double>
    {
        Values = new List&lt;double> { 8 },
        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
        DataLabelsSize = 22,
        // for more information about available positions see:
        // {{ website_url }}/api/{{ version }}/LiveChartsCore.Measure.PolarLabelsPosition
        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
        DataLabelsFormatter = point => point.PrimaryValue.ToString("N2") + " elements"
    },
    new PieSeries&lt;double>
    {
        Values = new List&lt;double> { 6 },
        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
        DataLabelsSize = 22,
        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
        DataLabelsFormatter = point => point.PrimaryValue.ToString("N2") + " elements"
    },
    new PieSeries&lt;double>
    {
        Values = new List&lt;double> { 4 },
        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
        DataLabelsSize = 22,
        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
        DataLabelsFormatter = point => point.PrimaryValue.ToString("N2") + " elements"
    }
};</code></pre>

The series above result in the following chart:

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/pielabels.png)

You can also use the `DataLabelsRotation` property to set an angle in degrees for the labels in the chart,
notice the constants `LiveCharts.CotangentAngle` and `LiveCharts.TangentAngle` to build labels rotation.

This is the result when we set all the series to `LiveCharts.CotangentAngle`:

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/pielabelscotan.png)

And this is the result when we set all the series to `LiveCharts.TangentAngle`:

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/pielabelstan.png)

Finally you can also combine tangent and cotangent angles with decimal degrees:

<pre><code>Series { get; set; } = new List&lt;ISeries>
{
    new PieSeries&lt;double>
    {
        DataLabelsRotation = 30, // in degrees
    },
    new PieSeries&lt;double>
    {
        DataLabelsRotation = LiveCharts.TangentAngle + 30, // the tangent + 30 degrees
    },
    new PieSeries&lt;double>
    {
        DataLabelsRotation = LiveCharts.CotangentAngle + 30, // the cotangent + 30 degrees
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}