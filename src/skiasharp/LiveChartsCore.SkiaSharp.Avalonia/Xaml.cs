using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.VisualStates;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

[XamlClass(typeof(Axis))]
public partial class XamlAxis : StyledElement, ICartesianAxis
{

}

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
        theme.Setup(Application.Current?.ActualThemeVariant == global::Avalonia.Styling.ThemeVariant.Dark);
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

public abstract class XamlSeries : StyledElement
{
    protected XamlSeries()
    {
        var observableCollection = new ObservableCollection<ChartPointState>();
        observableCollection.CollectionChanged += OnAdditionalVisualStatesCollectionChanged;
        AdditionalVisualStates = observableCollection;
    }

    public static readonly AvaloniaProperty<ICollection<ChartPointState>> AdditionalVisualStatesProperty =
        AvaloniaProperty.Register<XamlSeries, ICollection<ChartPointState>>(
            nameof(AdditionalVisualStates), inherits: true, defaultValue: null);

    public ICollection<ChartPointState> AdditionalVisualStates
    {
        private set => SetValue(AdditionalVisualStatesProperty, value);
        get => (ICollection<ChartPointState>)GetValue(AdditionalVisualStatesProperty);
    }

    protected abstract ISeries WrappedSeries { get; }

    protected void ValuesMap(object value) => Info.SetValues(WrappedSeries, value);

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
