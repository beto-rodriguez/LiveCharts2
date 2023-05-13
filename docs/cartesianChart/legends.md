<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Legends

A legend is a visual element that displays a list with the name, stroke and fills of the series in a chart:

![legends]({{ assets_url }}/docs/_assets/legend.png)

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
<pre><code>cartesianChart1.LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom; // mark
// or use Top, Left, Right or Hidden
</code></pre>
{{~ end ~}}

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

    public SolidColorPaint LedgendBackgroundPaint { get; set; } = // mark
        new SolidColorPaint(new SKColor(240, 240, 240)); // mark
}
```

![custom]({{ assets_url }}/docs/_assets/legend-custom-style.png)

## Tooltip control from scratch

You can also create your own legend, the recommended way is to use the LiveCharts API (example bellow) but you can
use anything as tooltip as soon as it implements the `IChartLegend<T>` interface. At the following example we build
a custom control to render legends in our charts using the LiveCharts API.

## CustomLegend.cs

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/TemplatedLegends/CustomLegend.cs" ~}}

## View

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/TemplatedLegends/$PlatformViewFile" ~}}

![custom tooltip]({{ assets_url }}/docs/_assets/legend-custom-template.png)
