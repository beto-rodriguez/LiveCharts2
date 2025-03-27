// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Paint = LiveChartsCore.Painting.Paint;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="IPieChartView"/>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PieChart : ChartView, IPieChartView
{
    #region fields

    private Chart? _core;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ChartElement> _visualsObserver;
    private IChartLegend? _legend;
    private IChartTooltip? _tooltip;
    private Theme? _chartTheme;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public PieChart()
    {
        InitializeComponent();

        LiveCharts.Configure(config => config.UseDefaults());

        InitializeCore();
        SizeChanged += OnSizeChanged;

        _seriesObserver = new CollectionDeepObserver<ISeries>(
           (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
           (object? sender, PropertyChangedEventArgs e) => _core?.Update());
        _visualsObserver = new CollectionDeepObserver<ChartElement>(
          (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
          (object? sender, PropertyChangedEventArgs e) => _core?.Update());

        SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetValue(VisualElementsProperty, new ObservableCollection<ChartElement>());
        SetValue(SyncContextProperty, new object());

        if (_core is null) throw new Exception("Core not found!");
        _core.Measuring += OnCoreMeasuring;
        _core.UpdateStarted += OnCoreUpdateStarted;
        _core.UpdateFinished += OnCoreUpdateFinished;

        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged += (sender, args) => _core?.ApplyTheme();
    }

    #region bindable properties

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly BindableProperty SyncContextProperty =
        BindableProperty.Create(
            nameof(SyncContext), typeof(object), typeof(PieChart), null, BindingMode.Default, null,
            (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PieChart)o;
                chart.CoreCanvas.Sync = newValue;
                if (chart._core is null) return;
                chart._core.Update();
            });

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title), typeof(IChartElement), typeof(PieChart), null, BindingMode.Default, null, UI.AddToCanvas);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly BindableProperty SeriesProperty =
          BindableProperty.Create(
              nameof(Series), typeof(IEnumerable<ISeries>), typeof(PieChart), new ObservableCollection<ISeries>(), BindingMode.Default, null,
              UI.DeepObserve<PieChart, ISeries>(c => c._seriesObserver));

    /// <summary>
    /// The visual elements property.
    /// </summary>
    public static readonly BindableProperty VisualElementsProperty =
        BindableProperty.Create(
            nameof(VisualElements), typeof(IEnumerable<ChartElement>), typeof(PieChart), new List<ChartElement>(),
            BindingMode.Default, null, (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PieChart)o;
                var observer = chart._visualsObserver;
                observer?.Dispose((IEnumerable<ChartElement>)oldValue);
                observer?.Initialize((IEnumerable<ChartElement>)newValue);
                if (chart._core is null) return;
                chart._core.Update();
            });

    /// <summary>
    /// The initial rotation property
    /// </summary>
    public static readonly BindableProperty InitialRotationProperty =
        BindableProperty.Create(
            nameof(InitialRotation), typeof(double), typeof(PieChart), 0d, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The IsClockwise angle property
    /// </summary>
    public static readonly BindableProperty IsClockwiseProperty =
        BindableProperty.Create(
            nameof(IsClockwise), typeof(bool), typeof(PieChart), true, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The maximum angle property
    /// </summary>
    public static readonly BindableProperty MaxAngleProperty =
        BindableProperty.Create(
            nameof(MaxAngle), typeof(double), typeof(PieChart), 360d, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The total property
    /// </summary>
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(
            nameof(MaxValue), typeof(double), typeof(PieChart), double.NaN, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The start property
    /// </summary>
    public static readonly BindableProperty MinValueProperty =
        BindableProperty.Create(
            nameof(MinValue), typeof(double), typeof(PieChart), 0d, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly BindableProperty DrawMarginProperty =
        BindableProperty.Create(
            nameof(DrawMargin), typeof(Margin), typeof(PieChart), null, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly BindableProperty AnimationsSpeedProperty =
      BindableProperty.Create(
          nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PieChart), LiveCharts.DefaultSettings.AnimationsSpeed);

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly BindableProperty EasingFunctionProperty =
        BindableProperty.Create(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(PieChart), LiveCharts.DefaultSettings.EasingFunction);

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly BindableProperty LegendPositionProperty =
        BindableProperty.Create(
            nameof(LegendPosition), typeof(LegendPosition), typeof(PieChart),
            LiveCharts.DefaultSettings.LegendPosition, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend background property.
    /// </summary>
    public static readonly BindableProperty LegendBackgroundPaintProperty =
        BindableProperty.Create(
            nameof(LegendBackgroundPaint), typeof(Paint), typeof(PieChart),
            LiveCharts.DefaultSettings.LegendBackgroundPaint, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend text paint property.
    /// </summary>
    public static readonly BindableProperty LegendTextPaintProperty =
        BindableProperty.Create(
            nameof(LegendTextPaint), typeof(Paint), typeof(PieChart),
            LiveCharts.DefaultSettings.LegendTextPaint, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend text size property.
    /// </summary>
    public static readonly BindableProperty LegendTextSizeProperty =
        BindableProperty.Create(
            nameof(LegendTextSize), typeof(double), typeof(PieChart),
            LiveCharts.DefaultSettings.LegendTextSize, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip position property;
    /// </summary>
    public static readonly BindableProperty TooltipPositionProperty =
       BindableProperty.Create(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(PieChart),
           LiveCharts.DefaultSettings.TooltipPosition, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tooltip background property.
    /// </summary>
    public static readonly BindableProperty TooltipBackgroundPaintProperty =
        BindableProperty.Create(
            nameof(TooltipBackgroundPaint), typeof(Paint), typeof(PieChart),
            LiveCharts.DefaultSettings.TooltipBackgroundPaint, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tooltip text paint property.
    /// </summary>
    public static readonly BindableProperty TooltipTextPaintProperty =
        BindableProperty.Create(
            nameof(TooltipTextPaint), typeof(Paint), typeof(PieChart),
            LiveCharts.DefaultSettings.TooltipTextPaint, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tooltip text size property.
    /// </summary>
    public static readonly BindableProperty TooltipTextSizeProperty =
        BindableProperty.Create(
            nameof(TooltipTextSize), typeof(double), typeof(PieChart),
            LiveCharts.DefaultSettings.TooltipTextSize, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The update started command.
    /// </summary>
    public static readonly BindableProperty UpdateStartedCommandProperty =
        BindableProperty.Create(
            nameof(UpdateStartedCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The pressed command.
    /// </summary>
    public static readonly BindableProperty PressedCommandProperty =
        BindableProperty.Create(
            nameof(PressedCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The released command.
    /// </summary>
    public static readonly BindableProperty ReleasedCommandProperty =
        BindableProperty.Create(
            nameof(ReleasedCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly BindableProperty MovedCommandProperty =
        BindableProperty.Create(
            nameof(MovedCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly BindableProperty DataPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(DataPointerDownCommand), typeof(ICommand), typeof(PieChart),
            null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The hovered points chaanged command property
    /// </summary>
    public static readonly BindableProperty HoveredPointsChangedCommandProperty =
        BindableProperty.Create(
            nameof(HoveredPointsChangedCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public static readonly BindableProperty ChartPointPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(PieChart), null);

    /// <summary>
    /// The visual elements pointer down command property
    /// </summary>
    public static readonly BindableProperty VisualElementsPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(VisualElementsPointerDownCommand), typeof(ICommand), typeof(PieChart), null);

    #endregion

    #region events

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

    #endregion

    #region properties

    bool IChartView.DesignerMode => false;

    bool IChartView.IsDarkMode => Application.Current?.RequestedTheme == AppTheme.Dark;

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get => _chartTheme; set { _chartTheme = value; _core?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreChart" />
    public override Chart CoreChart => _core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(
                (byte)(b.Color.Alpha * 255), (byte)(b.Color.Red * 255), (byte)(b.Color.Green * 255), (byte)(b.Color.Blue * 255));
        set => Background = new SolidColorBrush(Color.FromRgba(value.R / 255, value.G / 255, value.B / 255, value.A / 255));
    }

    PieChartEngine IPieChartView.Core =>
        _core is null ? throw new Exception("core not found") : (PieChartEngine)_core;

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    LvcSize IChartView.ControlSize => new() { Width = (float)CanvasView.Width, Height = (float)CanvasView.Height };

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => CanvasView.CanvasCore;

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => (Margin)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    /// <inheritdoc cref="IChartView.Title" />
    public IChartElement? Title
    {
        get => (IChartElement?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="IChartView.VisualElements" />
    public IEnumerable<ChartElement> VisualElements
    {
        get => (IEnumerable<ChartElement>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.IsClockwise" />
    public bool IsClockwise
    {
        get => (bool)GetValue(IsClockwiseProperty);
        set => SetValue(IsClockwiseProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.InitialRotation" />
    public double InitialRotation
    {
        get => (double)GetValue(InitialRotationProperty);
        set => SetValue(InitialRotationProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MaxAngle" />
    public double MaxAngle
    {
        get => (double)GetValue(MaxAngleProperty);
        set => SetValue(MaxAngleProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MaxValue" />
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MinValue" />
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed
    {
        get => (TimeSpan)GetValue(AnimationsSpeedProperty);
        set => SetValue(AnimationsSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.EasingFunction" />
    public Func<float, float>? EasingFunction
    {
        get => (Func<float, float>)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendBackgroundPaint" />
    public Paint? LegendBackgroundPaint
    {
        get => (Paint?)GetValue(LegendBackgroundPaintProperty);
        set => SetValue(LegendBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendTextPaint" />
    public Paint? LegendTextPaint
    {
        get => (Paint?)GetValue(LegendTextPaintProperty);
        set => SetValue(LegendTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendTextSize" />
    public double LegendTextSize
    {
        get => (double?)GetValue(LegendTextSizeProperty) ?? LiveCharts.DefaultSettings.LegendTextSize;
        set => SetValue(LegendTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => _legend; set { _legend = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty);
        set => SetValue(TooltipPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint" />
    public Paint? TooltipBackgroundPaint
    {
        get => (Paint?)GetValue(TooltipBackgroundPaintProperty);
        set => SetValue(TooltipBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipTextPaint" />
    public Paint? TooltipTextPaint
    {
        get => (Paint?)GetValue(TooltipTextPaintProperty);
        set => SetValue(TooltipTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipTextSize" />
    public double TooltipTextSize
    {
        get => (double?)GetValue(TooltipTextSizeProperty) ?? LiveCharts.DefaultSettings.TooltipTextSize;
        set => SetValue(TooltipTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get => _tooltip; set { _tooltip = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <summary>
    /// Gets or sets a command to execute when the chart update started.
    /// </summary>
    public ICommand? UpdateStartedCommand
    {
        get => (ICommand?)GetValue(UpdateStartedCommandProperty);
        set => SetValue(UpdateStartedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pressed the chart.
    /// </summary>
    public ICommand? PressedCommand
    {
        get => (ICommand?)GetValue(PressedCommandProperty);
        set => SetValue(PressedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the users released the press on the chart.
    /// </summary>
    public ICommand? ReleasedCommand
    {
        get => (ICommand?)GetValue(ReleasedCommandProperty);
        set => SetValue(ReleasedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer/finger moves over the chart.
    /// </summary>
    public ICommand? MovedCommand
    {
        get => (ICommand?)GetValue(MovedCommandProperty);
        set => SetValue(MovedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a data or data points.
    /// </summary>
    public ICommand? DataPointerDownCommand
    {
        get => (ICommand?)GetValue(DataPointerDownCommandProperty);
        set => SetValue(DataPointerDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the hovered points change.
    /// </summary>
    public ICommand? HoveredPointsChangedCommand
    {
        get => (ICommand?)GetValue(HoveredPointsChangedCommandProperty);
        set => SetValue(HoveredPointsChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public ICommand? ChartPointPointerDownCommand
    {
        get => (ICommand?)GetValue(ChartPointPointerDownCommandProperty);
        set => SetValue(ChartPointPointerDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    public ICommand? VisualElementsPointerDownCommand
    {
        get => (ICommand?)GetValue(VisualElementsPointerDownCommandProperty);
        set => SetValue(VisualElementsPointerDownCommandProperty, value);
    }

    #endregion

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (_core is not PieChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return _core is not PieChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((LiveChartsCore.VisualElements.VisualElement)visual).IsHitBy(_core, new(point)));
    }

    void IChartView.InvokeOnUIThread(Action action) =>
        _ = MainThread.InvokeOnMainThreadAsync(action);

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        _core = new PieChartEngine(this, config => config.UseDefaults(), CanvasView.CanvasCore);
        _core.Update();
    }

    /// <summary>
    /// Called when a bindable property changes.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns></returns>
    protected static void OnBindablePropertyChanged(BindableObject o, object oldValue, object newValue)
    {
        var chart = (PieChart)o;
        if (chart._core is null) return;
        chart._core.Update();
    }

    /// <inheritdoc cref="NavigableElement.OnParentSet"/>
    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent == null)
        {
            _core?.Unload();
            return;
        }

        _core?.Load();
    }

    internal override void OnPressed(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        // not implemented yet?
        // https://github.com/dotnet/maui/issues/16202
        //if (Keyboard.Modifiers > 0) return;

        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PressedCommand?.CanExecute(cArgs) == true) PressedCommand.Execute(cArgs);

        _core?.InvokePointerDown(args.Location, args.IsSecondaryPress);
    }

    internal override void OnMoved(object? sender, Behaviours.Events.ScreenEventArgs args)
    {
        var location = args.Location;

        var cArgs = new PointerCommandArgs(this, new(location.X, location.Y), args.OriginalEvent);
        if (MovedCommand?.CanExecute(cArgs) == true) MovedCommand.Execute(cArgs);

        _core?.InvokePointerMove(location);
    }

    internal override void OnReleased(object? sender, Behaviours.Events.PressedEventArgs args) =>
        _core?.InvokePointerUp(args.Location, args.IsSecondaryPress);

    internal override void OnExited(object? sender, Behaviours.Events.EventArgs args) =>
        _core?.InvokePointerLeft();

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (_core is null) return;
        _core.Update();
    }

    private void OnCoreUpdateFinished(IChartView chart) =>
        UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart)
    {
        if (UpdateStartedCommand is not null)
        {
            var args = new ChartCommandArgs(this);
            if (UpdateStartedCommand.CanExecute(args)) UpdateStartedCommand.Execute(args);
        }

        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView chart) =>
        Measuring?.Invoke(this);

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
#pragma warning disable CS0618 // Type or member is obsolete
        ChartPointPointerDownCommand?.Execute(points.FindClosestTo(pointer));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints)
    {
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

        var args = new HoverCommandArgs(this, newPoints, oldPoints);
        if (HoveredPointsChangedCommand is not null && HoveredPointsChangedCommand.CanExecute(args)) HoveredPointsChangedCommand.Execute(args);
    }

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer)
    {
        var args = new VisualElementsEventArgs(CoreChart, visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }

    void IChartView.Invalidate() =>
        CoreCanvas.Invalidate();
}
