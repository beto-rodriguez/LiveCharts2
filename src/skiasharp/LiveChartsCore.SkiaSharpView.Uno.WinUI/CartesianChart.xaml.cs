
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
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using LiveChartsCore.SkiaSharpView.WinUI.Helpers;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Input;
using LiveChartsCore.VisualElements;
using System.Linq;
using LiveChartsCore.SkiaSharpView.SKCharts;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="CartesianChart{TDrawingContext}"/>
public sealed partial class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>
{
    #region fields

    private Chart<SkiaSharpDrawingContext>? _core;
    private MotionCanvas? _motionCanvas;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _xObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _yObserver;
    private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;
    private DateTime _panLocketUntil;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    public CartesianChart()
    {
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(config => config.UseDefaults());

        InitializeComponent();

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        SetValue(XAxesProperty, new ObservableCollection<ICartesianAxis>()
        {
            LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
        });
        SetValue(YAxesProperty, new ObservableCollection<ICartesianAxis>()
        {
            LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
        });
        SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetValue(SectionsProperty, new ObservableCollection<Section<SkiaSharpDrawingContext>>());
        SetValue(VisualElementsProperty, new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>());
    }

    #region dependency properties

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title), typeof(VisualElement<SkiaSharpDrawingContext>), typeof(CartesianChart), new PropertyMetadata(null));

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(
            nameof(Series), typeof(IEnumerable<ISeries>), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    var seriesObserver = chart._seriesObserver;
                    seriesObserver?.Dispose((IEnumerable<ISeries>)args.OldValue);
                    seriesObserver?.Initialize((IEnumerable<ISeries>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The x axes property
    /// </summary>
    public static readonly DependencyProperty XAxesProperty =
        DependencyProperty.Register(
            nameof(XAxes), typeof(IEnumerable<ICartesianAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart._xObserver;
                    observer?.Dispose((IEnumerable<ICartesianAxis>)args.OldValue);
                    observer?.Initialize((IEnumerable<ICartesianAxis>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The y axes property
    /// </summary>
    public static readonly DependencyProperty YAxesProperty =
        DependencyProperty.Register(
            nameof(YAxes), typeof(IEnumerable<ICartesianAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart._yObserver;
                    observer?.Dispose((IEnumerable<ICartesianAxis>)args.OldValue);
                    observer?.Initialize((IEnumerable<ICartesianAxis>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The sections property
    /// </summary>
    public static readonly DependencyProperty SectionsProperty =
        DependencyProperty.Register(
            nameof(Sections), typeof(IEnumerable<Section<SkiaSharpDrawingContext>>), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart._sectionsObserver;
                    observer?.Dispose((IEnumerable<Section<SkiaSharpDrawingContext>>)args.OldValue);
                    observer?.Initialize((IEnumerable<Section<SkiaSharpDrawingContext>>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly DependencyProperty VisualElementsProperty =
        DependencyProperty.Register(
            nameof(VisualElements), typeof(IEnumerable<ChartElement<SkiaSharpDrawingContext>>), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart._visualsObserver;
                    observer?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.OldValue);
                    observer?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The sync context property
    /// </summary>
    public static readonly DependencyProperty SyncContextProperty =
        DependencyProperty.Register(
            nameof(SyncContext), typeof(object), typeof(CartesianChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (CartesianChart)o;
                    if (chart._motionCanvas != null) chart.CoreCanvas.Sync = args.NewValue;
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The zoom mode property
    /// </summary>
    public static readonly DependencyProperty DrawMarginFrameProperty =
        DependencyProperty.Register(
            nameof(DrawMarginFrame), typeof(DrawMarginFrame<SkiaSharpDrawingContext>), typeof(CartesianChart), new PropertyMetadata(null));

    /// <summary>
    /// The zoom mode property
    /// </summary>
    public static readonly DependencyProperty ZoomModeProperty =
        DependencyProperty.Register(
            nameof(ZoomMode), typeof(ZoomAndPanMode), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.ZoomMode));

    /// <summary>
    /// The zooming speed property
    /// </summary>
    public static readonly DependencyProperty ZoomingSpeedProperty =
        DependencyProperty.Register(
            nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.ZoomSpeed));

    /// <summary>
    /// The tool tip finding strategy property
    /// </summary>
    public static readonly DependencyProperty TooltipFindingStrategyProperty =
        DependencyProperty.Register(
            nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipFindingStrategy, OnDependencyPropertyChanged));

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly DependencyProperty DrawMarginProperty =
       DependencyProperty.Register(
           nameof(DrawMargin), typeof(Margin), typeof(CartesianChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly DependencyProperty AnimationsSpeedProperty =
        DependencyProperty.Register(
            nameof(AnimationsSpeed), typeof(TimeSpan), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.AnimationsSpeed, OnDependencyPropertyChanged));

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.EasingFunction, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
        DependencyProperty.Register(
            nameof(LegendPosition), typeof(LegendPosition), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend background paint property
    /// </summary>
    public static readonly DependencyProperty LegendBackgroundPaintProperty =
        DependencyProperty.Register(
            nameof(LegendBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text paint property
    /// </summary>
    public static readonly DependencyProperty LegendTextPaintProperty =
        DependencyProperty.Register(
            nameof(LegendTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text size property
    /// </summary>
    public static readonly DependencyProperty LegendTextSizeProperty =
        DependencyProperty.Register(
            nameof(LegendTextSize), typeof(double?), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip position property
    /// </summary>
    public static readonly DependencyProperty TooltipPositionProperty =
       DependencyProperty.Register(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(CartesianChart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip background paint property
    /// </summary>
    public static readonly DependencyProperty TooltipBackgroundPaintProperty =
        DependencyProperty.Register(
            nameof(TooltipBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip text paint property
    /// </summary>
    public static readonly DependencyProperty TooltipTextPaintProperty =
        DependencyProperty.Register(
            nameof(TooltipTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip text size property
    /// </summary>
    public static readonly DependencyProperty TooltipTextSizeProperty =
        DependencyProperty.Register(
            nameof(TooltipTextSize), typeof(double?), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly DependencyProperty DataPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(DataPointerDownCommand), typeof(ICommand), typeof(CartesianChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    public static readonly DependencyProperty ChartPointPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(CartesianChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    public static readonly DependencyProperty VisualElementsPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(VisualElementsPointerDownCommand), typeof(ICommand), typeof(CartesianChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

    #endregion

    #region events

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElementsPointerDown"/>
    public event VisualElementHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => Windows.ApplicationModel.DesignMode.DesignModeEnabled;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => SetValue(BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(value.A, value.R, value.G, value.B)));
    }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin DrawMargin
    {
        get => (Margin)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    Margin? IChartView.DrawMargin
    {
        get => DrawMargin;
        set => SetValue(DrawMarginProperty, value);
    }

    LvcSize IChartView.ControlSize => _motionCanvas == null
        ? throw new Exception("Canvas not found")
        : new LvcSize { Width = (float)_motionCanvas.ActualWidth, Height = (float)_motionCanvas.ActualHeight };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _motionCanvas == null ? throw new Exception("Canvas not found") : _motionCanvas.CanvasCore;

    CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
        _core == null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)_core;

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title" />
    public VisualElement<SkiaSharpDrawingContext>? Title
    {
        get => (VisualElement<SkiaSharpDrawingContext>?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
    public IEnumerable<ICartesianAxis> XAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(XAxesProperty);
        set => SetValue(XAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
    public IEnumerable<ICartesianAxis> YAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(YAxesProperty);
        set => SetValue(YAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections" />
    public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
    {
        get => (IEnumerable<Section<SkiaSharpDrawingContext>>)GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => (IEnumerable<ChartElement<SkiaSharpDrawingContext>>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame" />
    public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame
    {
        get => (DrawMarginFrame<SkiaSharpDrawingContext>)GetValue(DrawMarginFrameProperty);
        set => SetValue(DrawMarginFrameProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
    public ZoomAndPanMode ZoomMode
    {
        get => (ZoomAndPanMode)GetValue(ZoomModeProperty);
        set => SetValue(ZoomModeProperty, value);
    }

    ZoomAndPanMode ICartesianChartView<SkiaSharpDrawingContext>.ZoomMode
    {
        get => ZoomMode;
        set => SetValue(ZoomModeProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
    public double ZoomingSpeed
    {
        get => (double)GetValue(ZoomingSpeedProperty);
        set => SetValue(ZoomingSpeedProperty, value);
    }

    double ICartesianChartView<SkiaSharpDrawingContext>.ZoomingSpeed
    {
        get => ZoomingSpeed;
        set => SetValue(ZoomingSpeedProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy" />
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty);
        set => SetValue(TooltipFindingStrategyProperty, value);
    }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed
    {
        get => (TimeSpan)GetValue(AnimationsSpeedProperty);
        set => SetValue(AnimationsSpeedProperty, value);
    }

    TimeSpan IChartView.AnimationsSpeed
    {
        get => AnimationsSpeed;
        set => SetValue(AnimationsSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.EasingFunction" />
    public Func<float, float> EasingFunction
    {
        get => (Func<float, float>)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    Func<float, float>? IChartView.EasingFunction
    {
        get => EasingFunction;
        set => SetValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    LegendPosition IChartView.LegendPosition
    {
        get => LegendPosition;
        set => SetValue(LegendPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty);
        set => SetValue(TooltipPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipBackgroundPaint" />
    public IPaint<SkiaSharpDrawingContext>? TooltipBackgroundPaint
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(TooltipBackgroundPaintProperty);
        set => SetValue(TooltipBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextPaint" />
    public IPaint<SkiaSharpDrawingContext>? TooltipTextPaint
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(TooltipTextPaintProperty);
        set => SetValue(TooltipTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextSize" />
    public double? TooltipTextSize
    {
        get => (double?)GetValue(TooltipTextSizeProperty);
        set => SetValue(TooltipTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get; set; } = new SKDefaultTooltip();

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendBackgroundPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendBackgroundPaint
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(LegendBackgroundPaintProperty);
        set => SetValue(LegendBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendTextPaint
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(LegendTextPaintProperty);
        set => SetValue(LegendTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextSize" />
    public double? LegendTextSize
    {
        get => (double?)GetValue(LegendTextSizeProperty);
        set => SetValue(LegendTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; set; } = new SKDefaultLegend();

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a data or data points.
    /// </summary>
    public ICommand DataPointerDownCommand
    {
        get => (ICommand)GetValue(DataPointerDownCommandProperty);
        set => SetValue(DataPointerDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    public ICommand ChartPointPointerDownCommand
    {
        get => (ICommand)GetValue(ChartPointPointerDownCommandProperty);
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

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core == null) throw new Exception("core not found");
        var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)_core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");
        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (_core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return _core is not CartesianChart<SkiaSharpDrawingContext> cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(_core, point));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        if (Tooltip == null || _core == null) return;
        Tooltip.Show(points, _core);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        if (Tooltip == null || _core == null) return;
        _core.ClearTooltipData();
        Tooltip.Hide();
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        UnoPlatformHelpers.InvokeOnUIThread(action, DispatcherQueue);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var canvas = (MotionCanvas)FindName("motionCanvas");

        _motionCanvas = canvas;

        if (_core is null)
        {
            var zoomingSection = new Drawing.Geometries.RectangleGeometry();
            var zoomingSectionPaint = new SolidColorPaint
            {
                IsFill = true,
                Color = new SkiaSharp.SKColor(33, 150, 243, 50),
                ZIndex = int.MaxValue
            };
            zoomingSectionPaint.AddGeometryToPaintTask(canvas.CanvasCore, zoomingSection);
            canvas.CanvasCore.AddDrawableTask(zoomingSectionPaint);

            _core = new CartesianChart<SkiaSharpDrawingContext>(
                this, config => config.UseDefaults(), canvas.CanvasCore, zoomingSection);

            if (SyncContext != null)
                _motionCanvas.CanvasCore.Sync = SyncContext;

            if (_core == null) throw new Exception("Core not found!");
            _core.Measuring += OnCoreMeasuring;
            _core.UpdateStarted += OnCoreUpdateStarted;
            _core.UpdateFinished += OnCoreUpdateFinished;

            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerWheelChanged += OnWheelChanged;
            PointerExited += OnPointerExited;

            SizeChanged += OnSizeChanged;

            _motionCanvas.Pinched += OnCanvasPinched;

            var canvasContainer = (Canvas)FindName("canvasContainer");
            grid.Width = canvasContainer.ActualWidth;
            grid.Height = canvasContainer.ActualHeight;
        }

        _core.Load();
        _core.Update();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _core?.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _core?.Update();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (_core == null) throw new Exception("Core not found!");

        var canvasContainer = (Canvas)FindName("canvasContainer");
        grid.Width = canvasContainer.ActualWidth;
        grid.Height = canvasContainer.ActualHeight;

        _core.Update();
    }

    private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateFinished?.Invoke(this);
    }

    private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
    {
        Measuring?.Invoke(this);
    }

    private void OnPointerPressed(object? sender, PointerRoutedEventArgs e)
    {
        if (e.KeyModifiers > 0) return;
        _ = CapturePointer(e.Pointer);
        var p = e.GetCurrentPoint(this);
        var isRight = false;
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            isRight = properties.IsRightButtonPressed;
        }
        _core?.InvokePointerDown(new LvcPoint((float)p.Position.X, (float)p.Position.Y), isRight);
    }

    private void OnPointerMoved(object? sender, PointerRoutedEventArgs e)
    {
        if (DateTime.Now < _panLocketUntil) return;

        var p = e.GetCurrentPoint(motionCanvas);
        _core?.InvokePointerMove(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
    }

    private void OnPointerReleased(object? sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(this);
        var isRight = false;
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            isRight = properties.IsRightButtonPressed;
        }
        _core?.InvokePointerUp(new LvcPoint((float)p.Position.X, (float)p.Position.Y), isRight);
        ReleasePointerCapture(e.Pointer);
    }

    private void OnPointerExited(object? sender, PointerRoutedEventArgs e)
    {
        HideTooltip();
        _core?.InvokePointerLeft();
    }

    private void OnWheelChanged(object? sender, PointerRoutedEventArgs e)
    {
        if (_core == null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)_core;
        var p = e.GetCurrentPoint(this);

        c.Zoom(
            new LvcPoint(
                (float)p.Position.X, (float)p.Position.Y),
                p.Properties.MouseWheelDelta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private void OnCanvasPinched(object? sender, LiveChartsPinchEventArgs eventArgs)
    {
        if (_core == null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)_core;

        c.Zoom(eventArgs.PinchStart, ZoomDirection.DefinedByScaleFactor, eventArgs.Scale, true);
        _panLocketUntil = DateTime.Now.AddMilliseconds(500);
        //_lastScale = eventArgs.Scale;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        _core?.Unload();
    }

    private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
        var chart = (CartesianChart)o;
        if (chart._core == null) return;

        chart._core.Update();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        var closest = points.FindClosestTo(pointer);
        ChartPointPointerDown?.Invoke(this, closest);
        if (ChartPointPointerDownCommand is not null && ChartPointPointerDownCommand.CanExecute(closest)) ChartPointPointerDownCommand.Execute(closest);
    }

    void IChartView<SkiaSharpDrawingContext>.OnVisualElementPointerDown(
        IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        var args = new VisualElementsEventArgs<SkiaSharpDrawingContext>(visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
