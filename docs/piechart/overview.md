<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# The Pie Chart Control

:::info
This article is a quick guide to use the `PieChart` control, you can explore all the properties and the source code 
at the [ApiExplorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.{{ platform }}.PieChart).
:::

The `PieChart` control can build Pie, Doughnut and gauges charts, this article will cover only Pie and Doughnut charts,
if you need to know more about gauges please read 
[this guide]({{ website_url }}/docs/{{ platform }}/{{ version }}/PieChart.Gauges).

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Basic
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new PieSeries&lt;double> { Values = new double[] { 2 } },
                new PieSeries&lt;double> { Values = new double[] { 4 } },
                new PieSeries&lt;double> { Values = new double[] { 1 } },
                new PieSeries&lt;double> { Values = new double[] { 4 } },
                new PieSeries&lt;double> { Values = new double[] { 3 } }
            };
    }
}</code></pre>

<pre><code>&lt;lvc:PieChart
    Series="{Binding Series}">
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    Series="series">
&lt;/PieChart></code></pre>

<pre><code>private ISeries[] series = = new ISeries[]
{
    new PieSeries&lt;double> { Values = new double[] { 2 } },
    new PieSeries&lt;double> { Values = new double[] { 4 } },
    new PieSeries&lt;double> { Values = new double[] { 1 } },
    new PieSeries&lt;double> { Values = new double[] { 4 } },
    new PieSeries&lt;double> { Values = new double[] { 3 } }
};</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PieChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>PieChart1.Series = new ISeries[]
{
    new PieSeries&lt;double> { Values = new double[] { 2 } },
    new PieSeries&lt;double> { Values = new double[] { 4 } },
    new PieSeries&lt;double> { Values = new double[] { 1 } },
    new PieSeries&lt;double> { Values = new double[] { 4 } },
    new PieSeries&lt;double> { Values = new double[] { 3 } }
};</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/piemvp.png)

## InitialRotation property

Controls the angle in degrees where the first slice is drawn, the `InitialRotation` property will change the start angle of
the pie, the following diagram explains where the `PieChart` rotation starts:

![image]({{ assets_url }}/docs/_assets/pie-rotation.png)

{{~ if xaml ~}}
<pre><code>&lt;lvc:PieChart
    InitialRotation="-90">
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    InitialRotation="-90">
&lt;/PieChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PieChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>PieChart1.InitialRotation = -90;</code></pre>
{{~ end ~}}

Notice a change in the `InitialRotation` property is animated automatically based on the chart animations settings:

![image]({{ assets_url }}/docs/_assets/pie-inrot.gif)

## MaxAngle property

This property determines the complete angle in degrees of the chart, the default value is 360.

{{~ if xaml ~}}
<pre><code>&lt;lvc:PieChart
    MaxAngle="270">
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    MaxAngle="270">
&lt;/PieChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PieChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>PieChart1.MaxAngle = 270;</code></pre>
{{~ end ~}}

Notice the `MaxAngle` property is animated automatically based on the chart animations settings:

![image]({{ assets_url }}/docs/_assets/pie-maxangle.gif)

{{ render this "~/shared/chart.md" }}

{{ render this "~/shared/tooltips.md" }}

{{ render this "~/shared/legends.md" }}
