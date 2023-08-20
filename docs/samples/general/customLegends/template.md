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

## View model

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

## Legend control from scratch

You can also create your own legend, the recommended way is to use the LiveCharts API (example bellow) but you can
use anything as legend as soon as it implements the `IChartLegend<T>` interface. At the following example we build
a custom control to render legends in our charts using the LiveCharts API.

## CustomLegend.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedLegends/CustomLegend.cs" ~}}

## View

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/TemplatedLegends/$PlatformViewFile" ~}}

![custom legend]({{ assets_url }}/docs/_assets/legend-custom-template.png)