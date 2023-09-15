# Customize default tooltips

The next article is a quick guide on how to customize the default tooltip,if you want to learn more you can read the full
article:

<a href="{{ website_url }}/docs/{{ platform }}/{{ version }}/CartesianChart.Tooltips" class="btn btn-light btn-lg text-primary shadow-sm mb-3">
<b>Go to the full tooltips article</b>
</a>

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

## Tooltip control from scratch

You can also create your own tooltip, the recommended way is to use the LiveCharts API (example bellow) but you can
use anything as tooltip as soon as it implements the `IChartTooltip<T>` interface. AT the following example we build
a custom control to render tooltips in our charts using the LiveCharts API.

## CustomTooltip.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedTooltips/CustomTooltip.cs" ~}}

## View

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/TemplatedTooltips/$PlatformViewFile" ~}}

![custom tooltip]({{ assets_url }}/docs/_assets/tooltip-custom-template.gif)
