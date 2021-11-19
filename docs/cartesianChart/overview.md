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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/cc-mvp.png)

The main components of this control are:

- Series
- Axes
- Tooltip
- Legend

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/chart-overview.png)

But notice in the previous code snippet we did not specify the Axes, Tooltip, Legend or the series colors, this is because LiveCharts will
decide them based on the current theme of the library, you can also customize any of these properties when you require it, this article will
cover the most common scenarios.

## Series

There are multiple series available in the library, you can add one or mix them all in the same chart, every series has unique properties,
any image bellow is a link to an article explaining more about them.

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Line%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/lines/basic/result.png" alt="series"/>
<div class="text-center"><b>Line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Column%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/bars/basic/result.png"  alt="series"/>
<div class="text-center"><b>Column series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Scatter%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/scatter/basic/result.png"  alt="series"/>
<div class="text-center"><b>Scatter series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Step%20line%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/stepline/basic/result.png" alt="series"/>
<div class="text-center"><b>Step line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Heat%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/heat/basic/result.png" alt="series"/>
<div class="text-center"><b>Heat series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Candle%20Sticks%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/financial/basic/result.png" alt="series"/>
<div class="text-center"><b>Candle sticks series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Line%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/stackedArea/basic/result.png" alt="series"/>
<div class="text-center"><b>Stacked line series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Column%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/stackedColumn/basic/result.png" alt="series"/>
<div class="text-center"><b>Stacked column series</b></div>
</div>
</a>

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Stacked%20Step%20Line%20Series">
<div class="series-miniature">
<img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/samples/stackedStepLine/basic/result.png" alt="series"/>
<div class="text-center"><b>Stacked step line series</b></div>
</div>
</a>

## Axes

A `CartesianChart` has 2 axes, the `YAxis` and the `XAxis` properties, an [IAxis]({{ website_url }}/api/{{ version}}/LiveChartsCore.Kernel.Sketches.ICartesianAxis) 
besides de visual customization such as labels format and separators paints it also controls multiple important features such as 
the **Zooming**, the **Panning**, the **Scale** (log) and the **Paging**.

This is a brief sample about how to use the main features of the `Axis` class, you can find a more detailed article at the button below or at the 
[API explorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.Axis).

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Axes" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/axes-paints.png)

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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/1.7.labels.png)

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

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/zoom-x.gif)

## Clearing the current zooming or panning

Setting `MinLimit` and `MaxLimit` properties to `null` will clear the current `zooming` or `panning`, and will let the chart fit the view
of the chart to the available data in the chart, the default value of both properties is `null`.

{{ render this "~/shared/chart.md" }}

## Tooltips

Tooltips are popups that help the user to read a chart as the pointer moves.

This is a brief sample about how to use the main features of the `IChartTooltip<T>` interface, you can find a more detailed article at the button below or at the 
[API explorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.Kernel.Sketches.IChartTooltip-1).

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Tooltips" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
<b>Go to the full tooltips article</b>
</a>

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltips.gif)

You can place a tooltip at `Top`, `Bottom`, `Left`, `Right`, `Center` or `Hidden` positions, for now 
tooltips for the `PieChart` class only support the `Center` position, default value is `Top`.

:::info
Notice the `Hidden` position will disable tooltips in a chart.
:::

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Top">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Bottom">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Left">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Right">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Center">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Hidden">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Top">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Bottom">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Left">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Right">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Center">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Hidden">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.Series = new ISeries[] { new LineSeries&lt;int> { Values = new[] { 2, 5, 4 } } };
cartesianChart1.TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Bottom;</code></pre>
{{~ end ~}}

## Legends

A legend is a visual element that displays a list with the name, stroke and fills of the series in a chart:

This is a brief sample about how to use the main features of the `IChartLegend<T>` interface, you can find a more detailed article at the button below or at the 
[API explorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.Kernel.Sketches.IChartLegend-1).

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Legends" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
<b>Go to the full legends article</b>
</a>

![legends](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/legend.png)

You can place a legend at `Top`, `Bottom`, `Left`, `Right` or `Hidden` positions, notice the `Hidden` position will 
disable legends in a chart, default value is `Hidden`.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Top">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Bottom">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Left">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Right">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Hidden">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    LegendPosition="LiveChartsCore.Measure.LegendPosition.Top">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    LegendPosition="LiveChartsCore.Measure.LegendPosition.Bottom">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    LegendPosition="LiveChartsCore.Measure.LegendPosition.Left">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    LegendPosition="LiveChartsCore.Measure.LegendPosition.Right">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    LegendPosition="LiveChartsCore.Measure.LegendPosition.Hidden">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.TooltipPosition = LiveChartsCore.Measure.LegendPosition.Bottom; // mark
// or use Top, Left, Right or Hidden
</code></pre>
{{~ end ~}}