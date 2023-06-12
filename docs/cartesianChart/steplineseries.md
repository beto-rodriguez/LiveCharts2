<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/stepstroke.png)

<pre><code>Series = new ISeries[]
{
    new StepLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new StepLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 4, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

The alpha channel enables transparency, it goes from 0 to 255, 0 is transparent and 255 disables transparency completely.

![image]({{ assets_url }}/docs/_assets/stepfill.png)

<pre><code>
Series = new ISeries[]
{
    new StepLineSeries&lt;int>
    {
        Values = new [] { 4, 3, 7, 2, 8 },
        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## GeometryFill and GeometryStroke properties

The geometry is the circle shape (by default) that the line series draws for every point, you can customize
the fill and stroke of this shape, if none of these properties are set then LiveCharts will create them based on 
the series position in your series collection and the current theme.

![image]({{ assets_url }}/docs/_assets/stepgeometrystrokefill.png)

<pre><code>Series = new ISeries[]
{
    new StepLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 },
        Fill = null,
        GeometryFill = new SolidColorPaint(SKColors.AliceBlue), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 4 } // mark
    }
};</code></pre>

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image]({{ assets_url }}/docs/_assets/stepgeometrysize.png)

<pre><code>Series = new ISeries[]
{
    new StepLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GeometrySize = 10 // mark
    },
    new StepLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        GeometrySize = 30 // mark
    }
};</code></pre>

## EnableNullSplitting property

This property is enabled by default (`true`), it has a performance cost and allows the series to create gaps, when the
series finds a `null` instance then the series will create a gap.

![image]({{ assets_url }}/docs/_assets/stepnullsplit.png)

<pre><code>Series = new ISeries[]
{
    new StepLineSeries&lt;int?>
    {
        Values = new int?[] 
        { 
            5, 
            4, 
            2, 
            null, // mark
            3, 
            8, 
            6 
        },
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}