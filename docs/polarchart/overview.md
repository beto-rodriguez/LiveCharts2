<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

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

![image]({{ assets_url }}/docs/_assets/polar-mvp.png)

The main components of this control are:

- Series
- Axes
- Tooltip
- Legend

But notice in the previous code snippet we did not specify the Axes, Tooltip, Legend or the series colors, this is because LiveCharts will
decide them based on the current theme of the library, you can also customize any of these properties when you require it, this article will
cover the most common scenarios.

## Series

There are ~multiple~ series available in the library, you can add one or mix them all in the same chart, every series has unique properties,
any image bellow is a link to an article explaining more about them.

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Polar%20Line%20Series">
<div class="series-miniature">
<img src="{{ assets_url }}/docs/samples/polarLines/basic/result.png" alt="series"/>
<div class="text-center"><b>Polar Line series</b></div>
</div>
</a>

## InitialRotation property

Defines an offset to stablish where the 0 angle is.

{{~ if xaml ~}}
<pre><code>&lt;lvc:PolarChart
    Series="{Binding Series}"
    InitialRotation="-90">
&lt;/lvc:PolarChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PolarChart
    Series="series"
    InitialRotation="-90">
&lt;/PolarChart></code></pre>

{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PolarChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>polarChart1.InitialRotation = -90;</code></pre>
{{~ end ~}}

Notice a change in the `InitialRotation` property is animated automatically based on the chart animations settings:

![image]({{ assets_url }}/docs/_assets/polar-rotation.gif)

## InnerRadius property

Defines the inner radius in pixels of the chart.

{{~ if xaml ~}}
<pre><code>&lt;lvc:PolarChart
    Series="{Binding Series}"
    InnerRadius="50">
&lt;/lvc:PolarChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PolarChart
    Series="Series"
    InnerRadius="50">
&lt;/PolarChart></code></pre>

{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PolarChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>polarChart1.InnerRadius = 50;</code></pre>
{{~ end ~}}

Notice a change in the `InnerRadius` property is animated automatically based on the chart animations settings:

![image]({{ assets_url }}/docs/_assets/polar-innerRadius.gif)

## TotalAngle property

Defines the total circumference angle in degrees, from 0 to 360, default is 360.

{{~ if xaml ~}}
<pre><code>&lt;lvc:PolarChart
    Series="{Binding Series}"
    TotalAngle="270">
&lt;/lvc:PolarChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PolarChart
    Series="Series"
    TotalAngle="50">
&lt;/PolarChart></code></pre>

{{~ end ~}}

{{~ if winforms ~}}
Drag a new `PolarChart` control from your toolbox, then in the code behind assign the `Series` property:

<pre><code>polarChart1.TotalAngle = 270;</code></pre>
{{~ end ~}}

Notice a change in the `TotalAngle` property is animated automatically based on the chart animations settings:

![image]({{ assets_url }}/docs/_assets/polar-totalangle.gif)

## PolarAxis.LabelsPaint and PolarAxis.SeparatorsPaint properties

You can set the color, use dashed lines, build gradients for the axis name, labels and separators, if you need help using `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).

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
        public ISeries[] Series { get; set; } = new ISeries[] { ... };

        public PolarAxis[] RadiusAxes { get; set; }
            = new PolarAxis[]
            {
                new PolarAxis
                {
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }  // mark
                }
            };

        public PolarAxis[] AngleAxes { get; set; }
            = new PolarAxis[]
            {
                new PolarAxis
                {
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
    RadiusAxes="{Binding RadiusAxes}"
    AngleAxes="{Binding AngleAxes}">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using LiveChartsCore.SkiaSharpView.Painting.Effects;
@using SkiaSharp;

&lt;CartesianChart
    Series="series"
    RadiusAxes="radiusAxes"
    AngleAxes="angleAxes">
&lt;/CartesianChart>

@code {
    private ISeries[] series { get; set; } = new ISeries[] {  };

    private PolarAxis[] radiusAxes { get; set; }
        = new PolarAxis[]
        {
            new PolarAxis
            {
                TextSize = 10,
                LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark

                SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }  // mark
            }
        };

    private PolarAxis[] angleAxes { get; set; }
        = new PolarAxis[]
        {
            new PolarAxis
            {
                LabelsPaint = new SolidColorPaint(SKColors.Green), // mark
                TextSize = 20,

                SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) // mark
                { // mark
                    StrokeThickness = 2, // mark
                    PathEffect = new DashEffect(new float[] { 3, 3 }) // mark
                } // mark
            }
        };
}</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>polarChart1.RadiusAxes = new PolarAxis[]
{
    new PolarAxis
    {
        TextSize = 10,
        LabelsPaint = new SolidColorPaint(SKColors.Blue), // mark

        SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }  // mark
    }
};

polarChart1.AngleAxes = new PolarAxis[]
{
    new PolarAxis
    {
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

![image]({{ assets_url }}/docs/_assets/polar-axes-style.png)

## PolarAxis.Labels vs PolarAxis.Labeler properties

There are 2 ways to format and axis labels, using the `Labels` property and using the `Labeler` property,
you must normally use the `Labels` property to indicate names, and the `Labeler` property to give format to the current label.

The labels property is a collection of string, if this property is not `null`, then the axis label will be pulled from the labels 
collection, the label is mapped to the chart based on the position of the label and the position of the point, both integers,
if the axis requires a label outside the bounds of the labels collection, then the index will be returned as the label,
default value is null.

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
            = new ObservableCollection&lt;ISeries>
            {
                new PolarLineSeries&lt;double>
                {
                    Values = new ObservableCollection&lt;double> { 5, 2, 1, 4, 3, 3 },
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0.2,
                    Stroke = null,
                    IsClosed = true,
                },
                new PolarLineSeries&lt;double>
                {
                    Values = new ObservableCollection&lt;double> { 3, 5, 2, 3, 4, 5 },
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0.2,
                    Stroke = null,
                    IsClosed = true,
                }
            };

        public PolarAxis[] RadiusAxes { get; set; }
            = new PolarAxis[]
            {
                new PolarAxis
                {
                    // formats the value as a number with 2 decimals.
                    Labeler = value => value.ToString("N2")
                }
            };

        public PolarAxis[] AngleAxes { get; set; }
            = new PolarAxis[]
            {
                new PolarAxis
                {
                    Labels = new[] { "strength", "stamina", "resistance", "power", "hit points", "mana" },
                    MinStep = 1,
                    ForceStepToMin = true
                }
            };
    }
}</code></pre>

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    RadiusAxes="{Binding RadiusAxes}"
    AngleAxes="{Binding AngleAxes}">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using LiveChartsCore.SkiaSharpView.Painting.Effects;
@using SkiaSharp;

&lt;CartesianChart
    Series="series"
    RadiusAxes="radiusAxes"
    AngleAxes="angleAxes">
&lt;/CartesianChart>

@code {
    private ISeries[] series { get; set; } 
        = new ObservableCollection&lt;ISeries>
        {
            new PolarLineSeries&lt;double>
            {
                Values = new ObservableCollection&lt;double> { 5, 2, 1, 4, 3, 3 },
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                Stroke = null,
                IsClosed = true,
            },
            new PolarLineSeries&lt;double>
            {
                Values = new ObservableCollection&lt;double> { 3, 5, 2, 3, 4, 5 },
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0.2,
                Stroke = null,
                IsClosed = true,
            }
        };

    private PolarAxis[] radiusAxes { get; set; }
        = new PolarAxis[]
        {
            new PolarAxis
            {
                // formats the value as a number with 2 decimals.
                Labeler = value => value.ToString("N2")
            }
        };

    private PolarAxis[] angleAxes { get; set; }
        = new PolarAxis[]
        {
            new PolarAxis
            {
                Labels = new[] { "strength", "stamina", "resistance", "power", "hit points", "mana" },
                MinStep = 1,
                ForceStepToMin = true
            }
        };
}</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>polarChart1.Series = new ObservableCollection&lt;ISeries>
{
    new PolarLineSeries&lt;double>
    {
        Values = new ObservableCollection&lt;double> { 5, 2, 1, 4, 3, 3 },
        GeometryFill = null,
        GeometryStroke = null,
        LineSmoothness = 0.2,
        Stroke = null,
        IsClosed = true,
    },
    new PolarLineSeries&lt;double>
    {
        Values = new ObservableCollection&lt;double> { 3, 5, 2, 3, 4, 5 },
        GeometryFill = null,
        GeometryStroke = null,
        LineSmoothness = 0.2,
        Stroke = null,
        IsClosed = true,
    }
};
        
polarChart1.RadiusAxes = new PolarAxis[]
{
    new PolarAxis
    {
        // formats the value as a number with 2 decimals.
        Labeler = value => value.ToString("N2")
    }
};

polarChart1.AngleAxes = new PolarAxis[]
{
    new PolarAxis
    {
        Labels = new[] { "strength", "stamina", "resistance", "power", "hit points", "mana" },
        MinStep = 1,
        ForceStepToMin = true
    }
};</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/polar-star.png)

## PolarAxis.LabelsRotation property

Indicates the axis labels rotation in degrees, in the following image we have a rotation of 45 degrees in the Radius axis,
and for the angle axis the labels rotation will follow the tangent line.

<pre><code>RadiusAxes = new PolarAxis[]
{
    new PolarAxis    
    {
        LabelsRotation = 45 // mark
    }
};
AngleAxes = new PolarAxis[]
{
    new PolarAxis    
    {
        LabelsRotation = LiveCharts.TangentAngle // mark
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/polar-labels-rotation.png)

You can also place labels at the cotangent angle:

<pre><code>AngleAxes = new PolarAxis[]
{
    new PolarAxis
    {
        LabelsRotation = LiveCharts.CotangentAngle // mark
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/polar-labels-rotation2.png)

Finally notice that you can combine the `LiveCharts.CotangentAngle` and `LiveCharts.TangentAngle` with decimal degrees,
the following expressions are valid also:

<pre><code>Series { get; set; } = new PolarAxis[]
{
    new PolarAxis
    {
        LabelsRotation = 30, // in degrees
    },
    new PolarAxis
    {
        LabelsRotation = LiveCharts.TangentAngle + 30, // the tangent + 30 degrees
    },
    new PolarAxis
    {
        LabelsRotation = LiveCharts.CotangentAngle + 30, // the cotangent + 30 degrees
    }
};</code></pre>

## PolarAxis.Visible property

When the `Visible` property is set to `false` the axis will not be drawn and also it will not take any space in the chart
default is `true`.

{{ render this "~/shared/chart.md" }}

{{ render this "~/shared/tooltips.md" }}

{{ render this "~/shared/legends.md" }}
