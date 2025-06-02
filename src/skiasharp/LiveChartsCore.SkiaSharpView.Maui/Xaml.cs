// this file defines the xaml classes, most of the work is done automatically by the generator.
// The generator just wraps LiveCharts object in a Xaml object, then when a
// property changes in the Xaml object, it updates the LiveCharts object.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0032 // Use auto property

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using LiveChartsCore.VisualStates;
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

[XamlClass(typeof(RectangularSection))]
public partial class XamlRectangularSection : EmptyContentView, IChartElement
{ }

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
        "DataLabelsFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>{,}" +
        "ToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>{,}" +
        "XToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>{,}" +
        "YToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>";

    public const string PropertyChangeMap =
        "Values{=}ValuesMap";

    public static void SetValues(ISeries series, object value) => series.Values = (IEnumerable)value;

    public static void ConfigureDefaults(ISeries series)
    {
        LiveCharts.Configure(config => config.UseDefaults());
        var theme = LiveCharts.DefaultSettings.GetTheme();
        theme.Setup(Application.Current?.RequestedTheme == AppTheme.Dark);
        series.SeriesId = 0;
        var ce = (ChartElement)series;
        ce._isInternalSet = true;
        theme.ApplyStyleToSeries(series);
        ce._isInternalSet = false;

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

[ContentProperty(nameof(Setters))]
public class ChartPointState
{
    /// <summary>
    /// The sate name.
    /// </summary>
    public string Name
    {
        get;
        set
        {
            field = value;
            Setters.Name = value;
        }
    } = "Default";

    /// <summary>
    /// The setters.
    /// </summary>
    public SetterCollection Setters { get; set; } = new() { Name = "Default" };

    public class SetterCollection : ObservableCollection<Set>
    {
        public string Name { get; set; } = string.Empty;

        public VisualStatesDictionary.DrawnPropertiesDictionary AsStatesCollection()
        {
            var setters = new Dictionary<string, DrawnPropertySetter>();
            for (var i = 0; i < Count; i++)
            {
                var set = this[i];
                setters[set.PropertyName] = new DrawnPropertySetter(set.PropertyName, set.Value);
            }

            return new(setters, false);
        }
    }
}

public class Set
{
    public string PropertyName { get; set; } = string.Empty;
    public object Value { get; set; } = string.Empty;
}

public abstract class XamlSeries : EmptyContentView
{
    public static readonly BindableProperty AdditionalVisualStatesProperty = BindableProperty.Create(
        nameof(AdditionalVisualStates), typeof(ICollection<ChartPointState>), typeof(XamlSeries),
        defaultValueCreator: DefaultValueCreator);

    public ICollection<ChartPointState> AdditionalVisualStates =>
        (ICollection<ChartPointState>)GetValue(AdditionalVisualStatesProperty);

    protected abstract ISeries WrappedSeries { get; }

    protected void ValuesMap(object value) => Info.SetValues(WrappedSeries, value);

    private static object DefaultValueCreator(BindableObject bindable)
    {
        var series = (XamlSeries)bindable;
        var observableCollection = new ObservableCollection<ChartPointState>();

        observableCollection.CollectionChanged += series.OnAdditionalVisualStatesCollectionChanged;

        return observableCollection;
    }

    private void OnAdditionalVisualStatesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems ?? Array.Empty<object>())
                {
                    var state = (ChartPointState)item;
                    state.Setters.CollectionChanged += OnSetCollectionChanged;
                    WrappedSeries.VisualStates[state.Name] = state.Setters.AsStatesCollection();
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems ?? Array.Empty<object>())
                {
                    var state = (ChartPointState)item;
                    state.Setters.CollectionChanged -= OnSetCollectionChanged;
                    _ = WrappedSeries.VisualStates.Remove(state.Name);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
            default:
                break;
        }
    }

    private void OnSetCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not ChartPointState.SetterCollection setters) return;

        WrappedSeries.VisualStates[setters.Name] = setters.AsStatesCollection();
    }
}

#region column series

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries : XamlColumnSeries<object, RoundedRectangleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries<TModel> : XamlColumnSeries<TModel, RoundedRectangleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlColumnSeries{TModel, TVisual, TLabel}" />
public class XamlColumnSeries<TModel, TVisual> : XamlColumnSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

[XamlClass(typeof(ColumnSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlColumnSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultColumnSeries);
}

#endregion

#region row series

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries : XamlRowSeries<object, RoundedRectangleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries<TModel> : XamlRowSeries<TModel, RoundedRectangleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlRowSeries{TModel, TVisual, TLabel}" />
public class XamlRowSeries<TModel, TVisual> : XamlRowSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

[XamlClass(typeof(RowSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlRowSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultRowSeries);
}

#endregion

#region line series

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries : XamlLineSeries<object, CircleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries<TModel> : XamlLineSeries<TModel, CircleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlLineSeries{TModel, TVisual, TLabel}" />
public class XamlLineSeries<TModel, TVisual> : XamlLineSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

[XamlClass(typeof(LineSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlLineSeries<TModel, TVisual, TLabel> : XamlSeries, ILineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultLineSeries);
}

#endregion

#region scatter series

/// <inheritdoc cref="XamlScatterSeries{TModel, TVisual, TLabel}" />
public class XamlScatterSeries : XamlScatterSeries<object, CircleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlScatterSeries{TModel, TVisual, TLabel}" />
public class XamlScatterSeries<TModel> : XamlScatterSeries<TModel, CircleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlScatterSeries{TModel, TVisual, TLabel}" />
public class XamlScatterSeries<TModel, TVisual> : XamlScatterSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

[XamlClass(typeof(ScatterSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlScatterSeries<TModel, TVisual, TLabel> : XamlSeries, IScatterSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultScatterSeries);
}

#endregion

#region candle series

/// <inheritdoc cref="XamlCandlesticksSeries{TModel, TVisual, TLabel}" />
public class XamlCandlesticksSeries : XamlCandlesticksSeries<object, CandlestickGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlCandlesticksSeries{TModel, TVisual, TLabel}" />
public class XamlCandlesticksSeries<TModel> : XamlCandlesticksSeries<TModel, CandlestickGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlCandlesticksSeries{TModel, TVisual, TLabel}" />
public class XamlCandlesticksSeries<TModel, TVisual> : XamlCandlesticksSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BaseCandlestickGeometry, new()
{ }

[XamlClass(typeof(CandlesticksSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlCandlesticksSeries<TModel, TVisual, TLabel> : XamlSeries, IFinancialSeries, IInternalSeries
    where TVisual : BaseCandlestickGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultCandlesticksSeries);
}

#endregion

#region box series

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries : XamlBoxSeries<object, BoxGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries<TModel> : XamlBoxSeries<TModel, BoxGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlBoxSeries{TModel, TVisual, TLabel}" />
public class XamlBoxSeries<TModel, TVisual> : XamlBoxSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BaseBoxGeometry, new()
{ }

[XamlClass(typeof(BoxSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlBoxSeries<TModel, TVisual, TLabel> : XamlSeries, IBoxSeries, IInternalSeries
    where TVisual : BaseBoxGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultBoxSeries);
}

#endregion

#region heat series

/// <inheritdoc cref="XamlHeatSeries{TModel, TVisual, TLabel}" />
public class XamlHeatSeries : XamlHeatSeries<object, ColoredRectangleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlHeatSeries{TModel, TVisual, TLabel}" />
public class XamlHeatSeries<TModel> : XamlHeatSeries<TModel, ColoredRectangleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlHeatSeries{TModel, TVisual, TLabel}" />
public class XamlHeatSeries<TModel, TVisual> : XamlHeatSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, IColoredGeometry, new()
{ }

[XamlClass(typeof(HeatSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlHeatSeries<TModel, TVisual, TLabel> : XamlSeries, IHeatSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, IColoredGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultHeatSeries);
}

#endregion

#region pie series

/// <inheritdoc cref="XamlPieSeries{TModel, TVisual, TLabel}" />
public class XamlPieSeries : XamlPieSeries<object, DoughnutGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlPieSeries{TModel, TVisual, TLabel}" />
public class XamlPieSeries<TModel> : XamlPieSeries<TModel, DoughnutGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlPieSeries{TModel, TVisual, TLabel}" />
public class XamlPieSeries<TModel, TVisual> : XamlPieSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BaseDoughnutGeometry, new()
{ }

[XamlClass(typeof(PieSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlPieSeries<TModel, TVisual, TLabel> : XamlSeries, IPieSeries, IInternalSeries
    where TVisual : BaseDoughnutGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultPieSeries);
}

#endregion

#region gauge series

/// <inheritdoc cref="XamlGaugeSeries{TVisual, TLabel}" />
public class XamlGaugeSeries : XamlGaugeSeries<DoughnutGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlGaugeSeries{TVisual, TLabel}" />
public class XamlGaugeSeries<TVisual> : XamlGaugeSeries<TVisual, LabelGeometry>
    where TVisual : BaseDoughnutGeometry, new()
{ }

[XamlClass(typeof(PieSeries<,,>),
    FileHeader = "using TModel = LiveChartsCore.Defaults.ObservableValue;",
    PropertyTypeOverride = Info.PropertyTypeOverride,
    PropertyChangeMap = Info.PropertyChangeMap,
    GenerateBaseTypeDeclaration = false)]
public partial class XamlGaugeSeries<TVisual, TLabel> : XamlSeries, IPieSeries, IInternalSeries
    where TVisual : BaseDoughnutGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    private readonly ObservableValue _value = new(0d);
    private readonly PieSeries<ObservableValue, TVisual, TLabel> _baseType = new();
    private static readonly PieSeries<ObservableValue, TVisual, TLabel> _defaultPieSeries = new();
    protected override ISeries WrappedSeries => _baseType;

    protected PieSeries<ObservableValue, TVisual, TLabel> Base => _baseType;

    public XamlGaugeSeries()
    {
        _baseType.Values = new ObservableCollection<ObservableValue> { _value };
        var baseSeries = (IInternalSeries)_baseType;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge;
        _baseType.DataFactory.ValuesTransform = ValuesTransform;
    }

    public static BindableProperty GaugeValueProperty = BindableProperty.Create(
        nameof(GaugeValue), typeof(double), typeof(XamlGaugeSeries<TVisual, TLabel>), 0d,
        propertyChanged: (bo, o, n) =>
        {
            var series = (XamlGaugeSeries<TVisual, TLabel>)bo;
            series._value.Value = (double)n;
        });

    public double GaugeValue
    {
        get => (double)GetValue(GaugeValueProperty);
        set => SetValue(GaugeValueProperty, value);
    }

    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultPieSeries);

    protected virtual IEnumerable ValuesTransform(Chart chart, IEnumerable values)
    {
        var seriesArray = ((PieChartEngine)chart).Series
            .Where(x => x is IPieSeries pie && !pie.IsFillSeries)
            .ToArray();

        var index = 0;
        foreach (var series in seriesArray)
        {
            if (series == _baseType) break;
            index++;
        }

        var transformedValues = new ObservableValue?[seriesArray.Length];

        for (var i = 0; i < seriesArray.Length; i++)
        {
            transformedValues[i] = i == index
                ? _value
                : null;
        }

        return transformedValues;
    }
}

public class XamlGaugeBackgroundSeries : XamlGaugeSeries<DoughnutGeometry, LabelGeometry>
{
    public XamlGaugeBackgroundSeries()
    {
        ZIndex = -1;
        IsFillSeries = true;
        IsVisibleAtLegend = false;

        var baseSeries = (IInternalSeries)Base;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge | SeriesProperties.GaugeFill;
    }

    protected override IEnumerable ValuesTransform(Chart chart, IEnumerable values)
    {
        var seriesArray = ((PieChartEngine)chart).Series
            .Where(x => x is IPieSeries pie && !pie.IsFillSeries)
            .ToArray();

        var transformedValues = new ObservableValue?[seriesArray.Length];

        for (var i = 0; i < seriesArray.Length; i++)
            transformedValues[i] = new(0);

        return transformedValues;
    }
}

public class XamlAngularGaugeSeries : XamlGaugeSeries<DoughnutGeometry, LabelGeometry>
{
    public XamlAngularGaugeSeries()
    {
        HoverPushout = 0;
        IsHoverable = false;
        DataLabelsPaint = null;
        AnimationsSpeed = TimeSpan.FromSeconds(0);
        IsRelativeToMinValue = true;

        var baseSeries = (IInternalSeries)Base;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge;
    }

    protected override IEnumerable ValuesTransform(Chart chart, IEnumerable values) => values;
}

[XamlClass(typeof(NeedleVisual))]
public partial class XamlNeedle : EmptyContentView, IChartElement, IInternalInteractable
{ }

[XamlClass(typeof(AngularTicksVisual))]
public partial class XamlAngularTicks : EmptyContentView, IChartElement, IInternalInteractable
{ }

#endregion
