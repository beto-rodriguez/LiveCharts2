{{ render this "~/shared/genericSampleJustGifHeader.md" }}

Zooming and panning is disabled by default, you can enable it by setting the `ZoomMode` property, this property is of type
[ZoomAndPanMode](https://lvcharts.com/api/{{ version }}/LiveChartsCore.Measure.ZoomAndPanMode), this type is a flag enum
it means that you can combine the options as you need, you can learn more about zoomming and panning
[here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).


{{~ if wpf || winforms || winui || eto ~}}

:::tip
Use the mouse wheel to zoom in/out, hold click and drag to move the view (panning).
:::

{{~ end ~}}


{{~ if maui || uno || avalonia ~}}

:::tip
On **Windows**, use the mouse wheel to zoom in/out, hold click and drag to move the view (panning).
:::

:::tip
On **MacOS** slide vertically a finger(s) on a magic mouse to zoom in/out, hold click and drag to move the view (panning).
:::

:::tip
On **Android** or **iOS**, pinch the screen in/out to zoom, hold tap and drag to move the view (panning).
:::

{{~ end ~}}


{{~ if uno || avalonia || blazor ~}}

:::tip
On the **Browser**, use the mouse wheel to zoom in/out, hold click and drag to move the view (panning) (no touch devices yet).
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

# Zoom by selection

You can also zoom by selection, this is a feature that allows you to select an area in the chart and zoom in to that area,
when zooming is enabled, this feature is also enabled by default, you can also customize the trigger of this function, you can
find more info [here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/selection-zoom.gif" alt="sample image" />
</div>

{{~ if wpf || winforms || winui || eto ~}}

:::tip
Right click on the chart, hold and drag to select an area on the chart.
:::

{{~ end ~}}


{{~ if maui || uno || avalonia ~}}

:::tip
On **Windows**, right click on the chart, hold and drag to select an area on the chart.
:::

:::tip
On **MacOS**, **Android** or **iOS** double click/tap the chart, hold the last click/tap and drag to select an area.
:::

{{~ end ~}}


{{~ if uno || avalonia || blazor ~}}

:::tip
On the **Browser**, right click on the chart, hold and drag to select an area on the chart (no touch devices yet).
:::

{{~ end ~}}

{{ render this "~/shared/relatedTo.md" }}
