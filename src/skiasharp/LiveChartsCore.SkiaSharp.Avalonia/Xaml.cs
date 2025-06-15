using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
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

namespace LiveChartsCore.SkiaSharpView.Avalonia;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

// It seems that we should inherit from Styled element since that is all the level of abstraction we need.
// but we cant.... because to use datatemplates we need to inherit from Control, so we do that instead.
// maybe not a big issue?
[XamlClass(typeof(Axis))]
public partial class XamlAxis : Control, ICartesianAxis { }

[XamlClass(typeof(PolarAxis))]
public partial class XamlPolarAxis : Control, IPolarAxis { }

[XamlClass(typeof(DateTimeAxis), GenerateBaseTypeDeclaration = false, GenerateOnChange = false)]
public partial class XamlDateTimeAxis : Control, ICartesianAxis
{
    private readonly DateTimeAxis _baseType = new(TimeSpan.FromDays(1), date => date.ToString("d"));
    private static readonly DateTimeAxis _defaultDateTimeAxis = new(TimeSpan.FromDays(1), date => date.ToString("d"));

    private static readonly XamlProperty<TimeSpan> interval = new(TimeSpan.FromDays(1), XamlGeneration.OnAxisIntervalChanged);
    private static readonly XamlProperty<Func<DateTime, string>> dateFormatter = new(null, XamlGeneration.OnDateTimeAxisDateFormatterChanged);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        MapChangeToBaseType(change.Property.Name);
        OnXamlPropertyChanged(change);
    }
}

[XamlClass(typeof(TimeSpanAxis), GenerateBaseTypeDeclaration = false, GenerateOnChange = false)]
public partial class XamlTimeSpanAxis : Control, ICartesianAxis
{
    private readonly TimeSpanAxis _baseType = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");
    private static readonly TimeSpanAxis _defaultTimeSpanAxis = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");

    private static readonly XamlProperty<TimeSpan> interval = new(TimeSpan.FromSeconds(1), XamlGeneration.OnAxisIntervalChanged);
    private static readonly XamlProperty<Func<TimeSpan, string>> timeFormatter = new(null, XamlGeneration.OnTimeSpanAxisFormatterChanged);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        MapChangeToBaseType(change.Property.Name);
        OnXamlPropertyChanged(change);
    }
}

[XamlClass(typeof(LogarithmicAxis), GenerateBaseTypeDeclaration = false, GenerateOnChange = false)]
public partial class XamlLogarithmicAxis : Control, ICartesianAxis
{
    private readonly LogarithmicAxis _baseType = new(10);
    private static readonly LogarithmicAxis _defaultLogarithmicAxis = new(10);

    private static readonly XamlProperty<double> logBase = new(10d, XamlGeneration.OnAxisLogBaseChanged);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        MapChangeToBaseType(change.Property.Name);
        OnXamlPropertyChanged(change);
    }
}

[XamlClass(typeof(DrawnLabelVisual), Map = typeof(LabelGeometry), MapPath = "DrawnLabel")]
public partial class XamlDrawnLabelVisual : Control, IChartElement, IInternalInteractable
{
    private static readonly LabelGeometry _defaultDrawnLabel = new();
    private LabelGeometry? DrawnLabel => (LabelGeometry?)_baseType.DrawnElement;
}

[XamlClass(typeof(RectangularSection))]
public partial class XamlRectangularSection : Control, IChartElement { }

public class SharedAxesPair : BaseSharedAxesPair { }

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
        theme.Setup(Application.Current?.ActualThemeVariant == ThemeVariant.Dark);
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

//[ContentProperty(nameof(Setters))] // <- how dis in avalonia?
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

#region stepline series

/// <inheritdoc cref="XamlStepLineSeries{TModel, TVisual, TLabel}" />
public class XamlStepLineSeries : XamlStepLineSeries<object, CircleGeometry, LabelGeometry> { }

/// <inheritdoc cref="XamlStepLineSeries{TModel, TVisual, TLabel}" />
public class XamlStepLineSeries<TModel> : XamlStepLineSeries<TModel, CircleGeometry, LabelGeometry>
{ }

/// <inheritdoc cref="XamlStepLineSeries{TModel, TVisual, TLabel}" />
public class XamlStepLineSeries<TModel, TVisual> : XamlStepLineSeries<TModel, TVisual, LabelGeometry>
    where TVisual : BoundedDrawnGeometry, new()
{ }

[XamlClass(typeof(StepLineSeries<,,>), PropertyTypeOverride = Info.PropertyTypeOverride, PropertyChangeMap = Info.PropertyChangeMap)]
public partial class XamlStepLineSeries<TModel, TVisual, TLabel> : XamlSeries, IStepLineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    protected override ISeries WrappedSeries => _baseType;
    static partial void OnTypeDefined() => Info.ConfigureDefaults(_defaultStepLineSeries);
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
