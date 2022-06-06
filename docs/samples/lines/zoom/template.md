
{{ render this "~/shared/genericSampleJustGifHeader.md" }}

Zooming and panning is disabled by default, you can enable it by setting the `ZoomMode` property, this property is of type
`LiveChartsCore.Measure.ZoomAndPanMode` (enum) and the options are `X`, `Y`, `Both` and `None` (default), you can learn more
about zooming an panning [here](https://lvcharts.com/docs/{{ platform }}/{{ version }}/CartesianChart.Axes%20properties#zooming-and-panning).

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