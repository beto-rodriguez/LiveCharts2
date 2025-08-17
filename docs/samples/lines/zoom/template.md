{{ render this "~/shared/genericSampleJustGifHeader.md" }}

Zooming and panning is disabled by default, you can enable it by setting the `ZoomMode` property, this property is of type
[ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode), this type is a flag enum
it means that you can combine the options as you need, you can learn more about zooming and panning
[here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).


{{~ if wpf || winforms || winui || blazor || eto ~}}

:::tip
Use the mouse wheel to zoom in/out, hold click and drag to pan.
:::

{{~ end ~}}


{{~ if maui || uno || avalonia ~}}

:::tip
On **Desktop**:

- **Zoom**: Use the scroll gesture — swipe up/down with two fingers on a trackpad or Magic Mouse, or scroll with the mouse wheel — to zoom in and out.
- **Pan**: Click and drag with the mouse, or press and hold while dragging with one finger on a trackpad or Magic Mouse.
:::

:::tip
On **Mobile** and touch screens:
- **Zoom**: Pinch the screen in or out using two fingers.
- **Pan**: Tap and hold, then drag to move the view.
:::

{{~ end ~}}

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

{{~ if blazor ~}}
## HTML
{{~ end~}}

```
{{ full_name | get_view_from_docs }}
```

# Options

You can enable zooming and panning by setting the `ZoomMode` property, this property is of type
[ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) and the options are:

- `None`: Disables zooming and panning.
- `X`: Enables zooming and panning on the X axis.
- `Y`: Enables zooming and panning on the Y axis.
- `Both`: Enables zooming and panning on both axes.
- `NoFit`: Disables the "Fit to Bounds" feature that forces the the chart to bounce back to the data bounds when zooming and panning finishes.
- `NoZoomBySection`: Disables the "Zoom by Section" feature, this feature selects an area and zooms to this area, normally by right clicking or double tapping and then dragging to the end of the section.
- `InvertPanningPointerTrigger`: Inverts the panning and zoom by section pointer triggers, when the flag is present, panning is triggered by right clicking the chart, and zoom by section by left clicking (or inverts single/double taps on mobile).

The [ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) type is a flag enum,
so you can combine the options, for example, if you want to enable zooming on the `X` axis and disable "Fit top Bounds"
and "Zoom by Section" you can set the `ZoomMode` property to:

{{~ if blazor || winforms || eto ~}}

```c#
var flags = ZoomAndPanMode.X | ZoomAndPanMode.NoFit | ZoomAndPanMode.NoZoomBySection;
myChart.ZoomMode = flags;
```

{{~ endif ~}}

{{~ if avalonia || uno || maui || winui || wpf ~}}

```xml
&lt;lvc:CartesianChart ZoomMode="X,NoFit,NoZoomBySection">
&lt;/lvc:CartesianChart>
```

{{~ endif ~}}

# Clearing zooming and panning

Setting the axes `MinLimit` and `MaxLimit` properties to `null` will clear the current `zooming` or `panning`, and will let the chart fit the view
of the chart to the available data in the chart, for example:

```c#
// where myChart is a reference to the chart in the UI
 foreach (var x in myChart.XAxes)
 {
     x.MinLimit = null;
     x.MaxLimit = null;
 }

 foreach (var y in myChart.YAxes)
 {
     y.MinLimit = null;
     y.MaxLimit = null;
 }
```

# Zoom by Section

You can also zoom by section, this is a feature that allows you to select an area in the chart and zoom in to that area,
when zooming is enabled, this feature is also enabled by default, you can also customize the trigger of this function, you can
find more info [here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/selection-zoom.gif" alt="sample image" />
</div>

{{~ if wpf || winforms || winui || blazor || eto ~}}

:::tip
Right click on the chart, hold and drag to select an area in the chart.
:::

{{~ end ~}}


{{~ if maui ~}}

:::tip
On **Windows**, right click on the chart, hold and drag to select an area on the chart.
:::

:::tip
On **MacOS**, **Android** and **iOS** double tap the chart, hold the last tap and drag to select an area.
:::

{{~ end ~}}

{{~ if avalonia ~}}

:::tip
On **Desktop**, right click on the chart, hold and drag to select an area on the chart.
:::

:::tip
On **Desktop** and **Mobile** double tap the chart, hold the last tap and drag to select an area.
:::

{{~ end ~}}

{{~ if uno ~}}

:::tip
On **Desktop**, right click on the chart, hold and drag to select an area on the chart, the right click detection
relies on the Uno implementation of `PointerPointProperties.IsRightButtonPressed`.
:::

:::tip
On **Mobile** double tap the chart, hold the last tap and drag to select an area.
:::

{{~ end ~}}

# Override defaults

You can manually trigger the functions in the chart that trigger Zooming or panning, for example you could set the
`ZoomMode` property to `None`, this will prevent LiveCharts to fire zooming or panning, and then we could fire these features
when we want, for example zooming on double tap or panning on mouse move.

```c#
// where myChart is a reference to the chart in the UI
var engine = (CartesianChartEngine)myChart.CoreChart;

// zooms in in X using the 100,100 as the center of the zoom
engine.Zoom(ZoomAndPanMode.X, new(100, 100), ZoomDirection.ZoomIn);

// pans the chart 10 pixels to the right and 10 pixels down on both axes
engine.Pan(ZoomAndPanMode.Both, new(10, 10));

// starts the zooming section feature at the point (100, 100)
// for example start the section on double tap.
engine.StartZoomingSection(ZoomAndPanMode.Both, new(100, 100));

// grows the zooming section by 10 pixels on both axes
// for example when the pointer moves after the previous double tap.
engine.GrowZoomingSection(ZoomAndPanMode.Both, new(10, 10));

// ends the zooming section feature at the point (110, 110)
// for example on double tap again.
engine.EndZoomingSection(ZoomAndPanMode.Both, new(110, 110));
```

{{ render this "~/shared/relatedTo.md" }}
