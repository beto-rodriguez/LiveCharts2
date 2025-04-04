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
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.Maui.ApplicationModel;
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

[XamlClass(typeof(TimeSpanAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlTimeSpanAxis : EmptyContentView, ICartesianAxis
{
    private readonly TimeSpanAxis _baseType = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");
    private static readonly TimeSpanAxis _defaultTimeSpanAxis = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");

    public static readonly BindableProperty IntervalProperty = BindableProperty.Create(
        nameof(Interval), typeof(TimeSpan), typeof(XamlTimeSpanAxis), TimeSpan.FromMicroseconds(1),
        propertyChanged: (BindableObject bo, object o, object n) =>
        {
            var axis = (XamlTimeSpanAxis)bo;
            var interval = (TimeSpan)n;

            axis._baseType.UnitWidth = interval.Ticks;
            axis._baseType.MinStep = interval.Ticks;
        });

    public static readonly BindableProperty TimeFormatterProperty = BindableProperty.Create(
        nameof(Interval), typeof(Func<TimeSpan, string>), typeof(XamlTimeSpanAxis), null,
        propertyChanged: (BindableObject bo, object o, object n) =>
        {
            var axis = (XamlTimeSpanAxis)bo;
            var formatter = (Func<TimeSpan, string>)n;
            axis._baseType.Labeler = value => formatter(value.AsTimeSpan());
        });

    public TimeSpan Interval
    {
        get => (TimeSpan)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public Func<TimeSpan, string> TimeFormatter
    {
        get => (Func<TimeSpan, string>)GetValue(TimeFormatterProperty);
        set => SetValue(TimeFormatterProperty, value);
    }
}

[XamlClass(typeof(LogarithmicAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlLogarithmicAxis : EmptyContentView, ICartesianAxis
{
    private bool _hasCustomLabeler;
    private readonly LogarithmicAxis _baseType = new(10);
    private static readonly LogarithmicAxis _defaultLogarithmicAxis = new(10);

    public static readonly BindableProperty LogBaseProperty = BindableProperty.Create(
        nameof(LogBase), typeof(double), typeof(XamlLogarithmicAxis), 10d,
        propertyChanged: (BindableObject bo, object o, object n) =>
        {
            var axis = (XamlLogarithmicAxis)bo;
            var @base = (double)n;

            axis._baseType.MinStep = 1;
            if (!axis._hasCustomLabeler) axis._baseType.Labeler = value => Math.Pow(@base, value).ToString("N2");
            axis._baseType._logBase = @base;
        });

    public double LogBase
    {
        get => (double)GetValue(LogBaseProperty);
        set => SetValue(LogBaseProperty, value);
    }

    partial void AlsoOnPropertyChanged(string? propertyName)
    {
        if (propertyName == nameof(LogBase)) _hasCustomLabeler = true;
    }
}

[XamlClass(typeof(DrawnLabelVisual), Map = typeof(LabelGeometry), MapPath = "DrawnLabel")]
public partial class XamlDrawnLabelVisual : EmptyContentView, IChartElement, IInternalInteractable
{
    private static readonly LabelGeometry _defaultDrawnLabel = new();
    private LabelGeometry? DrawnLabel => (LabelGeometry?)_baseType.DrawnElement;
}

public class SharedAxesPair
{
    public ICartesianAxis? First
    {
        get;
        set { field = value; OnSet(); }
    }

    public ICartesianAxis? Second
    {
        get;
        set { field = value; OnSet(); }
    }

    private void OnSet()
    {
        if (First is null || Second is null) return;

        // this object does not handle axes removal :(
        SharedAxes.Set(First, Second);
    }
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

// Note 3.
// Bindable properties default value is initialized according to the active LiveCharts theme,
// The default value of bindable properties can not change at runtime, so when theme changes at runtime
// the devault value can not change to the new theme.
// if this a problem for the developer, then a theme in XAML must be implemented.

// ============================

internal static class Info
{
    public const string PropertyTypeOverride =
        "Values{=}object{,}" +
        "DataLabelsFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>";

    public const string PropertyChangeMap =
        "Values{=}ValuesMap";
}

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

[XamlClass(typeof(ColumnSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlColumnSeries<TModel, TVisual, TLabel> : EmptyContentView, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    static partial void OnTypeDefined()
    {
        static string formatter(ChartPoint point) => point.Coordinate.PrimaryValue.ToString();
        _defaultColumnSeries.DataLabelsFormatter = (Func<ChartPoint, string>)formatter;
        ThemeDefaults.ConfigureSeriesDefaults(_defaultColumnSeries);
    }

    private void ValuesMap(object value) => ((ISeries)_baseType).Values = (IEnumerable)value;
}

#endregion

#region row series

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries : XamlRowSeries<object, RoundedRectangleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries<TVisual> : XamlRowSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries<TVisual, TLabel> : XamlRowSeries<object, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(RowSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlRowSeries<TModel, TVisual, TLabel> : EmptyContentView, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    static partial void OnTypeDefined()
    {
        static string formatter(ChartPoint point) => point.Coordinate.PrimaryValue.ToString();
        _defaultRowSeries.DataLabelsFormatter = (Func<ChartPoint, string>)formatter;
        ThemeDefaults.ConfigureSeriesDefaults(_defaultRowSeries);
    }
    private void ValuesMap(object value) => ((ISeries)_baseType).Values = (IEnumerable)value;
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

[XamlClass(typeof(LineSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlLineSeries<TModel, TVisual, TLabel> : EmptyContentView, ILineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    static partial void OnTypeDefined()
    {
        static string formatter(ChartPoint point) => point.Coordinate.PrimaryValue.ToString();
        _defaultLineSeries.DataLabelsFormatter = (Func<ChartPoint, string>)formatter;
        ThemeDefaults.ConfigureSeriesDefaults(_defaultLineSeries);
    }

    private void ValuesMap(object value) => ((ISeries)_baseType).Values = (IEnumerable)value;
}

#endregion

#region box series

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries : XamlBoxSeries<object, BoxGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries<TVisual> : XamlBoxSeries<object, TVisual, LabelGeometry>
    where TVisual : BaseBoxGeometry, new()
{ }

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries<TVisual, TLabel> : XamlBoxSeries<object, TVisual, LabelGeometry>
    where TVisual : BaseBoxGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(BoxSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlBoxSeries<TModel, TVisual, TLabel> : EmptyContentView, IBoxSeries, IInternalSeries
    where TVisual : BaseBoxGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    static partial void OnTypeDefined()
    {
        static string formatter(ChartPoint point) => point.Coordinate.PrimaryValue.ToString();
        _defaultBoxSeries.DataLabelsFormatter = (Func<ChartPoint, string>)formatter;
        ThemeDefaults.ConfigureSeriesDefaults(_defaultBoxSeries);
    }

    private void ValuesMap(object value) => ((ISeries)_baseType).Values = (IEnumerable)value;
}

#endregion

internal class ThemeDefaults
{
    public static void ConfigureSeriesDefaults(ISeries series)
    {
        LiveCharts.Configure(config => config.UseDefaults());
        var theme = LiveCharts.DefaultSettings.GetTheme();
        theme.Setup(Application.Current?.RequestedTheme == AppTheme.Dark);
        series.SeriesId = 0;
        theme.ApplyStyleToSeries(series);

        series.DataLabelsPaint = Paint.Default;
        series.ShowDataLabels = false;

        if (series is IStrokedAndFilled strokedAndFilled)
        {
            strokedAndFilled.Fill = Paint.Default;
            strokedAndFilled.Stroke = Paint.Default;
        }

        if (series is ILineSeries lineSeries)
        {
            lineSeries.GeometryFill = Paint.Default;
            lineSeries.GeometryStroke = Paint.Default;
        }

        if (series is IPolarLineSeries polarLine)
        {
            polarLine.GeometryFill = Paint.Default;
            polarLine.GeometryStroke = Paint.Default;
        }

        if (series is IStepLineSeries stepLine)
        {
            stepLine.GeometryFill = Paint.Default;
            stepLine.GeometryStroke = Paint.Default;
        }

        if (series is IErrorSeries errorSeries)
        {
            errorSeries.ErrorPaint = Paint.Default;
            errorSeries.ShowError = false;
        }

        if (series is IFinancialSeries financial)
        {
            financial.UpStroke = Paint.Default;
            financial.DownStroke = Paint.Default;
            financial.UpFill = Paint.Default;
            financial.DownFill = Paint.Default;
        }
    }
}
