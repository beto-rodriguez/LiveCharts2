{{ render this "~/shared/genericSampleSimpleHeader.md" }}

We can add custom elements to a chart using the `VisualElements` property, this property is of type `IEnumerable<ChartElement>`,
the library provides the `Visual` class, this class inherits from `ChartElement` and handles animations, the `PointerDown` event and
the creation and destruction of the drawn geometries in the canvas.

In the next example, we create a `CartesianChart`, this chart contains multiple visual elements, each visual element is defined below in this article:

## View Model

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

### Basic sample

In the next example, we define the `RectangleVisual` class, this class inherits from `Visual`, then we override the `Measure` method
and the `DrawnElement` property.

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/RectangleVisual.cs" ~}}

In the `Measure` method, we must define the properties of the `DrawnElement`, this method will be called every time the chart is measured;
Now for the `DrawnElement` property, we override the return type using the [covariant returns](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/covariant-returns)
feature, this means that we can override the type of the `DrawnElement` property, but the new type must implement `IDrawnElement`, this interface
is implement by any object drawn by LiveCharts, you can use the geometries already defined in the library 
([here is a list](https://github.com/beto-rodriguez/LiveCharts2/tree/master/src/skiasharp/LiveChartsCore.SkiaSharp/Drawing/Geometries)) 
or you can define your own geometries as soon as they implement this interface.

### Scaled shapes

Normally, we need to scale the drawn shapes based on the chart data, you can use the `ScaleDataToPixels` method to do so:

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/ScaledRectangleVisual.cs" ~}}

### PointerDown event

You can subscribe to the `PointerDown` event to detect when the user pointer goes down on the element.

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/PointerDownAwareVisual.cs" ~}}

### Svg shapes

Use the `VariableSVGPathGeometry` as the `DrawnElement` to draw svg paths:

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/SvgVisual.cs" ~}}

### Themed visuals

In the next example, when the app theme is dark the fill is white, otherwise is black, dark and light variants are detected only
on Avalonia, Maui, Uno and WinUI.

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/ThemedVisual.cs" ~}}

### Custom IDrawnElement

The easiest way is to inherit from `DrawnGeometry`, this class implements `IDrawnElement` and also animates all of its properties;
In the next example we inherit from `BoundedDrawnGeometry` it only adds the `Width` and `Height` properties to the `DrawnGeometry` class.

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/custom.png" alt="sample image" />
</div>

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/CustomSkiaShape.cs" ~}}

Finally, we need to define a `Visual` that uses this geometry as the `DrawnElement`:

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/CustomVisual.cs" ~}}

:::tip
If you need to define your own properties and these properties must be animated, you must use the `MotionProperty<T>` class, please see the
[draw on canvas sample](https://livecharts.dev/docs/{{ platform }}/{{ version }}/samples.general.drawOnCanvas) for more info.
:::

## Layouts

LiveCharts provides a small layout framework, this is used by the library to render the tooltips and legends,
Layouts also implement `IDrawnElement`, but they are not drawn in the UI, instead they just define the coordinates
of the elements inside when an element is added to a layout, the coordinates of these elements are relative to the
layout, [here is a list](https://github.com/beto-rodriguez/LiveCharts2/tree/master/src/skiasharp/LiveChartsCore.SkiaSharp/Drawing/Layouts)
of the layouts defined in the library.

### Container Layout

A container is just a shape that can host other drawn elements as the content of this shape, a container takes the size of its content, and can
set the `Fill` and `Stroke` properties of this shape. an example in the library is the default tooltip, it is of type `Container<PopUpGeometry>`
then it sets the `Geometry.Fill` to define the background of the tooltip.

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/ContainerVisual.cs" ~}}

### Absolute Layout

Used to place children on its own coordinate system, all the children X and Y coordinates are relative to the Layout position, the layout takes
the size of the largest element in the children collection. For example in the next case, we place the place the RectangleGeometry` in the 0,0
coordinate [in the layout system] and the `LabelGeometry` in the 10,0 coordinate.

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/absolute.png" alt="sample image" />
</div>

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/AbsoluteVisual.cs" ~}}

### Stacked Layout

Stacks `IDrawnElement` objects in vertical or horizontal order.

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/stack.png" alt="sample image" />
</div>

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/StackedVisual.cs" ~}}

### Table Layout

Uses a grid system to place `IDrawnElement` objects.

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/table.png" alt="sample image" />
</div>

{{~ render_params_file_as_code this "~/../samples/ViewModelsSamples/General/VisualElements/TableVisual.cs" ~}}

### Update on property change

To do...