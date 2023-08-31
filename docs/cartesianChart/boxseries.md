<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/box-stroke.png)

<pre><code>Series = new ISeries[]
{
    new BoxSeries&lt;BoxValue>
    {
        Values = new BoxValue[]
        {
            // max, upper quartile, lower quartile, min, median
            new(100, 80, 60, 20, 70),
            new(90, 70, 50, 30, 60),
            new(80, 60, 40, 10, 50)
        },
        Stroke = new SolidColorPaint(SKColors.Red, 3),
        Fill = null
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/box-fill.png)

<pre><code>Series = new ISeries[]
{
    new BoxSeries&lt;BoxValue>
    {
        Values = new BoxValue[]
        {
            // max, upper quartile, lower quartile, min, median
            new(100, 80, 60, 20, 70),
            new(90, 70, 50, 30, 60),
            new(80, 60, 40, 10, 50)
        },
        Fill = new SolidColorPaint(SKColors.Red)
    }
};</code></pre>
:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## MaxBarWidth property

Specifies the maximum width a column can take, take a look at the following sample, where the max width is `10`.

![image]({{ assets_url }}/docs/_assets/box-mw10.png)

<pre><code>Series = new ISeries[]
{
    new BoxSeries&lt;BoxValue>
    {
        Values = new BoxValue[] { ... },
        MaxBarWidth = 10 // mark
    }
};</code></pre>

But now lets use `double.MaxValue` to see the difference.

![image]({{ assets_url }}/docs/_assets/box-mw-.png)

<pre><code>Series = new ISeries[]
{
    new BoxSeries&lt;BoxValue>
    {
        Values = new BoxValue[] { ... },
        MaxBarWidth = double.MaxValue // mark
    }
};</code></pre>

Finally we could aso set the padding to `0`.

![image]({{ assets_url }}/docs/_assets/box-mw-p0.png)

<pre><code>Series = new ISeries[]
{
    new BoxSeries&lt;BoxValue>
    {
        Values = new BoxValue[] { ... },
        MaxBarWidth = double.MaxValue,
        Padding = 0 // mark
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}
