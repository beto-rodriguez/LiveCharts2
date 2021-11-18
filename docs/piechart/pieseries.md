<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/polarlabels.md" }}

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

{{ render this "~/shared/series2.md" }}