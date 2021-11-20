# The Polar Chart Control

:::info
This article is a quick guide to use the `PolarChart` control, you can explore all the properties and the source code 
at the [ApiExplorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.{{ platform }}.PolarChart).
:::

The `PolarChart` control is a 'ready to go' control to create plots using the 
[Polar coordinate system](https://en.wikipedia.org/wiki/Polar_coordinate_system),
To get started all you need to do is assign the `Series` property with a collection of 
[`IPolarSeries`]({{ website_url }}/api/{{ version }}/LiveChartsCore.Kernel.Sketches.IPolarSeries-1).

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Polar.Basic
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; } = new[]
        {
            new PolarLineSeries&lt;double>
            {
                Values = new double[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
                Fill = null,
                IsClosed = false
            }
        };
    }
}</code></pre>

<pre><code>&lt;lvc:PolarChart
    Series="{Binding Series}">
&lt;/lvc:PolarChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PolarChart
    Series="series">
&lt;/PolarChart></code></pre>

<pre><code>private ISeries[] series = new[]
{
    new PolarLineSeries&lt;double>
    {
        Values = new double[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
        Fill = null,
        IsClosed = false
    }
};</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PolarChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>PolarChart1.Series = new[]
{
    new PolarLineSeries&lt;double>
    {
        Values = new double[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
        Fill = null,
        IsClosed = false
    }
};</code></pre>
{{~ end ~}}

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/polar-mvp.png)

The main components of this control are:

- Series
- Axes
- Tooltip
- Legend

But notice in the previous code snippet we did not specify the Axes, Tooltip, Legend or the series colors, this is because LiveCharts will
decide them based on the current theme of the library, you can also customize any of these properties when you require it, this article will
cover the most common scenarios.

## Series

There are multiple series available in the library, you can add one or mix them all in the same chart, every series has unique properties,
any image bellow is a link to an article explaining more about them.

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Polar%20Line%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/polarLines/basic/result.png" alt="series"/>
<div class="text-center"><b>Polar Line series</b></div>
</div>
</a>

