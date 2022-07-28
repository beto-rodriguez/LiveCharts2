# Custom tooltips

There are 2 ways to customize tooltips, styling and templating.

## Styling a tooltip

{{~ if xaml ~}}
You can quickly specify the background color, text color, font, size or position using the chart properties.

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Axes/NamedLabels/$PlatformViewFile" ~}}
{{~ end ~}}

{{~ if winforms ~}}
You can quickly specify the background color, text color, font, size or position using the chart properties.

{{~ render_params_file_as_code this "~/../samples/WinFormsSample/Axes/NamedLabels/View.cs" ~}}
{{~ end ~}}

{{~ if blazor ~}}
You can use css to specify the background color, text color, font, size or position using the chart properties.

{{~ render_params_file_as_code this "~/../samples/BlazorSample/Pages/Axes/NamedLabels.razor" ~}}
{{~ end ~}}

![image]({{ assets_url }}/docs/samples/general/customTooltips/styling-tooltips.png)

## Templating tooltips

{{~ if xaml || blazor ~}}
If you need to customize more, you can also pass your own template:
{{~ end ~}}

{{~ if avalonia ~}}
{{~ "~/../samples/AvaloniaSample/General/TemplatedTooltips/View.axaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs)).
:::

{{~ end ~}}

{{~ if blazor ~}}
{{~ "~/../samples/BlazorSample/Pages/General/TemplatedTooltips.razor" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([view](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultTooltip.razor), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/DefaultTooltip.razor.cs)).
:::

{{~ end ~}}

{{~ if maui ~}}
{{~ "~/../samples/MauiSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/DefaultTooltip.xaml.cs)).
:::

{{~ end ~}}

{{~ if uno || uno ~}}
{{~ "~/../samples/UnoSample/UnoSample.Shared/LiveChartsSamples/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Uno/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpView.Uno/DefaultTooltip.xaml.cs)).
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
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/DefaultTooltip.axaml.cs)).
:::

{{~ end ~}}

{{~ if winui ~}}
{{~ "~/../samples/WinUISample/WinUISample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharpVew.WinUI/DefaultTooltip.xaml.cs)).
:::

{{~ end ~}}

{{~ if wpf ~}}
{{~ "~/../samples/WPFSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.WPF/DefaultTooltip.xaml.cs)).
:::

{{~ end ~}}

{{~ if xamarin ~}}
{{~ "~/../samples/XamarinSample/XamarinSample/XamarinSample/General/TemplatedTooltips/View.xaml" | render_file_as_code ~}}

:::tip
You can find a another example at the source code of the `DefaultTooltip` class 
([xaml](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultTooltip.xaml), 
[code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/DefaultTooltip.xaml.cs)).
:::

{{~ end ~}}

![custom tooltip]({{ assets_url }}/docs/_assets/tooltip-custom-template.png)
