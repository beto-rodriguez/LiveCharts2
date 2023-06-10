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

{{ render this "~/shared/series2.md" }}