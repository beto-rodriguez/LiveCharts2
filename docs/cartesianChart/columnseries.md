<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/columnstroke.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
    },
    new ColumnSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 }, // mark
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

![image]({{ assets_url }}/docs/_assets/columnfill.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Fill = new SolidColorPaint(SKColors.Blue), // mark
        Stroke = null
    },
    new ColumnSeries<int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Fill = new SolidColorPaint(SKColors.Red), // mark
        Stroke = null
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Rx and Ry properties

These properties define the corners radius in the rectangle geometry.

![image]({{ assets_url }}/docs/_assets/columnr.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Rx = 50, // mark
        Ry = 50 // mark
    }
};</code></pre>

## MaxBarWidth property

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

Defines the distance between every group of columns in the plot, a group of columns is all the column that share the same
secondary value coordinate, in the following image there are 5 groups of columns, the first one the columns that share the 
`0` coordinate, the second one shares the `1`, the third group shares the `2` coordinate, the forth group shares the `3` coordinate,
finally the fifth group shares the `4` coordinate.

:::info
To highlight this feature the following code uses the ColumnSeries class, but it works the same for the StackedColumnSeries
notice the sample above is using the GroupPadding property also.
:::

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

## IgnoresBarPosition property

The ignores bar position property let the series ignore all the other bar series in the same coordinate, this is useful
to create backgrounds for columns, take a look at the following sample:

![image]({{ assets_url }}/docs/_assets/columnbg.png)

<pre><code>Series = new ISeries[]
{
    new ColumnSeries&lt;double>
    {
        Values = new ObservableCollection&lt;double> { 10, 10, 10, 10, 10, 10, 10 },
        Stroke = null,
        Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
        IgnoresBarPosition = true // mark
    },
    new ColumnSeries&lt;double>
    {
        Values = new ObservableCollection&lt;double> { 3, 10, 5, 3, 7, 3, 8 },
        Stroke = null,
        Fill = new SolidColorPaint(SKColors.CornflowerBlue),
        IgnoresBarPosition = true // mark
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}