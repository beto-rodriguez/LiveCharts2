<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Tooltips

Tooltips are popups that help the user to read a chart as the pointer moves.

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltips.gif)

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

Every point drawn by the library defines a [HoverArea]{{ website_url}}/api/{{ version }}/LiveChartsCore.Kernel.Drawing.HoverArea),
it defines an area in the chart that "triggers" the point, it is specially important to fire tooltips, a point will be included in a
tooltip when the hover area was triggered by the pointer position.

The `TooltipFindingStrategy` property determines the hover area planes (X or Y) that a chart will use to trigger the `HoverArea` instances
in the chart, the following options are available:

**CompareOnlyX**: Selects all the points that share the same X unit range (the space taken in the plot by a unit in the X axis).

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/compare-x-strategy.gif)

**CompareOnlyY**: Selects all the points that share the same Y unit range (the space taken in the plot by a unit in the Y axis).

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/compare-y-strategy.gif)

**CompareAll**: Selects all the points that share the X and Y range.

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/compare-all-strategy.gif)

**Automatic** *(default)*: Based on the series in the chart, LiveCharts will determine a finding strategy (`CompareAll`, `CompareOnlyX` or 
`CompareOnlyY`), all the series have a preferred finding strategy, normally vertical series prefer the `CompareOnlyX` strategy, 
horizontal series prefer `CompareOnlyY`, and scatter series prefers `CompareAll`, if all the series prefer the same strategy, then that
strategy will be selected for the chart, if any series differs then the `CompareAll` strategy will be used.

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

## Styling tooltips

{{~ if xaml ~}}
A chart exposes many properties to quickly style a tooltip:

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Left"
    TooltipFontFamily="Courier New"
    TooltipFontSize="25"
    TooltipTextBrush="#f2f4c3"
    TooltipBackground="#480032">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
A chart exposes many properties to quickly style a tooltip:

<pre><code>cartesianChart1.TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left;
cartesianChart1.TooltipFont = new System.Drawing.Font("Courier New", 25);
cartesianChart1.TooltipTextColor = System.Drawing.Color.FromArgb(255, 242, 244, 195);
cartesianChart1.TooltipBackColor = System.Drawing.Color.FromArgb(255, 72, 0, 50);</code></pre>
{{~ end ~}}

{{~ if blazor ~}}
You can use css to override the style of the tooltip.

<pre><code>&lt;style>
	.lvc-tooltip {
		background-color: #480032 !important;
	}

	.lvc-tooltip-item {
		font-family: SFMono-Regular, Menlo, Monaco, Consolas !important;
		color: #F2F4C3 !important;
	}
&lt;/style></code></pre>
{{~ end ~}}

The code above would result in the following tooltip:

![zooming](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltip-custom-style.png)

## Custom template

{{~ if xaml || blazor ~}}
If you need to customize more, you can also pass your own template:
{{~ end ~}}

{{~ if avalonia ~}}
{{~ "~/../samples/AvaloniaSample/General/TemplatedTooltips/View.axaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs)).
:::
{{~ end ~}}

{{~ if blazor ~}}
{{~ "~/../samples/BlazorSample/Pages/General/TemplatedTooltips.razor" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([view](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultTooltip.razor), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultTooltip.razor.cs)).
:::
{{~ end ~}}

{{~ if maui ~}}
{{~ "~/../samples/MauiSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultTooltip.xaml.cs)).
:::
{{~ end ~}}

{{~ if uwp ~}}
{{~ "~/../samples/UWPSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.UWP/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.UWP/DefaultTooltip.xaml.cs)).
:::
{{~ end ~}}

{{~ if winforms ~}}
You can create your own tooltip control, the key is that your control must implement `IChartTooltip<SkiaSharpDrawingContext>` and then
you have to create a new instance of that control when your chart initializes.

Add a new form to your app named `CustomTooltip`, then change the code behind as follows:

{{~ "~/../samples/WinFormsSample/General/TemplatedTooltips/CustomTooltip.cs" | render_file_as_code ~}}

Your tooltip is ready to be used, now when you create a chart, we have to pass a new instance of the tooltip we just created.

<pre><code>var cartesianChart = new CartesianChart(tooltip: new CustomTooltip())
{
    Series = viewModel.Series
};</code></pre>
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs)).
:::
{{~ end ~}}

{{~ if winui ~}}
{{~ "~/../samples/WinUISample/WinUI/WinUI/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultTooltip.xaml.cs)).
:::
{{~ end ~}}

{{~ if wpf ~}}
{{~ "~/../samples/WPFSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultTooltip.xaml.cs)).
:::
{{~ end ~}}

{{~ if xamarin ~}}
{{~ "~/../samples/XamarinSample/XamarinSample/XamarinSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}
:::tip
You can find a more complete example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultTooltip.xaml.cs)).
:::
{{~ end ~}}

![custom tooltip](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltip-custom-template.png)
