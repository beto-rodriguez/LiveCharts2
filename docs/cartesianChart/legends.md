<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Legends

A legend is a visual element that displays a list with the name, stroke and fills of the series in a chart:

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

## Styling legends

{{~ if xaml ~}}
A chart exposes many properties to quickly style a legend:

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    LegendPosition="Left"
    LegendFontFamily="Courier New"
    LegendFontSize="25"
    LegendTextBrush="#f2f4c3"
    LegendBackground="#480032">
&lt;/lvc:CartesianChart>
</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
A chart exposes many properties to quickly style a legend:

<pre><code>cartesianChart1.LegendPosition = LiveChartsCore.Measure.LegendPosition.Left;
cartesianChart1.LegendFont = new System.Drawing.Font("Courier New", 25);
cartesianChart1.LegendTextColor = System.Drawing.Color.FromArgb(255, 50, 50, 50);
cartesianChart1.LegendBackColor = System.Drawing.Color.FromArgb(255, 250, 250, 250);</code></pre>
{{~ end ~}}

{{~ if blazor ~}}
You can use css to override the style of the tooltip.

<pre><code>&lt;style>
    /*
        You can also use css to override the styles.
    */

    .lvc-legend {
        background-color: #fafafa !important;
    }

    .lvc-legend-item {
        font-family: SFMono-Regular,Menlo,Monaco,Consolas !important;
        color: #808080 !important;
    }
&lt;/style></code></pre>
{{~ end ~}}

The code above would result in the following legend:

![custom](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/legend-custom-style.png)

## Custom template

{{~ if xaml || blazor ~}}
If you need to customize more, you can also use the create your own template:
{{~ end ~}}

{{~ if avalonia ~}}
{{~ "~/../samples/AvaloniaSample/General/TemplatedLegends/View.axaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultLegend.axaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultLegend.axaml.cs)).
:::

{{~ end ~}}

{{~ if blazor ~}}
{{~ "~/../samples/BlazorSample/Pages/General/TemplatedLegends.razor" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([view](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultLegend.razor), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultLegend.razor.cs)).
:::

{{~ end ~}}

{{~ if maui ~}}
{{~ "~/../samples/MauiSample/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultLegend.xaml.cs)).
:::

{{~ end ~}}

{{~ if uwp ~}}
{{~ "~/../samples/UWPSample/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.UWP/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.UWP/DefaultLegend.xaml.cs)).
:::

{{~ end ~}}

{{~ if winforms ~}}
You can create your own legend control, the key is that your control must implement `IChartLegend<SkiaSharpDrawingContext>` and then
you have to create a new instance of that control when your chart initializes.

Add a new form to your app named `CustomLegend`, then change the code behind as follows:

{{~ "~/../samples/WinFormsSample/General/TemplatedLegends/CustomLegend.cs" | render_file_as_code ~}}

Your legend is ready to be used, now when you create a chart, we have to pass a new instance of the tooltip we just created.

<pre><code>var cartesianChart = new CartesianChart(legend: new CustomLegend())
{
    Series = viewModel.Series
};</code></pre>

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultLegend.axaml.cs), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultLegend.axaml.cs)).
:::

{{~ end ~}}

{{~ if winui ~}}
{{~ "~/../samples/WinUISample/WinUI/WinUI/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultLegend.xaml.cs)).
:::

{{~ end ~}}

{{~ if wpf ~}}
{{~ "~/../samples/WPFSample/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultLegend.xaml.cs)).
:::

{{~ end ~}}

{{~ if xamarin ~}}
{{~ "~/../samples/XamarinSample/XamarinSample/XamarinSample/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultLegend.xaml.cs)).
:::

{{~ end ~}}

![custom tooltip](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/legend-custom-template.png)