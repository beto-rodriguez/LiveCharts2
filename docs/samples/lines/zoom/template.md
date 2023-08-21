{{ render this "~/shared/genericSampleJustGifHeader.md" }}

{{~ if maui ~}}
:::warning
Currently zooming is not working on MAUI on desktop devices, due a MAUI limitation, please show your interest on this feature to the MAUI team at
https://github.com/dotnet/maui/issues/16130.
:::
{{~ end ~}}

Zooming and panning is disabled by default, you can enable it by setting the `ZoomMode` property, this property is of type
[ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) and the options are:

- `X`: Enables zooming and panning on the X axis.
- `Y`: Enables zooming and panning on the Y axis.
- `Both`: Enables zooming and panning on both axes.
- `None`: Disables zooming and panning.
- `PanX`: Enables panning on the X axis.
- `PanY`: Enables panning on the Y axis.
- `ZoomX`: Enables zooming on the X axis.
- `ZoomY`: Enables zooming on the Y axis.

The [ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode) type is a flag enum,
so you can combine the options, for example, if you want to enable zooming on the X axis and panning on the Y axis you can
set the `ZoomMode` property to `ZoomAndPanMode.ZoomX | ZoomAndPanMode.PanY`.

There is also the `InvertPanningPointerTrigger` flag, when this flag is present the panning will be triggered using
the right click on desktop devices and the touch-and-hold gesture on touch devices, the `zoom by section` feature will be
triggered to the left click on desktop devices and the touch-and-hold gesture on touch devices.

You can learn more about zooming an panning [here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).

{{~ if desktop ~}}

:::tip
In desktop, use the mouse wheel to zoom in/out, hold click and drag to move the view (panning).
:::

{{~ end ~}}

{{~ if mobile ~}}

:::info
In touch devices, pinch the screen in/out to zoom, hold tap and drag to move the view (panning).
:::

{{~ end ~}} 

## View model

```
{{ full_name | get_vm_from_docs }}
```

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

{{~ if blazor~}}
## HTML
{{~ end~}}

```
{{ full_name | get_view_from_docs }}
```

{{ render this "~/shared/relatedTo.md" }}
