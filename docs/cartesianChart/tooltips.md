<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Tooltips

Tooltips are popups that help the user to read a chart as the pointer moves.

![tooltips]({{ assets_url }}/docs/_assets/tooltips.gif)

## Behaviour

On **Windows** or **MacOS** you can move the pointer over the chart to display the tooltip, tooltips will be closed when the
pointer leaves the chart area.

On **Android** or **iOS** slide your finger over the chart to display the tooltip, the tooltip will be closed when the finger
goes up.

## TooltipPosition property

You can place a tooltip at `Top`, `Bottom`, `Left`, `Right`, `Center` or `Hidden` positions, for now 
tooltips for the `PieChart` class only support the `Center` position, default value is `Top`.

Notice the `Hidden` position will disable tooltips in a chart.

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

## TooltipFindingStrategy property

Every point drawn by the library defines a [HoverArea]({{ website_url }}/api/{{ version }}/LiveChartsCore.Kernel.Drawing.HoverArea),
it defines an area in the chart that "triggers" the point, it is specially important to fire tooltips, a point will be included in a
tooltip when the hover area was triggered by the pointer position.

The `TooltipFindingStrategy` property determines the hover area planes (X or Y) that a chart will use to trigger the `HoverArea` instances.
In a chart, the following options are available:

**CompareAll**: Selects all the points whose hover area contain the pointer position.

![tooltips]({{ assets_url }}/docs/_assets/tsm_cac.gif)

**CompareOnlyX**: Selects all the points whose hover area contain the pointer position, but it ignores the Y plane.

![tooltips]({{ assets_url }}/docs/_assets/tsm_cxc.gif)

**CompareOnlyY**: Selects all the points whose hover area contain the pointer position, but it ignores the X plane.

![tooltips]({{ assets_url }}/docs/_assets/tsm_cyc.gif)

**CompareAllTakeClosest**: Selects all the points whose hover area contain the pointer position,
it only takes the closest point to the pointer, one per series.

**CompareOnlyXTakeClosest**: Selects all the points whose hover area contain the pointer position, but it ignores the Y plane,
it only takes the closest point to the pointer, one per series.

**CompareOnlyYTakeClosest**: Selects all the points whose hover area contain the pointer position, but it ignores the X plane,
it only takes the closest point to the pointer, one per series.

**Automatic** *(default)*: Based on the series in the chart, LiveCharts will determine a finding strategy (one of the previous mentioned),
all the series have a preferred finding strategy, normally vertical series prefer the `CompareOnlyXTakeClosest` strategy, 
horizontal series prefer `CompareOnlyYTakeClosest`, and scatter series prefers `CompareAllTakeClosest`, if all the series prefer the same strategy, then that
strategy will be selected for the chart, if any series differs then the `CompareAllTakeClosest` strategy will be used.

:::info
Notice that the `Axis.UnitWidth` property might affect the tooltips in `DateTime` scaled charts, ensure your chart axis is using
the properly [unit width]({{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#unitwidth-property).
:::

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipFindingStrategy="CompareOnlyX">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    TooltipFindingStrategy="LiveChartsCore.Measure.TooltipFindingStrategy.CompareOnlyX">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.Series = new ISeries[] { new LineSeries&lt;int> { Values = new[] { 2, 5, 4 } } };
cartesianChart1.TooltipFindingStrategy = LiveChartsCore.Measure.TooltipFindingStrategy.CompareOnlyX;</code></pre>
{{~ end ~}}

## Tooltip point text

You can define the text the tooltip will display for a given point, using the `Series.TooltipLabelFormatter` property, this 
property is of type `Func<ChartPoint, string>` this means that is is a function, that takes a point as parameter
and returns a string, the point will be injected by LiveCharts in this function to get a string out of it when it
requires to build the text for a point in a tooltip, the injected point will be different as the user moves the pointer over the
user interface.

By default the library already defines a default `TooltipLabelFormatter` for every series, all the series have a different
formatter, but generally the default value uses the `Series.Name` and the `ChartPoint.PrimaryValue` properties, the following
code snippet illustrates how to build a custom tooltip formatter.

<pre><code>new LineSeries&lt;double>
{
    Name = "Sales",
    Values = new ObservableCollection&lt;double> { 200, 558, 458 },
    // for the following formatter
    // when the pointer is over the first point (200), the tooltip will display:
    // Sales: 200
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue}"
},

new ColumnSeries&lt;double>
{
    Name = "Sales 2",
    Values = new ObservableCollection&lt;double> { 250, 350, 240 },
    // now it will use a currency formatter to display the primary value
    // result: Sales 2: $200.00
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:C2}"
},

new StepLineSeries&lt;ObservablePoint>
{
    Name = "Average",
    Values = new ObservableCollection&lt;ObservablePoint>
    {
        new ObservablePoint(10, 5),
        new ObservablePoint(5, 8)
    },
    // We can also display both coordinates (X and Y in a cartesian coordinate system)
    // result: Average: 10, 5
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.SecondaryValue}, {chartPoint.PrimaryValue}"
},

new ColumnSeries&lt;ObservablePoint>
{
    Values = new ObservableCollection&lt;double> { 250, 350, 240 },
    // or anything...
    // result: Sales at this moment: $200.00
    TooltipLabelFormatter =
        (chartPoint) => $"Sales at this moment: {chartPoint.PrimaryValue:C2}"
}</code></pre>

# Customize default tooltips

You can quickly change the position, the font, the text size or the background color:

## View

{{~ if xaml ~}}
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Axes/NamedLabels/$PlatformViewFile" ~}}
{{~ end ~}}

{{~ if winforms ~}}
{{~ render_params_file_as_code this "~/../samples/WinFormsSample/Axes/NamedLabels/View.cs" ~}}
{{~ end ~}}

{{~ if eto ~}}
{{~ render_params_file_as_code this "~/../samples/EtoFormsSample/Axes/NamedLabels/View.cs" ~}}
{{~ end ~}}

{{~ if blazor ~}}
{{~ render_params_file_as_code this "~/../samples/BlazorSample/Pages/Axes/NamedLabels.razor" ~}}
{{~ end ~}}

## View model

```c#
[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } = { ... };
    public Axis[] XAxes { get; set; } = { ... };
    public Axis[] YAxes { get; set; } = { ... };

    public SolidColorPaint TooltipTextPaint { get; set; } = // mark
        new SolidColorPaint // mark
        { // mark
            Color = new SKColor(242, 244, 195), // mark
            SKTypeface = SKTypeface.FromFamilyName("Courier New") // mark
        }; // mark

    public SolidColorPaint TooltipBackgroundPaint { get; set; } = // mark
        new SolidColorPaint(new SKColor(72, 0, 50)); // mark
}
```

![image]({{ assets_url }}/docs/samples/general/customTooltips/styling-tooltips.png)

## Custom tooltip control

You can also create your own tooltip, the recommended way is to use the LiveCharts API (example bellow) but you can
use anything as tooltip as soon as it implements the `IChartTooltip<T>` interface. In the following example we build
a custom control to render tooltips in out charts using the LiveCharts API.

## CustomTooltip.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedTooltips/CustomTooltip.cs" ~}}

## View

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/TemplatedTooltips/$PlatformViewFile" ~}}

![custom tooltip]({{ assets_url }}/docs/_assets/tooltip-custom-template.gif)
