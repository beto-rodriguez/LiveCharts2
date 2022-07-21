# Custom Legends

There are 2 ways to customize tooltips, styling and templating.

# Styling a legend

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

![custom]({{ assets_url }}/docs/_assets/legend-custom-style.png)

## Templating a legend

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

{{~ if uno || uno-winui ~}}
{{~ "~/../samples/UnoSample/UnoSample.Shared/LiveChartsSamples/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

:::tip
You can find another example at the source code of the `DefaultLegend` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Uno.WinWI/DefaultLegend.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Uno.WInUI/DefaultLegend.xaml.cs)).
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
{{~ "~/../samples/WinUISample/WinUISample/General/TemplatedLegends/View.xaml" | render_file_as_code ~}}

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

![custom tooltip]({{ assets_url }}/docs/_assets/legend-custom-template.png)