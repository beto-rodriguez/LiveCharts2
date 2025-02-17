# Customize default legends

You can quickly change the position, the font, the text size or the background color:

{{~ if xaml ~}}
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Axes/Multiple/$PlatformViewFile" ~}}
{{~ end ~}}

{{~ if winforms ~}}
{{~ render_params_file_as_code this "~/../samples/WinFormsSample/Axes/Multiple/View.cs" ~}}
{{~ end ~}}

{{~ if eto ~}}
{{~ render_params_file_as_code this "~/../samples/EtoFormsSample/Axes/Multiple/View.cs" ~}}
{{~ end ~}}

{{~ if blazor ~}}
{{~ render_params_file_as_code this "~/../samples/BlazorSample/Pages/Axes/Multiple.razor" ~}}
{{~ end ~}}

#### View model

```c#
[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } = { ... };
    public Axis[] YAxes { get; set; } = { ... };

    public SolidColorPaint LegendTextPaint { get; set; } = // mark
        new SolidColorPaint // mark
        { // mark
            Color = new SKColor(50, 50, 50), // mark
            SKTypeface = SKTypeface.FromFamilyName("Courier New") // mark
        }; // mark

    public SolidColorPaint LegendBackgroundPaint { get; set; } = // mark
        new SolidColorPaint(new SKColor(240, 240, 240)); // mark
}
```

![custom]({{ assets_url }}/docs/_assets/legend-custom-style.png)

# Override the default legend behavior

You can inherit from `SKDefaultLegend` and override the `GetLayout()` method to define your own template,
in the next example we set a larger miniature compared with the default size.

#### CustomLegend.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedLegends/CustomLegend.cs" ~}}

#### LegendItem.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedLegends/LegendItem.cs" ~}}

#### View

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/TemplatedLegends/$PlatformViewFile" ~}}

![custom legend]({{ assets_url }}/docs/_assets/legend-custom-template.png)

# Legend control from scratch

You can also create your own legend, the recommended way is to use the LiveCharts API but you can
use anything as legend as soon as it implements the `IChartLegend` interface; You can use the [SKDefaultLegend source code](https://github.com/beto-rodriguez/LiveCharts2/blob/master/src/skiasharp/LiveChartsCore.SkiaSharp/SKCharts/SKDefaultLegend.cs) as a guide to build your own implementation, this class is the default legend used by the library.

Instead of using the LiveCharts API you can use your UI framework, 
see [#1558](https://github.com/beto-rodriguez/LiveCharts2/issues/1558) for more info, in that ticket, there is an example 
that implements the `IChartTooltip` interface on a WPF control, then LiveCharts uses this WPF control as the tooltip, even
that example implements `IChartTooltip`, there are no big differences from creating a control that implements `IChartLegend`.
