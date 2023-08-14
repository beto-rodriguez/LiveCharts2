<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Axes

The following diagram illustrates an axis and its main components:

![image]({{ assets_url }}/docs/_assets/1.7.diagram.png)

The cartesian chart control has the `XAxes` and `YAxes` properties, both of type `IEnumerable<IAxis>` by default
when you do not set these properties, they will be an array containing only one element of the `Axis` class 
(`new Axis[] { new Axis() }`). 

This article will cover the properties of an `IAxis` interface that require a further explanation but it will 
not cover all of them, if you need to know more about this type then use the [API explorer](./) in our website.

## Zooming and Panning

Both of these features are directly related to the <a href="#MaxLimit and MinLimit properties">MaxLimit and MinLimit properties</a>,
zooming occurs when the mouse wheel moves or when the pinch gesture occurs, the panning is called when the pointer goes down,
moves and then goes up or when the pan gesture occurs.

Zooming and panning is disabled by default, you can enable it by setting the `ZoomMode` property, this property is of type
[ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) and the options are:

- `X`: Enables zooming and panning on the X axis.
- `Y`: Enables zooming and panning on the Y axis.
- `Both`: Enables zooming and panning on both axes.
- `None`: Disables zooming and panning.
- `PanX`: Enables panning on the X axis.
- `PanY`: Enables panning on the Y axis.
- `ZoomX`: Enables zooming on the X axis.
- `ZoomY`: Enables zooming on the Y axis.

The [ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) type is a flag enum,
so you can combine the options, for example, if you want to enable zooming on the X axis and panning on the Y axis you can
set the `ZoomMode` property to `ZoomAndPanMode.ZoomX | ZoomAndPanMode.PanY`.

There is also the `InvertPanningPointerTrigger` flag, when this flag is present the panning will be triggered using
the right click on desktop devices and the touch-and-hold gesture on touch devices, the `zoom by section` feature will be
triggered to the left click on desktop devices and the touch-and-hold gesture on touch devices.

Here are a few examples of the different zoom modes:

**X Mode:**

When the user interacts with the chart, He/She/* is only moving the chart in the X axis direction, the Y axis range is calculated automatically
by the library to fit all the visible points in the X axis.

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

**Y Mode:**

When the user interacts with the chart, He/She/* is only moving the chart in the Y axis direction, the X axis range is calculated automatically
by the library to fit all the visible points in the Y axis.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Hidden"
    ZoomMode="Y"> &lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    ZoomMode="LiveChartsCore.Measure.ZoomAndPanMode.Y"> &lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Y;</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/zoom-y.gif)

**Both Mode:**

Both axis are moved by the user, this method could lead to the user to get lost in the canvas, you probably need to implement
a button to reset the zoom to the data in the chart (settings `MinLimit` and `MaxLimit` properties to `null`).

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Hidden"
    ZoomMode="Both"> &lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    ZoomMode="LiveChartsCore.Measure.ZoomAndPanMode.Both"> &lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Both;</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/zoom-both.gif)

## MaxLimit and MinLimit properties

These properties control the **zooming** and **panning** of a chart, both properties are of type of `double?`, you can
indicate the visible range of an axis setting these properties. For example, imagine you are plotting a 
`LineSeries` that goes from 0 to 100 in the `X` axis, but you only need to show to the user the first 10 points (**paging**),
in this case you could set the `MinLimit` property to 0 and the `MaxLimit` to 10.

<pre><code>var xAxis = new Axis
{
    MaxLimit = 10, // mark
    MinLimit = 0 // mark
};

myChart.XAxes = new List&lt;Axis>{ xAxis };</code></pre>

When a user uses the zooming and panning features, both properties will be calculated by the library, this means 
that you can always know where the user is at a chart when you read both of these properties, notice the `Axis` class
implements `INotifyPropertyChanged`, if you need so you could subscribe to the `PropertyChanged` event to detect every
time a user uses these features (zooming and panning).

With the code above, we could detect the chart visible range when the user is zooming or panning subscribing a handler in
the `Axis.PropertyChanged` event:

<pre><code>var xAxis = new Axis
{
    MaxLimit = 10,
    MinLimit = 0
};

xAxis.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => // mark
{ // mark
    if (e.PropertyName is (nameof(xAxis.MaxLimit)) or (nameof(xAxis.MinLimit))) // mark
    { // mark
        // at this point the axis limits changed // mark
        // the user is using the zooming or panning features // mark
        // or the range was set explicitly in code // mark

        var minXVisible = xAxis.MinLimit; // mark
        var maxXVisible = xAxis.MaxLimit; // mark
    } // mark
}; // mark

myChart.XAxes = new List&lt;Axis>{ xAxis };</code></pre>

Finally notice what when the chart loads, by default both of these properties are `null`, and `null` means that the library will
calculate the value of both, so when you need to read the initial value of an axis you can access the `Axis.DataBounds` or
`Axis.VisibleDataBounds` properties, these properties are loaded until the chart is measured, there are multiple options to wait
for the chart to be measured, in this example we will use the `Chart.UpdateStarted` event that happens when the chart is already measured
and the draw in the UI is about to start.

<pre><code>using System.Linq; // mark

// get the control in the UI
var chartControl = (CartesianChart)FindName("cartesianChart"); // notice this code is WPF specific

chartControl.UpdateStarted +=
    (LiveChartsCore.Kernel.Sketches.IChartView<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> chart) =>
    {
        var x = chartControl.XAxes.First(); 

        var minLimit = x.VisibleDataBounds.Min; // mark
        var maxLimit = x.VisibleDataBounds.Max; // mark
    };</code></pre>

## Clearing the current zooming or panning

Setting `MinLimit` and `MaxLimit` properties to `null` will clear the current `zooming` or `panning`, and will let the chart fit the view
of the chart to the available data in the chart, the default value of both properties is `null`.

## MinStep property

The `Step` defines the interval or distance between every separator and label in the axis, LiveCharts will calculate it automatically 
based on the chart data and the chart size size, but you can configure the minimum value this property could be, for example in the case
where you don't want decimal separations in the axis labels, you could set the `MinStep` property to `1`, this way, when the calculated 
step is less that `1` the library will force it to be `1`.

<pre><code>new Axis
{
    // The MinStep property lets you define the minimum separation (in chart values scale)
    // between every axis separator, in this case we don't want decimals,
    // so lets force it to be greater or equals than 1
    MinStep = 1
}</code></pre>

## Position property

The axis position property is type `AxisPosition`, it is an `enum` containing only 2 options, `Start` and `End` 
the `Start` position places the axis at the bottom for X axes and at the left for Y axes and the `End` position 
places the axis at the top for X axes and at the right for Y axes.

<pre><code>YAxes = new List&lt;Axis>
{
    new Axis    
    {
        Position = LiveChartsCore.Measure.AxisPosition.End
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/1.7.position.png)

## LabelsPaint and SeparatorsPaint properties

You can set the color, use dashed lines, build gradients for the axis name, labels and separators.

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
<pre><code>@using LiveChartsCore;
@using LiveChartsCore.SkiaSharpView;
@using LiveChartsCore.SkiaSharpView.Painting;
@using LiveChartsCore.SkiaSharpView.Painting.Effects;
@using SkiaSharp;

&lt;CartesianChart
    Series="series"
    XAxes="xAxes"
    YAxes="yAxes">
&lt;/CartesianChart>

@code {
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
        };
}</code></pre>
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

:::info
Paints can create gradients, dashed lines and more, if you need help using the `Paint` instances take 
a look at the [Paints article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Paints).
:::

## Labels vs Labeler properties

There are 2 ways to format and axis labels, using the `Labels` property and using the `Labeler` property,
you must normally use the `Labels` property to indicate names, and the `Labeler` property to give format to the current label.

The labels property is a collection of string, if this property is not `null`, then the axis label will be pulled from the labels 
collection, the label is mapped to the chart based on the position of the label and the position of the point, both integers,
if the axis requires a label outside the bounds of the labels collection, then the index will be returned as the label,
default value is null.

![image]({{ assets_url }}/docs/_assets/1.7.labels.png)

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

## LabelsRotation property

Indicates the axis labels rotation in degrees, in the following image we have a rotation of 45 degrees in the Y axis.

<pre><code>YAxes = new List&lt;Axis>
{
    new Axis    
    {
        LabelsRotation = 45
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/1.7.rotation.png)

## Multiple axes

Both of these properties are collections because the library supports to have more than one axis, the following sample illustrates
how to create a chart that uses multiple axes:

![image]({{ assets_url }}/docs/_assets/1.7.multiple.png)

<pre><code>var blue = new SKColor(25, 118, 210);
var red = new SKColor(229, 57, 53);
var yellow = new SKColor(198, 167, 0);

Series = new ObservableCollection&lt;ISeries>
{
    new LineSeries&lt;double>
    {
        LineSmoothness = 1,
        Name = "tens",
        Values = new ObservableCollection&lt;double> { 14, 13, 14, 15, 17 },
        Stroke = new SolidColorPaint(blue, 2),
        GeometryStroke = new SolidColorPaint(blue, 2),
        Fill = null,
        // it will be scaled at the Axis[0] instance (see YAxes property bellow)
        ScalesYAt = 0 // mark
    },
    new LineSeries&lt;double>
    {
        Name = "tens 2",
        Values = new ObservableCollection&lt;double> { 11, 12, 13, 10, 13 },
        Stroke = new SolidColorPaint(blue, 2),
        GeometryStroke = new SolidColorPaint(blue, 2),
        Fill = null,
        // it will be scaled at the Axis[0] instance (see YAxes property bellow)
        ScalesYAt = 0 // mark
    },
    new LineSeries&lt;double>
    {
        Name = "hundreds",
        Values = new ObservableCollection&lt;double> { 533, 586, 425, 579, 518 },
        Stroke = new SolidColorPaint(red, 2),
        GeometryStroke = new SolidColorPaint(red, 2),
        Fill = null,
        // it will be scaled at the YAxes[1] instance (see YAxes property bellow)
        ScalesYAt = 1 // mark
    },
    new LineSeries&lt;double>
    {
        Name = "thousands",
        Values = new ObservableCollection&lt;double> { 5493, 7843, 4368, 9018, 3902 },
        Stroke = new SolidColorPaint(yellow, 2),
        GeometryStroke = new SolidColorPaint(yellow, 2),
        Fill = null,
         // it will be scaled at the YAxes[2] instance (see YAxes property bellow)
        ScalesYAt = 2 // mark
    }
};

YAxes = new List&lt;Axis>
{
    new Axis // the "units" and "tens" series will be scaled on this axis
    {
        LabelsPaint = new SolidColorPaint(blue)
    },
    new Axis // the "hundreds" series will be scaled on this axis
    {
        LabelsPaint = new SolidColorPaint(red),
        ShowSeparatorLines = false,
        Position = LiveChartsCore.Measure.AxisPosition.End
    },
    new Axis // the "thousands" series will be scaled on this axis
    {
        LabelsPaint = new SolidColorPaint(yellow),
        ShowSeparatorLines = false,
        Position = LiveChartsCore.Measure.AxisPosition.End
    }
};</code></pre>

## Inverted property

Normally both `X` and `Y` axes scale according to the Cartesian coordinate system you can invert the direction of the axes
setting the `Inverted` property to `true`.

## Visible property

When the `Visible` property is set to `false` the axis will not be drawn and also it will not take any space in the chart
default is `true`.

## UnitWidth property

You normally do not need to use this property, unless you ware working in a Logarithmic or DateTime scale. 
The unit width property affects a few series, `Column`, `Row` and `Financial` series, the unit width is the width of a bar in 
a chart, by default this property is 1, but if you are using a custom scale, you must let the chart know about it.

<pre><code>// The UnitWidth is only required for column or financial series
// because the library needs to know the width of each column, by default the
// width is 1, but when you are using a different scale, you must let the library know it.

XAxes = new List&lt;Axis>
{
    new Axis
    {
        // in this case we want our columns with a width of 1 day, we can get that number
        // using the following syntax
        UnitWidth = TimeSpan.FromDays(1).Ticks

        // if the difference between our points is in hours then we would:
        // UnitWidth = TimeSpan.FromHours(1).Ticks,

        // since all the months and years have a different number of days
        // we can use the average, it would not cause any visible error in the user interface
        // Months: TimeSpan.FromDays(30.4375).Ticks
        // Years: TimeSpan.FromDays(365.25).Ticks
    }
};</code></pre>
