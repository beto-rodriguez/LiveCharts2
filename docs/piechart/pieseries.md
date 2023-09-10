<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/polarlabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/piestroke.png)

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.Basic
{
    public class ViewModel
    {
        public IEnumerable&lt;ISeries> Series { get; set; } = new List&lt;ISeries>
        {
            new PieSeries&lt;double>
            {
                Values = new List&lt;double> { 4 },
                Pushout = 8,
                Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 } // mark
            },
            new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
        };
    }
}</code></pre>

<pre><code>&lt;lvc:PieChart Series="{Binding Series}">&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using SkiaSharp;
@using System.Collections.Generic;

&lt;PieChart Series="series">&lt;/PieChart>

@code {
    private ISeries[] series 
        = new List&lt;ISeries>
        {
            new PieSeries&lt;double>
            {
                Values = new List&lt;double> { 4 },
                Pushout = 8,
                Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 } // mark
            },
            new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
        };
}</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.Series = new List&lt;ISeries>
{
    new PieSeries&lt;double>
    {
        Values = new List&lt;double> { 4 },
        Pushout = 8,
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 } // mark
    },
    new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
};;</code></pre>
{{~ end ~}}

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/piefill.png)

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.Basic
{
    public class ViewModel
    {
        public IEnumerable&lt;ISeries> Series { get; set; } = new List&lt;ISeries>
        {
            new PieSeries&lt;double>
            {
                Values = new List&lt;double> { 4 },
                Pushout = 8,
                Fill = new SolidColorPaint(SKColors.Yellow) // mark
            },
            new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
        };
    }
}</code></pre>

<pre><code>&lt;lvc:PieChart Series="{Binding Series}">&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using SkiaSharp;
@using System.Collections.Generic;

&lt;PieChart Series="series">&lt;/PieChart>

@code {
    private ISeries[] series 
        = new List&lt;ISeries>
        {
            new PieSeries&lt;double>
            {
                Values = new List&lt;double> { 4 },
                Pushout = 8,
                Fill = new SolidColorPaint(SKColors.Yellow) // mark
            },
            new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
            new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
        };
}</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.Series = new List&lt;ISeries>
{
    new PieSeries&lt;double>
    {
        Values = new List&lt;double> { 4 },
        Pushout = 8,
        Fill = new SolidColorPaint(SKColors.Yellow) // mark
    },
    new PieSeries&lt;double> { Values = new List&lt;double> { 2 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 1 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 4 } },
    new PieSeries&lt;double> { Values = new List&lt;double> { 3 } }
};;</code></pre>
{{~ end ~}}

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Pushout property

It is the distance in pixels between the center of the control and the pie slice, notice the 
`HoverPushout` property defines the push-out when the pointer is above the pie slice shape.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    Pushout = 40 // mark
};</code></pre>

![image]({{ assets_url }}/docs/_assets/piepushout.png)

## MaxRadialColumnWidth

Sets the maximum value a radial column can take in pixels.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    MaxRadialColumnWidth = 50 // mark
};</code></pre>

![image]({{ assets_url }}/docs/_assets/pieMaxRadialCW.png)

## InnerRadius property

The inner radius of the slice in pixels, it is similar to the `MaxRadialColumnWidth` property,
both are useful to create doughnut charts, the difference is that `MaxRadialColumnWidth` is more flexible
on different screen sizes.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    InnerRadius = 50 // mark
};</code></pre>

![image]({{ assets_url }}/docs/_assets/pieInnerRadius.png)

## OuterRadiusOffset property

It is the distance from the maximum radius available to the end of the slice in pixels.

<pre><code>var pieSeries = new PieSeries&lt;int>
{
    Values = new [] { ... },
    OuterRadiusOffset = 20 // mark
};</code></pre>

![image]({{ assets_url }}/docs/_assets/pieOuterRadiusOffset.png)

{{ render this "~/shared/series2.md" }}
