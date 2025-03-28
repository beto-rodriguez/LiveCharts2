// this file defines the xaml classes, most of the work is done automatically by the generator.
// The generator just wraps LiveCharts object in a Xaml object, then when a
// property changes in the Xaml object, it updates the LiveCharts object.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

using System;
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

[XamlClass(typeof(DateTimeAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlDateTimeAxis : EmptyContentView, ICartesianAxis
{
    private readonly DateTimeAxis _baseType = new(TimeSpan.FromDays(1), date => date.ToString("d"));
    private static readonly DateTimeAxis _defaultDateTimeAxis = new(TimeSpan.FromDays(1), date => date.ToString("d"));

    public static readonly BindableProperty IntervalProperty = BindableProperty.Create(
        nameof(Interval), typeof(TimeSpan), typeof(XamlDateTimeAxis), TimeSpan.FromDays(1),
        propertyChanged: (BindableObject bo, object o, object n) =>
        {
            var axis = (XamlDateTimeAxis)bo;
            var interval = (TimeSpan)n;

            axis._baseType.UnitWidth = interval.Ticks;
            axis._baseType.MinStep = interval.Ticks;
        });

    public static readonly BindableProperty DateFormatterProperty = BindableProperty.Create(
        nameof(Interval), typeof(Func<DateTime, string>), typeof(XamlDateTimeAxis), null,
        propertyChanged: (BindableObject bo, object o, object n) =>
        {
            var axis = (XamlDateTimeAxis)bo;
            var formatter = (Func<DateTime, string>)n;
            axis._baseType.Labeler = value => formatter(value.AsDate());
        });

    public TimeSpan Interval
    {
        get => (TimeSpan)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public Func<DateTime, string> DateFormatter
    {
        get => (Func<DateTime, string>)GetValue(DateFormatterProperty);
        set => SetValue(DateFormatterProperty, value);
    }
}

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
