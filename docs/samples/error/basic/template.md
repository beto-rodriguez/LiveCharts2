{{ render this "~/shared/genericSampleSimple.md" }}

### Error bard in line series

In the previous sample you can replace the `ColumnSeries<ErrorValue>` with `LineSeries<ErrorValue>` to create 
a line chart with error bars:

<div class="position-relative text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result2.png" alt="sample image 2" />
</div>

In the previous image, the `Fill`, `GeometryFill` and `GeometryStroke` properties are `null`.