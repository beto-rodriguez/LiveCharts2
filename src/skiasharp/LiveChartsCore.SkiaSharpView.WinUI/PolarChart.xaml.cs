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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IPolarChartView{TDrawingContext}" />
public sealed partial class PolarChart : UserControl, IPolarChartView<SkiaSharpDrawingContext>
{
    #region fields

    private Chart<SkiaSharpDrawingContext>? _core;
    private MotionCanvas? _canvas;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<IPolarAxis> _angleObserver;
    private readonly CollectionDeepObserver<IPolarAxis> _radiusObserver;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    public PolarChart()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        InitializeComponent();

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _angleObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _radiusObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        SetValue(AngleAxesProperty, new ObservableCollection<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            });
        SetValue(RadiusAxesProperty, new ObservableCollection<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            });
        SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetValue(VisualElementsProperty, new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>());
        SetValue(SyncContextProperty, new object());
    }

    #region dependency properties

    /// <summary>
    /// The fit to bounds property.
    /// </summary>
    public static readonly DependencyProperty FitToBoundsProperty =
        DependencyProperty.Register(
            nameof(FitToBounds), typeof(bool), typeof(PolarChart),
            new PropertyMetadata(false, OnDependencyPropertyChanged));

    /// <summary>
    /// The total angle property.
    /// </summary>
    public static readonly DependencyProperty TotalAngleProperty =
        DependencyProperty.Register(
            nameof(TotalAngle), typeof(double), typeof(PolarChart),
            new PropertyMetadata(360d, OnDependencyPropertyChanged));

    /// <summary>
    /// The Inner radius property.
    /// </summary>
    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register(
            nameof(InnerRadius), typeof(double), typeof(PolarChart),
            new PropertyMetadata(0d, OnDependencyPropertyChanged));

    /// <summary>
    /// The initial rotation property.
    /// </summary>
    public static readonly DependencyProperty InitialRotationProperty =
        DependencyProperty.Register(
            nameof(InitialRotation), typeof(double), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.PolarInitialRotation, OnDependencyPropertyChanged));

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title), typeof(VisualElement<SkiaSharpDrawingContext>), typeof(PolarChart), new PropertyMetadata(null));

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(
            nameof(Series), typeof(IEnumerable<ISeries>), typeof(PolarChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PolarChart)o;
                    var seriesObserver = chart._seriesObserver;
                    seriesObserver?.Dispose((IEnumerable<ISeries>)args.OldValue);
                    seriesObserver?.Initialize((IEnumerable<ISeries>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly DependencyProperty VisualElementsProperty =
        DependencyProperty.Register(
            nameof(VisualElements), typeof(IEnumerable<ChartElement<SkiaSharpDrawingContext>>), typeof(PolarChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PolarChart)o;
                    var observer = chart._visualsObserver;
                    observer?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.OldValue);
                    observer?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The x axes property.
    /// </summary>
    public static readonly DependencyProperty AngleAxesProperty =
        DependencyProperty.Register(
            nameof(AngleAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PolarChart)o;
                    var observer = chart._angleObserver;
                    observer?.Dispose((IEnumerable<IPolarAxis>)args.OldValue);
                    observer?.Initialize((IEnumerable<IPolarAxis>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The y axes property.
    /// </summary>
    public static readonly DependencyProperty RadiusAxesProperty =
        DependencyProperty.Register(
            nameof(RadiusAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PolarChart)o;
                    var observer = chart._radiusObserver;
                    observer?.Dispose((IEnumerable<IPolarAxis>)args.OldValue);
                    observer?.Initialize((IEnumerable<IPolarAxis>)args.NewValue);
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly DependencyProperty SyncContextProperty =
        DependencyProperty.Register(
            nameof(SyncContext), typeof(object), typeof(PolarChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PolarChart)o;
                    if (chart._canvas != null) chart.CoreCanvas.Sync = args.NewValue;
                    if (chart._core == null) return;
                    chart._core.Update();
                }));

    /// <summary>
    /// The animations speed property.
    /// </summary>
    public static readonly DependencyProperty AnimationsSpeedProperty =
        DependencyProperty.Register(
            nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.AnimationsSpeed, OnDependencyPropertyChanged));

    /// <summary>
    /// The easing function property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.EasingFunction, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend position property.
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
        DependencyProperty.Register(
            nameof(LegendPosition), typeof(LegendPosition), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend background paint property
    /// </summary>
    public static readonly DependencyProperty LegendBackgroundPaintProperty =
        DependencyProperty.Register(
            nameof(LegendBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text paint property
    /// </summary>
    public static readonly DependencyProperty LegendTextPaintProperty =
        DependencyProperty.Register(
            nameof(LegendTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text size property
    /// </summary>
    public static readonly DependencyProperty LegendTextSizeProperty =
        DependencyProperty.Register(
            nameof(LegendTextSize), typeof(double?), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip position property.
    /// </summary>
    public static readonly DependencyProperty TooltipPositionProperty =
       DependencyProperty.Register(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(PolarChart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip background paint property
    /// </summary>
    public static readonly DependencyProperty TooltipBackgroundPaintProperty =
        DependencyProperty.Register(
            nameof(TooltipBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip text paint property
    /// </summary>
    public static readonly DependencyProperty TooltipTextPaintProperty =
        DependencyProperty.Register(
            nameof(TooltipTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tooltip text size property
    /// </summary>
    public static readonly DependencyProperty TooltipTextSizeProperty =
        DependencyProperty.Register(
            nameof(TooltipTextSize), typeof(double?), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The update started command.
    /// </summary>
    public static readonly DependencyProperty UpdateStartedCommandProperty =
       DependencyProperty.Register(
           nameof(UpdateStartedCommand), typeof(ICommand), typeof(PolarChart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer pressed command.
    /// </summary>
    public static readonly DependencyProperty PointerPressedCommandProperty =
       DependencyProperty.Register(
           nameof(PointerPressedCommand), typeof(ICommand), typeof(PolarChart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer released command.
    /// </summary>
    public static readonly DependencyProperty PointerReleasedCommandProperty =
       DependencyProperty.Register(
           nameof(PointerReleasedCommand), typeof(ICommand), typeof(PolarChart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly DependencyProperty PointerMoveCommandProperty =
       DependencyProperty.Register(
           nameof(PointerMoveCommand), typeof(ICommand), typeof(PolarChart),
           new PropertyMetadata(null));

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly DependencyProperty DataPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(DataPointerDownCommand), typeof(ICommand), typeof(PolarChart), new PropertyMetadata(null));

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    public static readonly DependencyProperty ChartPointPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(PolarChart), new PropertyMetadata(null));

    /// <summary>
    /// The visual elements pointer down command property
    /// </summary>
    public static readonly DependencyProperty VisualElementsPointerDownCommandProperty =
        DependencyProperty.Register(
            nameof(VisualElementsPointerDownCommand), typeof(ICommand), typeof(PolarChart), new PropertyMetadata(null));

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
    public event VisualElementsHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => Windows.ApplicationModel.DesignMode.DesignModeEnabled;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => SetValue(BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(value.A, value.R, value.G, value.B)));
    }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => null;
        set => throw new NotImplementedException();
    }

    Margin? IChartView.DrawMargin
    {
        get => null;
        set => throw new NotImplementedException();
    }

    LvcSize IChartView.ControlSize => _canvas == null
        ? throw new Exception("Canvas not found")
        : new LvcSize { Width = (float)_canvas.ActualWidth, Height = (float)_canvas.ActualHeight };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _canvas == null ? throw new Exception("Canvas not found") : _canvas.CanvasCore;

    PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core =>
        _core == null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)_core;

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.FitToBounds" />
    public bool FitToBounds
    {
        get => (bool)GetValue(FitToBoundsProperty);
        set => SetValue(FitToBoundsProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.TotalAngle" />
    public double TotalAngle
    {
        get => (double)GetValue(TotalAngleProperty);
        set => SetValue(TotalAngleProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InnerRadius" />
    public double InnerRadius
    {
        get => (double)GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InitialRotation" />
    public double InitialRotation
    {
        get => (double)GetValue(InitialRotationProperty);
        set => SetValue(InitialRotationProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title" />
    public VisualElement<SkiaSharpDrawingContext>? Title
    {
        get => (VisualElement<SkiaSharpDrawingContext>?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.AngleAxes" />
    public IEnumerable<IPolarAxis> AngleAxes
    {
        get => (IEnumerable<IPolarAxis>)GetValue(AngleAxesProperty);
        set => SetValue(AngleAxesProperty, value);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.RadiusAxes" />
    public IEnumerable<IPolarAxis> RadiusAxes
    {
        get => (IEnumerable<IPolarAxis>)GetValue(RadiusAxesProperty);
        set => SetValue(RadiusAxesProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => (IEnumerable<ChartElement<SkiaSharpDrawingContext>>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
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
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
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

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (_core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        return scaler.ToChartValues(point.X, point.Y);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (_core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        var r = scaler.ToPixels(point.X, point.Y);

        return new LvcPointD { X = (float)r.X, Y = (float)r.Y };
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (_core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return _core is not PolarChart<SkiaSharpDrawingContext> cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(_core, point));
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            () => action());
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LiveCharts.Configure(config => config.UseDefaults());

        var canvas = (MotionCanvas)FindName("motionCanvas");
        _canvas = canvas;

        if (_core is null)
        {
            _core = new PolarChart<SkiaSharpDrawingContext>(this, config => config.UseDefaults(), canvas.CanvasCore);

            if (SyncContext != null)
                _canvas.CanvasCore.Sync = SyncContext;

            if (_core == null) throw new Exception("Core not found!");
            _core.Measuring += OnCoreMeasuring;
            _core.UpdateStarted += OnCoreUpdateStarted;
            _core.UpdateFinished += OnCoreUpdateFinished;

            SizeChanged += OnSizeChanged;

            // We use the behaviours assembly to share to support Uno.WinUI
            var chartBehaviour = new ChartBehaviour();

            chartBehaviour.Pressed += OnPressed;
            chartBehaviour.Moved += OnMoved;
            chartBehaviour.Released += OnReleased;
            chartBehaviour.Pinched += OnPinched;
            chartBehaviour.Exited += OnExited;

            chartBehaviour.On(this);
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

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_core == null) throw new Exception("Core not found!");
        _core.Update();
    }

    private void OnPressed(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        // is this working on all platforms?
        //if (args.KeyModifiers > 0) return;

        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerPressedCommand?.CanExecute(cArgs) == true) PointerPressedCommand.Execute(cArgs);

        _core?.InvokePointerDown(args.Location, args.IsSecondaryPress);
    }

    private void OnMoved(object? sender, Behaviours.Events.ScreenEventArgs args)
    {
        var location = args.Location;

        var cArgs = new PointerCommandArgs(this, new(location.X, location.Y), args.OriginalEvent);
        if (PointerMoveCommand?.CanExecute(cArgs) == true) PointerMoveCommand.Execute(cArgs);

        _core?.InvokePointerMove(location);
    }

    private void OnReleased(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerReleasedCommand?.CanExecute(cArgs) == true) PointerReleasedCommand.Execute(cArgs);

        _core?.InvokePointerUp(args.Location, args.IsSecondaryPress);
    }

    private void OnPinched(object? sender, Behaviours.Events.PinchEventArgs args)
    {
        if (_core is null) return;

        var c = (CartesianChart<SkiaSharpDrawingContext>)_core;
        var p = args.PinchStart;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));
        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, args.Scale, true);
    }

    private void OnExited(object? sender, Behaviours.Events.EventArgs args)
    {
        _core?.InvokePointerLeft();
    }

    private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateFinished?.Invoke(this);
    }

    private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
    {
        if (UpdateStartedCommand is not null)
        {
            var args = new ChartCommandArgs(this);
            if (UpdateStartedCommand.CanExecute(args)) UpdateStartedCommand.Execute(args);
        }

        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
    {
        Measuring?.Invoke(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _core?.Unload();
    }

    private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
        var chart = (PolarChart)o;
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
        var args = new VisualElementsEventArgs<SkiaSharpDrawingContext>(CoreChart, visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
