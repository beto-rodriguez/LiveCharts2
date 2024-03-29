<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/polarlabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/polarlinestroke.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 1 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

The alpha channel enables transparency, it goes from 0 to 255, 0 is transparent and 255 disables transparency completely.

![image]({{ assets_url }}/docs/_assets/polarlinefill.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Fill = new SolidColorPaint(SKColors.Green.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## GeometryFill and GeometryStroke properties

The geometry is the circle shape (by default) that the line series draws for every point, you can customize
the fill and stroke of this shape, if none of these properties are set then LiveCharts will create them based on 
the series position in your series collection and the current theme.

![image]({{ assets_url }}/docs/_assets/polarlinegsf.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 },
        Fill = null,
        GeometryFill = new SolidColorPaint(SKColors.AliceBlue), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 4 } // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 },
        Fill = null,
        GeometryFill = new SolidColorPaint(SKColors.IndianRed), // mark
        GeometryStroke = new SolidColorPaint(SKColors.DarkSalmon) { StrokeThickness = 8 } // mark
    }
};</code></pre>

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image]({{ assets_url }}/docs/_assets/polarlinegs.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GeometrySize = 10 // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        GeometrySize = 30 // mark
    }
};</code></pre>

## LineSmoothness property

Determines if the series line is straight or curved, this property is of type `double` and goes from `0` to `1` any other
value will be ignored, where 0 is straight and 1 is the most curved line.

![image]({{ assets_url }}/docs/_assets/polarlinesmothness.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 5, 4, 7, 3, 8 },
        LineSmoothness = 0 // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 2, 6, 2, 6 },
        LineSmoothness = 1 // mark
    }
};</code></pre>

## EnableNullSplitting property

This property is enabled by default (`true`), it has a performance cost and allows the series to create gaps, when the
series finds a `null` instance then the series will create a gap.

![image]({{ assets_url }}/docs/_assets/polarlinesnullsplit.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int?>
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
        LineSmoothness = 0,
        IsClosed = false
    }
};</code></pre>

{{ render this "~/shared/series2.md" }}