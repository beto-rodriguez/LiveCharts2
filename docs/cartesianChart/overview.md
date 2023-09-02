<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Cartesian chart control

:::info
This article is a quick guide to use the `CartesianChart` control, you can explore all the properties and the source code 
at the [ApiExplorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.{{ platform }}.CartesianChart).
:::

The `CartesianChart` control is a 'ready to go' control to create plots using the 
[Cartesian coordinate system](https://en.wikipedia.org/wiki/Cartesian_coordinate_system),
To get started all you need to do is assign the `Series` property with a collection of 
[`ICartesianSeries`]({{ website_url }}/api/{{ version }}/LiveChartsCore.Kernel.Sketches.ICartesianSeries-1).

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Bars.Basic
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new LineSeries&lt;int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
                new ColumnSeries&lt;double>
                {
                    Values = new double[] { 2, 5, 4, -2, 4, -3, 5 }
                }
            };
    }
}</code></pre>

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series">
&lt;/CartesianChart></code></pre>

<pre><code>private ISeries[] series = new ISeries[]
{
    new LineSeries&lt;int>
    {
        Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
    },
    new ColumnSeries&lt;double>
    {
        Values = new double[] { 2, 5, 4, -2, 4, -3, 5 }
    }
};</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
Drag a new `CartesianChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>cartesianChart1.Series = new ISeries[]
{
    new LineSeries&lt;int>
    {
        Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
    },
    new ColumnSeries&lt;double>
    {
        Values = new double[] { 2, 5, 4, -2, 4, -3, 5 }
    }
};</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/cc-mvp.png)

The main components of this control are:

- Series
- Axes
- Tooltip
- Legend

![image]({{ assets_url }}/docs/_assets/chart-overview.png)

But notice in the previous code snippet we did not specify the Axes, Tooltip, Legend or the series colors, this is because LiveCharts will
decide them based on the current theme of the library, you can also customize any of these properties when you require it, this article will
cover the most common scenarios.

## Series

There are multiple series available in the library, you can add one or mix them all in the same chart, every series has unique properties,
any image bellow is a link to an article explaining more about them.

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Line%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/lines/basic/result.png" alt="series"/>
<div class="text-center"><b>Line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Column%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/bars/basic/result.png"  alt="series"/>
<div class="text-center"><b>Column series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Scatter%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/scatter/basic/result.png"  alt="series"/>
<div class="text-center"><b>Scatter series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Step%20line%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/steplines/basic/result.png" alt="series"/>
<div class="text-center"><b>Step line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Heat%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/heat/basic/result.png" alt="series"/>
<div class="text-center"><b>Heat series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Candle%20Sticks%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/financial/basicCandlesticks/result.png" alt="series"/>
<div class="text-center"><b>Candle sticks series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Line%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/stackedArea/basic/result.png" alt="series"/>
<div class="text-center"><b>Stacked line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Column%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/stackedBars/basic/result.png" alt="series"/>
<div class="text-center"><b>Stacked column series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Step%20Line%20Series%20props">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/stackedArea/stepArea/result.png" alt="series"/>
<div class="text-center"><b>Stacked step line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/samples.box.basic">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/box/basic/result.png" alt="series"/>
<div class="text-center"><b>Box series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/samples.error.basic">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/error/basic/result.png" alt="series"/>
<div class="text-center"><b>Error series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/samples.scatter.bubbles">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/scatter/bubbles/result.png" alt="series"/>
<div class="text-center"><b>Bubble series</b></div>
</div>
</a>

## Axes

A `CartesianChart` has 2 axes, the `YAxis` and the `XAxis` properties, an [IAxis]({{ website_url }}/api/{{ version}}/LiveChartsCore.Kernel.Sketches.ICartesianAxis) 
besides de visual customization such as labels format and separators paints it also controls multiple important features such as 
the **Zooming**, the **Panning**, the **Scale** (log) and the **Paging**.

This is a brief sample about how to use the main features of the `Axis` class, you can find a more detailed article at the button below or at the 
[API explorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.Axis).

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
<b>Go to the full axes article</b>
</a>

## Axes.SeparatorsStyle

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Basic
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; }
            = new ISeries[] { new ColumnSeries&lt;int> { Values = new[] { 2, 5, 4, -2, 4, -3, 5 } } };

        public Axis[] XAxes { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "X Axis",
                    NamePaint = new SolidColorPaint(SKColors.Black), // mark

                    LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark
                    TextSize = 10,

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }  // mark
                }
            };

        public Axis[] YAxes { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "Y Axis",
                    NamePaint = new SolidColorPaint(SKColors.Red), // mark

                    LabelsPaint = new SolidColorPaint(SKColors.Green), // mark
                    TextSize = 20,

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) // mark
                    { // mark
                        StrokeThickness = 2, // mark
                        PathEffect = new DashEffect(new float[] { 3, 3 }) // mark
                    } // mark
                }
            };
    }
}</code></pre>

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    XAxes="{Binding XAxes}"
    YAxes="{Binding YAxes}">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    XAxes="xAxes"
    YAxes="yAxes">
&lt;/CartesianChart></code></pre>

<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using LiveChartsCore.SkiaSharpView.Painting.Effects;
@using SkiaSharp;

private ISeries[] series { get; set; }
    = new ISeries[] { new ColumnSeries&lt;int> { Values = new[] { 2, 5, 4, -2, 4, -3, 5 } } };

private Axis[] xAxes { get; set; }
    = new Axis[]
    {
        new Axis
        {
            Name = "X Axis",
            NamePaint = new SolidColorPaint(SKColors.Black), // mark

            LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark
            TextSize = 10,

            SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 } // mark
        }
    };

private Axis[] yAxes { get; set; }
    = new Axis[]
    {
        new Axis
        {
            Name = "Y Axis",
            NamePaint = new SolidColorPaint(SKColors.Red), // mark

            LabelsPaint = new SolidColorPaint(SKColors.Green), // mark
            TextSize = 20,

            SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) // mark
            { // mark
                StrokeThickness = 2, // mark
                PathEffect = new DashEffect(new float[] { 3, 3 }) // mark
            } // mark
        }
    };</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.XAxes = new Axis[]
{
    new Axis
    {
        Name = "X Axis",
        NamePaint = new SolidColorPaint(SKColors.Black), // mark

        LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark
        TextSize = 10,

        SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 } // mark
    }
};

cartesianChart1.YAxes = new Axis[]
{
    new Axis
    {
        Name = "Y Axis",
        NamePaint = new SolidColorPaint(SKColors.Red), // mark

        LabelsPaint = new SolidColorPaint(SKColors.Green), // mark
        TextSize = 20,

        SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) // mark
        { // mark
            StrokeThickness = 2, // mark
            PathEffect = new DashEffect(new float[] { 3, 3 }) // mark
        } // mark
    }
};</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/axes-paints.png)

## Axes.Labels and Axes.Labelers

<pre><code>Series = new ObservableCollection&lt;ISeries>
{
    new ColumnSeries&lt;int>
    {
        Values = new ObservableCollection&lt;int> { 200, 558, 458, 249 },
    }
};

XAxes = new List&lt;Axis>
{
    new Axis
    {
        // Use the labels property to define named labels.
        Labels = new string[] { "Anne", "Johnny", "Zac", "Rosa" } // mark
    }
};

YAxes = new List&lt;Axis>
{
    new Axis
    {
        // Now the Y axis we will display labels as currency
        // LiveCharts provides some common formatters
        // in this case we are using the currency formatter.
        Labeler = Labelers.Currency // mark

        // you could also build your own currency formatter
        // for example:
        // Labeler = (value) => value.ToString("C")

        // But the one that LiveCharts provides creates shorter labels when
        // the amount is in millions or trillions
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/1.7.labels.png)

## Zooming and panning

It is disabled by default, to enable it you must set the `ZoomMode` property.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Hidden"
    ZoomMode="X"> &lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    ZoomMode="LiveChartsCore.Measure.ZoomAndPanMode.X"> &lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X;</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/zoom-x.gif)

## ZoomingSpeed property

Defines the zooming speed, it is of type `double` and goes from 0 to 1, where 0 is the slowest and 1 the fastest,
do not confuse with animations seed, this property controls the new axis length (`MaxLimit` - `MinLimit`) when the `Zoom()` 
method is called.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    ZoomingSpeed="0"> &lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    ZoomingSpeed="0"> &lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.ZoomingSpeed = 0;</code></pre>
{{~ end ~}}

## Clearing the current zooming or panning

Setting `MinLimit` and `MaxLimit` properties to `null` will clear the current `zooming` or `panning`, and will let the chart fit the view
of the chart to the available data in the chart, the default value of both properties is `null`.

{{ render this "~/shared/chart.md" }}

{{ render this "~/shared/tooltips.md" }}

{{ render this "~/shared/legends.md" }}

## DrawMarginFrame property

This property defines a visual border for the `DrawMargin`.

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Lines.Area
{
    public class ViewModel
    {
        public DrawMarginFrame DrawMarginFrame => new DrawMarginFrame
        {
            Fill = new SolidColorPaint(SKColors.AliceBlue),
            Stroke = new SolidColorPaint(SKColors.Black, 3)
        };

        public ISeries[] Series { get; set; } = new[] { ... };
    }
}</code></pre>

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    DrawMarginFrame="{Binding DrawMarginFrame}">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    DrawMarginFrame="drawMarginFrame">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>

<pre><code>private DrawMarginFrame drawMarginFrame { get; set; }
    = new DrawMarginFrame
    {
        Fill = new SolidColorPaint(SKColors.AliceBlue),
        Stroke = new SolidColorPaint(SKColors.Black, 3)
    };</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.DrawMarginFrame = new DrawMarginFrame
{
    Fill = new SolidColorPaint(SKColors.AliceBlue),
    Stroke = new SolidColorPaint(SKColors.Black, 3)
};</code></pre>
{{~ end ~}}

![sections]({{ assets_url }}/docs/_assets/drawmarginframe.png)

## Sections 

A section is a visual area in a chart that highlights a range of values in an axis, to stablish the limits of a section
you must use the `Xi` and `Xj` properties in the `X` axis, the `Xi` represents the start of the section while the `Xj` the end,
the same goes for the `Yi` and `Yj` properties for the `Y` axis.

**Xi:** Gets or sets the xi, the value where the section starts at the X axis, set the property to null to indicate that 
the section must start at the beginning of the X axis, default is null.

**Xj:** Gets or sets the xj, the value where the section ends and the X axis, set the property to null to indicate that 
the section must go to the end of the X axis, default is null.

**Yi:** Gets or sets the yi, the value where the section starts and the Y axis, set the property to null to indicate that 
the section must start at the beginning of the Y axis, default is null.

**Yj:** Gets or sets the yj, the value where the section ends and the Y axis, set the property to null to indicate that 
the section must go to the end of the Y axis, default is null.

{{~ if xaml ~}}
<pre><code>using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.Sections
{
    public class ViewModel
    {
        public RectangularSection[] Sections { get; set; }
            = new RectangularSection[]
            {
                new RectangularSection
                {
                    // creates a section from 3 to 4 in the X axis
                    Xi = 3,
                    Xj = 4,
                    Fill = new SolidColorPaint(new SKColor(255, 205, 210))
                },

                new RectangularSection
                {
                    // creates a section from 5 to 6 in the X axis
                    // and from 2 to 8 in the Y axis
                    Xi = 5,
                    Xj = 6,
                    Yi = 2,
                    Yj = 8,
                    Fill = new SolidColorPaint(new SKColor(187, 222, 251))
                },

                 new RectangularSection
                {
                    // creates a section from 8 to the end in the X axis
                    Xi = 8,
                    Fill = new SolidColorPaint(new SKColor(249, 251, 231))
                }
            };

        public ISeries[] Series { get; set; } = new ISeries[] { ... };
    }
}</code></pre>

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    Sections="{Binding Sections}">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    Sections="sections">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>

<pre><code>private RectangularSection[] sections { get; set; }
    = new RectangularSection[]
    {
        new RectangularSection
        {
            // creates a section from 3 to 4 in the X axis
            Xi = 3,
            Xj = 4,
            Fill = new SolidColorPaint(new SKColor(255, 205, 210))
        },

        new RectangularSection
        {
            // creates a section from 5 to 6 in the X axis
            // and from 2 to 8 in the Y axis
            Xi = 5,
            Xj = 6,
            Yi = 2,
            Yj = 8,
            Fill = new SolidColorPaint(new SKColor(187, 222, 251))
        },

            new RectangularSection
        {
            // creates a section from 8 to the end in the X axis
            Xi = 8,
            Fill = new SolidColorPaint(new SKColor(249, 251, 231))
        }
    };</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.Sections = = new RectangularSection[]
{
    new RectangularSection
    {
        // creates a section from 3 to 4 in the X axis
        Xi = 3,
        Xj = 4,
        Fill = new SolidColorPaint(new SKColor(255, 205, 210))
    },

    new RectangularSection
    {
        // creates a section from 5 to 6 in the X axis
        // and from 2 to 8 in the Y axis
        Xi = 5,
        Xj = 6,
        Yi = 2,
        Yj = 8,
        Fill = new SolidColorPaint(new SKColor(187, 222, 251))
    },

        new RectangularSection
    {
        // creates a section from 8 to the end in the X axis
        Xi = 8,
        Fill = new SolidColorPaint(new SKColor(249, 251, 231))
    }
};</code></pre>
{{~ end ~}}

![sections]({{ assets_url }}/docs/_assets/cc-sections.png)
