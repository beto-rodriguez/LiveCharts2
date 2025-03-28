// this file defines the xaml classes, most of the work is done automatically by the generator.
// The generator just wraps LiveCharts object in a Xaml object, then when a
// property changes in the Xaml object, it updates the LiveCharts object.

#pragma warning disable IDE1006 // Naming Styles

using System.Collections;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

[XamlClass(typeof(Axis))]
public partial class XamlAxis : EmptyContentView, ICartesianAxis { }

[XamlClass(typeof(DrawnLabelVisual),
    Map = typeof(LabelGeometry),
    MapPath = "DrawnLabel")]
public partial class XamlDrawnLabelVisual : EmptyContentView, IChartElement, IInternalInteractable
{
    private static readonly LabelGeometry _defaultDrawnLabel = new();
    private LabelGeometry? DrawnLabel => (LabelGeometry?)_baseType.DrawnElement;
}

// == About generated series ==

// Note 1.
// The generated series are of Type Series<object, TVisual, TLabel>, where TVisual and TLabels
// are the default visual and label geometries for the series, but instead of using TModel, we use object,
// this is to prevent specifying the type of the series in xaml, it should not have a considerable impact in performance.

// Note 2.
// The main problem with using object, is that mappers are not strongly typed, the Mapping property is of type
// Func<object, int, Coordinate>, instead of Func<TModel, int, Coordinate>, the cast should be done in the mapper,
// ideally as stated in docs, when performance is critical, you must implement IChartEntity to prevent mapping.

// ============================

#region column series

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries : XamlColumnSeries<object, RoundedRectangleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries<TVisual> : XamlColumnSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries<TVisual, TLabel> : XamlColumnSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(ColumnSeries<,,>),
    PropertyTypeOverride = "Values{=}object",
    PropertyChangeHandlers = "Values{=}OnValuesChanged")]
public partial class XamlColumnSeries<TModel, TVisual, TLabel> : EmptyContentView, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    private static void OnValuesChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((ISeries)((XamlColumnSeries<TModel, TVisual, TLabel>)bindable)._baseType).Values = (IEnumerable)newValue;
}

#endregion

#region line series

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries : XamlLineSeries<object, CircleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries<TVisual> : XamlLineSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries<TVisual, TLabel> : XamlLineSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(LineSeries<,,>),
    PropertyTypeOverride = "Values{=}object",
    PropertyChangeHandlers = "Values{=}OnValuesChanged")]
public partial class XamlLineSeries<TModel, TVisual, TLabel> : EmptyContentView, ILineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    private static void OnValuesChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((ISeries)((XamlLineSeries<TModel, TVisual, TLabel>)bindable)._baseType).Values = (IEnumerable)newValue;
}

#endregion
