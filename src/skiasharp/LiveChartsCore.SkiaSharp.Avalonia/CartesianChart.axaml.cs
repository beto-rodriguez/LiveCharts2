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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="ICartesianChartView" />
public class CartesianChart : UserControl, ICartesianChartView
{
    #region fields

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend? legend;

    /// <summary>
    /// The tool tip
    /// </summary>
    protected IChartTooltip? tooltip;

    private Chart? _core;
    private MotionCanvas? _avaloniaCanvas;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _xObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _yObserver;
    private readonly CollectionDeepObserver<CoreSection> _sectionsObserver;
    private readonly CollectionDeepObserver<ChartElement> _visualsObserver;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public CartesianChart()
    {
        InitializeComponent();

        // workaround to detect mouse events.
        // Avalonia do not seem to detect pointer events if background is not set.
        ((IChartView)this).BackColor = LvcColor.FromArgb(0, 0, 0, 0);

        LiveCharts.Configure(config => config.UseDefaults());

        InitializeCore();

        AttachedToVisualTree += CartesianChart_AttachedToVisualTree;

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<CoreSection>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement>(
           OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        XAxes =
        [
            LiveCharts.DefaultSettings.GetProvider().GetDefaultCartesianAxis()
        ];
        YAxes =
        [
            LiveCharts.DefaultSettings.GetProvider().GetDefaultCartesianAxis()
        ];
        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<ChartElement>();

        PointerPressed += CartesianChart_PointerPressed;
        PointerMoved += CartesianChart_PointerMoved;
        PointerReleased += CartesianChart_PointerReleased;
        PointerWheelChanged += CartesianChart_PointerWheelChanged;
        PointerExited += CartesianChart_PointerLeave;

        var pinchGesture = new PinchGestureRecognizer();
        GestureRecognizers.Add(pinchGesture);
        AddHandler(Gestures.PinchEvent, CartesianChart_Pinched);

        DetachedFromVisualTree += CartesianChart_DetachedFromVisualTree;
    }

    #region avalonia/dependency properties

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
       AvaloniaProperty.Register<CartesianChart, Margin?>(nameof(DrawMargin), null, inherits: true);

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly AvaloniaProperty<object> SyncContextProperty =
       AvaloniaProperty.Register<CartesianChart, object>(nameof(SyncContext), new object(), inherits: true);

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly AvaloniaProperty<CoreVisualElement?> TitleProperty =
       AvaloniaProperty.Register<CartesianChart, CoreVisualElement?>(nameof(Title), null, inherits: true);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
       AvaloniaProperty.Register<CartesianChart, IEnumerable<ISeries>>(nameof(Series), [], inherits: true);

    /// <summary>
    /// The x axes property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ICartesianAxis>> XAxesProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ICartesianAxis>>(nameof(XAxes), [], inherits: true);

    /// <summary>
    /// The y axes property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ICartesianAxis>> YAxesProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ICartesianAxis>>(nameof(YAxes), [], inherits: true);

    /// <summary>
    /// The sections property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<CoreSection>> SectionsProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<CoreSection>>(
            nameof(Sections), [], inherits: true);

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ChartElement>> VisualElementsProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ChartElement>>(
            nameof(VisualElements), [], inherits: true);

    /// <summary>
    /// The draw margin frame property
    /// </summary>
    public static readonly AvaloniaProperty<CoreDrawMarginFrame?> DrawMarginFrameProperty =
        AvaloniaProperty.Register<CartesianChart, CoreDrawMarginFrame?>(
            nameof(DrawMarginFrame), null, inherits: true);

    /// <summary>
    /// The zoom mode property
    /// </summary>
    public static readonly AvaloniaProperty<ZoomAndPanMode> ZoomModeProperty =
        AvaloniaProperty.Register<CartesianChart, ZoomAndPanMode>(
            nameof(ZoomMode), LiveCharts.DefaultSettings.ZoomMode, inherits: true);

    /// <summary>
    /// The zooming speed property
    /// </summary>
    public static readonly AvaloniaProperty<double> ZoomingSpeedProperty =
        AvaloniaProperty.Register<CartesianChart, double>(
            nameof(ZoomingSpeed), LiveCharts.DefaultSettings.ZoomSpeed, inherits: true);

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
        AvaloniaProperty.Register<CartesianChart, TimeSpan>(
            nameof(AnimationsSpeed), LiveCharts.DefaultSettings.AnimationsSpeed, inherits: true);

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
        AvaloniaProperty.Register<CartesianChart, Func<float, float>>(
            nameof(EasingFunction), LiveCharts.DefaultSettings.EasingFunction, inherits: true);

    /// <summary>
    /// The tool tip position property
    /// </summary>
    public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
        AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
            nameof(TooltipPosition), LiveCharts.DefaultSettings.TooltipPosition, inherits: true);

    /// <summary>
    /// The tool tip finding strategy property
    /// </summary>
    public static readonly AvaloniaProperty<FindingStrategy> FindingStrategyProperty =
        AvaloniaProperty.Register<CartesianChart, FindingStrategy>(
            nameof(FindingStrategy), LiveCharts.DefaultSettings.FindingStrategy, inherits: true);

    /// <summary>
    /// The tooltip background paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> TooltipBackgroundPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(TooltipBackgroundPaint),
            (Paint?)LiveCharts.DefaultSettings.TooltipBackgroundPaint, inherits: true);

    /// <summary>
    /// The tooltip text paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> TooltipTextPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(TooltipTextPaint),
            (Paint?)LiveCharts.DefaultSettings.TooltipTextPaint, inherits: true);

    /// <summary>
    /// The tooltip text size property
    /// </summary>
    public static readonly AvaloniaProperty<double?> TooltipTextSizeProperty =
        AvaloniaProperty.Register<CartesianChart, double?>(
            nameof(TooltipTextSize), LiveCharts.DefaultSettings.TooltipTextSize, inherits: true);

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
        AvaloniaProperty.Register<CartesianChart, LegendPosition>(
            nameof(LegendPosition), LiveCharts.DefaultSettings.LegendPosition, inherits: true);

    /// <summary>
    /// The legend background paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> LegendBackgroundPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(LegendBackgroundPaint),
            (Paint?)LiveCharts.DefaultSettings.LegendBackgroundPaint, inherits: true);

    /// <summary>
    /// The legend text paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> LegendTextPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(LegendTextPaint),
            (Paint?)LiveCharts.DefaultSettings.LegendTextPaint, inherits: true);

    /// <summary>
    /// The legend text size property
    /// </summary>
    public static readonly AvaloniaProperty<double?> LegendTextSizeProperty =
        AvaloniaProperty.Register<CartesianChart, double?>(
            nameof(LegendTextSize), LiveCharts.DefaultSettings.LegendTextSize, inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> UpdateStartedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(UpdateStartedCommand), null, inherits: true);

    /// <summary>
    /// The pointer pressed command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerPressedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerPressedCommand), null, inherits: true);

    /// <summary>
    /// The pointer released command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerReleasedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerReleasedCommand), null, inherits: true);

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerMoveCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerMoveCommand), null, inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> DataPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(DataPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The hovered points chaanged command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> HoveredPointsChangedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(HoveredPointsChangedCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="ChartPoint"/> pointer down command property
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public static readonly AvaloniaProperty<ICommand?> ChartPointPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(ChartPointPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="CoreVisualElement"/> pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> VisualElementsPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(VisualElementsPointerDownCommand), null, inherits: true);

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

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => Design.IsDesignMode;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public Chart CoreChart => _core ?? throw new Exception("Core not set yet.");

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => _core is null ? throw new Exception("core not found") : _core.Canvas;

    CartesianChartEngine ICartesianChartView.Core =>
        _core is null ? throw new Exception("core not found") : (CartesianChartEngine)_core;

    LvcColor IChartView.BackColor
    {
        get => Background is not ISolidColorBrush b
                ? new LvcColor()
                : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => Background = new SolidColorBrush(new Color(value.R, value.G, value.B, value.A));
    }

    LvcSize IChartView.ControlSize => _avaloniaCanvas is null
        ? new LvcSize()
        : new LvcSize
        {
            Width = (float)_avaloniaCanvas.Bounds.Width,
            Height = (float)_avaloniaCanvas.Bounds.Height
        };

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => (Margin?)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty)!;
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IChartView.Title" />
    public CoreVisualElement? Title
    {
        get => (CoreVisualElement?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty)!;
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.XAxes" />
    public IEnumerable<ICartesianAxis> XAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(XAxesProperty)!;
        set => SetValue(XAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.YAxes" />
    public IEnumerable<ICartesianAxis> YAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(YAxesProperty)!;
        set => SetValue(YAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.Sections" />
    public IEnumerable<CoreSection> Sections
    {
        get => (IEnumerable<CoreSection>)GetValue(SectionsProperty)!;
        set => SetValue(SectionsProperty, value);
    }

    /// <inheritdoc cref="IChartView.VisualElements" />
    public IEnumerable<ChartElement> VisualElements
    {
        get => (IEnumerable<ChartElement>)GetValue(VisualElementsProperty)!;
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame" />
    public CoreDrawMarginFrame? DrawMarginFrame
    {
        get => (CoreDrawMarginFrame?)GetValue(DrawMarginFrameProperty);
        set => SetValue(DrawMarginFrameProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomMode" />
    public ZoomAndPanMode ZoomMode
    {
        get => (ZoomAndPanMode)GetValue(ZoomModeProperty)!;
        set => SetValue(ZoomModeProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed" />
    public double ZoomingSpeed
    {
        get => (double)GetValue(ZoomingSpeedProperty)!;
        set => SetValue(ZoomingSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed
    {
        get => (TimeSpan)GetValue(AnimationsSpeedProperty)!;
        set => SetValue(AnimationsSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.EasingFunction" />
    public Func<float, float>? EasingFunction
    {
        get => (Func<float, float>?)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty)!;
        set => SetValue(TooltipPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get => tooltip; set => tooltip = value; }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    public FindingStrategy FindingStrategy
    {
        get => (FindingStrategy)GetValue(FindingStrategyProperty)!;
        set => SetValue(FindingStrategyProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => ((FindingStrategy)GetValue(FindingStrategyProperty)!).AsOld();
        set => SetValue(FindingStrategyProperty, value.AsNew());
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
    public double? TooltipTextSize
    {
        get => (double?)GetValue(TooltipTextSizeProperty);
        set => SetValue(TooltipTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty)!;
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
    public double? LegendTextSize
    {
        get => (double?)GetValue(LegendTextSizeProperty);
        set => SetValue(LegendTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => legend; set => legend = value; }

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
    /// Gets or sets a command to execute when the pointer is pressed on the chart.
    /// </summary>
    public ICommand? PointerPressedCommand
    {
        get => (ICommand?)GetValue(PointerPressedCommandProperty);
        set => SetValue(PointerPressedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer is released on the chart.
    /// </summary>
    public ICommand? PointerReleasedCommand
    {
        get => (ICommand?)GetValue(PointerReleasedCommandProperty);
        set => SetValue(PointerReleasedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer moves over the chart.
    /// </summary>
    public ICommand? PointerMoveCommand
    {
        get => (ICommand?)GetValue(PointerMoveCommandProperty);
        set => SetValue(PointerMoveCommandProperty, value);
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
    /// Gets or sets a command to execute when the pointer goes down on a visual element(s).
    /// </summary>
    public ICommand? VisualElementsPointerDownCommand
    {
        get => (ICommand?)GetValue(VisualElementsPointerDownCommandProperty);
        set => SetValue(VisualElementsPointerDownCommandProperty, value);
    }

    #endregion

    /// <inheritdoc cref="ICartesianChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");
        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return _core is not CartesianChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((CoreVisualElement)visual).IsHitBy(cc, new(point)));
    }

    void IChartView.InvokeOnUIThread(Action action) =>
        Dispatcher.UIThread.Post(action);

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        var canvas = this.FindControl<MotionCanvas>("canvas")!;

        _avaloniaCanvas = canvas;
        _core = new CartesianChartEngine(
            this, config => config.UseDefaults(), canvas.CanvasCore);

        _core.Measuring += OnCoreMeasuring;
        _core.UpdateStarted += OnCoreUpdateStarted;
        _core.UpdateFinished += OnCoreUpdateFinished;

        legend = LiveCharts.DefaultSettings.GetTheme().DefaultLegend();
        tooltip = LiveCharts.DefaultSettings.GetTheme().DefaultTooltip();

        _core.Update();
    }

    /// <inheritdoc cref="OnPropertyChanged(AvaloniaPropertyChangedEventArgs)"/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_core is null || change.Property.Name == nameof(IsPointerOver)) return;

        if (change.Property.Name == nameof(SyncContext))
        {
            CoreCanvas.Sync = change.NewValue ?? new object();
        }

        if (change.Property.Name == nameof(Series))
        {
            _seriesObserver?.Dispose((IEnumerable<ISeries>?)change.OldValue);
            _seriesObserver?.Initialize((IEnumerable<ISeries>?)change.NewValue);
        }

        if (change.Property.Name == nameof(XAxes))
        {
            _xObserver?.Dispose((IEnumerable<ICartesianAxis>?)change.OldValue);
            _xObserver?.Initialize((IEnumerable<ICartesianAxis>?)change.NewValue);
        }

        if (change.Property.Name == nameof(YAxes))
        {
            _yObserver?.Dispose((IEnumerable<ICartesianAxis>?)change.OldValue);
            _yObserver?.Initialize((IEnumerable<ICartesianAxis>?)change.NewValue);
        }

        if (change.Property.Name == nameof(Sections))
        {
            _sectionsObserver?.Dispose((IEnumerable<CoreSection>?)change.OldValue);
            _sectionsObserver?.Initialize((IEnumerable<CoreSection>?)change.NewValue);
        }

        if (change.Property.Name == nameof(VisualElementsProperty))
        {
            _visualsObserver?.Dispose((IEnumerable<ChartElement>?)change.OldValue);
            _visualsObserver?.Initialize((IEnumerable<ChartElement>?)change.NewValue);
        }

        _core.Update();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update();

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e) => _core?.Update();

    private DateTime _lastPresed;
    private readonly int _tolearance = 50;
    private void CartesianChart_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.KeyModifiers > 0) return;
        var p = e.GetPosition(this);

        if (PointerPressedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerPressedCommand.CanExecute(args)) PointerPressedCommand.Execute(args);
        }

        var isSecondary =
            e.GetCurrentPoint(this).Properties.IsRightButtonPressed ||
            (DateTime.Now - _lastPresed).TotalMilliseconds < 500;

        _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), isSecondary);
        _lastPresed = DateTime.Now;
    }

    private void CartesianChart_PointerMoved(object? sender, PointerEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(_avaloniaCanvas);

        if (PointerMoveCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerMoveCommand.CanExecute(args)) PointerMoveCommand.Execute(args);
        }

        _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void CartesianChart_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(this);

        if (PointerReleasedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerReleasedCommand.CanExecute(args)) PointerReleasedCommand.Execute(args);
        }

        _core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y), e.GetCurrentPoint(this).Properties.IsRightButtonPressed);
    }

    private void CartesianChart_PointerLeave(object? sender, PointerEventArgs e) => _core?.InvokePointerLeft();

    private void CartesianChart_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_core is null) return;

        e.Handled = true;

        var c = (CartesianChartEngine)_core;
        var p = e.GetPosition(this);

        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private float _previousScale = 1;
    private void CartesianChart_Pinched(object? sender, PinchEventArgs e)
    {
        if (_core is null) return;

        var c = (CartesianChartEngine)_core;
        var p = e.ScaleOrigin;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));

        var scale = (float)e.Scale;
        var delta = _previousScale - scale;
        if (Math.Abs(delta) > 0.05) delta = 0; // ignore the first call.
        _previousScale = scale;

        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, 1 - delta, true);
    }

    private void OnCoreUpdateFinished(IChartView chart) => UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart)
    {
        if (UpdateStartedCommand is not null)
        {
            var args = new ChartCommandArgs(this);
            if (UpdateStartedCommand.CanExecute(args)) UpdateStartedCommand.Execute(args);
        }

        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView chart) => Measuring?.Invoke(this);

    private void CartesianChart_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) => _core?.Load();

    private void CartesianChart_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e) => _core?.Unload();

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
        ChartPointPointerDownCommand?.Execute(points.FindClosestTo(pointer));
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

    void IChartView.Invalidate() => CoreCanvas.Invalidate();
}
