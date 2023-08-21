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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="IChartView{TDrawingContext}" />
public abstract class Chart : Control, IChartView<SkiaSharpDrawingContext>
{
    #region fields

    /// <summary>
    /// The core
    /// </summary>
    protected Chart<SkiaSharpDrawingContext>? core;

    /// <summary>
    /// The canvas
    /// </summary>
    protected MotionCanvas? canvas;

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend<SkiaSharpDrawingContext>? legend;

    /// <summary>
    /// The tool tip
    /// </summary>
    protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    protected Chart()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        MouseDown += Chart_MouseDown;
        MouseMove += OnMouseMove;
        MouseUp += Chart_MouseUp;
        MouseDoubleClick += Chart_MouseDoubleClick;
        MouseLeave += OnMouseLeave;
        Unloaded += Chart_Unloaded;

        SizeChanged += OnSizeChanged;

        Loaded += Chart_Loaded;
    }

    #region dependency properties

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
       DependencyProperty.Register(
           nameof(Title), typeof(VisualElement<SkiaSharpDrawingContext>), typeof(Chart), new PropertyMetadata(null));

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly DependencyProperty SyncContextProperty =
       DependencyProperty.Register(
           nameof(SyncContext), typeof(object), typeof(Chart), new PropertyMetadata(null,
               (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
               {
                   var chart = (Chart)o;
                   if (chart.canvas != null) chart.CoreCanvas.Sync = args.NewValue;
                   if (chart.core is null) return;
                   chart.core.Update();
               }));

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly DependencyProperty DrawMarginProperty =
       DependencyProperty.Register(
           nameof(DrawMargin), typeof(Margin), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly DependencyProperty AnimationsSpeedProperty =
        DependencyProperty.Register(
            nameof(AnimationsSpeed), typeof(TimeSpan), typeof(Chart),
            new PropertyMetadata(LiveCharts.DefaultSettings.AnimationsSpeed, OnDependencyPropertyChanged));

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(Chart),
            new PropertyMetadata(LiveCharts.DefaultSettings.EasingFunction, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
        DependencyProperty.Register(
            nameof(LegendPosition), typeof(LegendPosition), typeof(Chart),
            new PropertyMetadata(LiveCharts.DefaultSettings.LegendPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend background property
    /// </summary>
    public static readonly DependencyProperty LegendBackgroundPaintProperty =
       DependencyProperty.Register(
           nameof(LegendBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.LegendBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text paint property
    /// </summary>
    public static readonly DependencyProperty LegendTextPaintProperty =
       DependencyProperty.Register(
           nameof(LegendTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The legend text size property
    /// </summary>
    public static readonly DependencyProperty LegendTextSizeProperty =
       DependencyProperty.Register(
           nameof(LegendTextSize), typeof(double?), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.LegendTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip position property
    /// </summary>
    public static readonly DependencyProperty TooltipPositionProperty =
       DependencyProperty.Register(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipPosition, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip background property
    /// </summary>
    public static readonly DependencyProperty TooltipBackgroundPaintProperty =
       DependencyProperty.Register(
           nameof(TooltipBackgroundPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipBackgroundPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip text paint property
    /// </summary>
    public static readonly DependencyProperty TooltipTextPaintProperty =
       DependencyProperty.Register(
           nameof(TooltipTextPaint), typeof(IPaint<SkiaSharpDrawingContext>), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextPaint, OnDependencyPropertyChanged));

    /// <summary>
    /// The tool tip text size property
    /// </summary>
    public static readonly DependencyProperty TooltipTextSizeProperty =
       DependencyProperty.Register(
           nameof(TooltipTextSize), typeof(double?), typeof(Chart),
           new PropertyMetadata(LiveCharts.DefaultSettings.TooltipTextSize, OnDependencyPropertyChanged));

    /// <summary>
    /// The update started command.
    /// </summary>
    public static readonly DependencyProperty UpdateStartedCommandProperty =
       DependencyProperty.Register(
           nameof(UpdateStartedCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer down command.
    /// </summary>
    public static readonly DependencyProperty PointerPressedCommandProperty =
       DependencyProperty.Register(
           nameof(PointerPressedCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer up command.
    /// </summary>
    public static readonly DependencyProperty PointerReleasedCommandProperty =
       DependencyProperty.Register(
           nameof(PointerReleasedCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly DependencyProperty PointerMoveCommandProperty =
       DependencyProperty.Register(
           nameof(PointerMoveCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The double click command.
    /// </summary>
    public static readonly DependencyProperty DoubleClickCommandProperty =
       DependencyProperty.Register(
           nameof(DoubleClickCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The data pointer down command.
    /// </summary>
    public static readonly DependencyProperty DataPointerDownCommandProperty =
       DependencyProperty.Register(
           nameof(DataPointerDownCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The chart point pointer down command.
    /// </summary>
    public static readonly DependencyProperty ChartPointPointerDownCommandProperty =
       DependencyProperty.Register(
           nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The visual elements pointer down command.
    /// </summary>
    public static readonly DependencyProperty VisualElementsPointerDownCommandProperty =
       DependencyProperty.Register(
           nameof(VisualElementsPointerDownCommand), typeof(ICommand), typeof(Chart),
           new PropertyMetadata(null));

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly DependencyProperty VisualElementsProperty =
        DependencyProperty.Register(
            nameof(VisualElements), typeof(IEnumerable<ChartElement<SkiaSharpDrawingContext>>), typeof(Chart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (Chart)o;
                    var observer = chart._visualsObserver;
                    observer?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.OldValue);
                    observer?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.NewValue);
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ChartElement<SkiaSharpDrawingContext>>
                    ? value
                    : new List<ChartElement<SkiaSharpDrawingContext>>();
                }));

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
    bool IChartView.DesignerMode => DesignerProperties.GetIsInDesignMode(this);

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => SetValueOrCurrentValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb(value.A, value.R, value.G, value.B)));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title" />
    public VisualElement<SkiaSharpDrawingContext>? Title
    {
        get => (VisualElement<SkiaSharpDrawingContext>?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => (Margin)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    Margin? IChartView.DrawMargin
    {
        get => DrawMargin;
        set => SetValueOrCurrentValue(DrawMarginProperty, value);
    }

    LvcSize IChartView.ControlSize => canvas is null
        ? throw new Exception("Canvas not found")
        : new LvcSize { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas is null ? throw new Exception("Canvas not found") : canvas.CanvasCore;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed
    {
        get => (TimeSpan)GetValue(AnimationsSpeedProperty);
        set => SetValue(AnimationsSpeedProperty, value);
    }

    TimeSpan IChartView.AnimationsSpeed
    {
        get => AnimationsSpeed;
        set => SetValueOrCurrentValue(AnimationsSpeedProperty, value);
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
        set => SetValueOrCurrentValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
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
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get => tooltip; set => tooltip = value; }

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
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get => legend; set => legend = value; }

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
    /// Gets or sets a command to execute when the pointer goes down on the chart.
    /// </summary>
    public ICommand? PointerPressedCommand
    {
        get => (ICommand?)GetValue(PointerPressedCommandProperty);
        set => SetValue(PointerPressedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes up on the chart.
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
    /// Gets or sets a command to execute when a double click happens on a chart.
    /// </summary>
    public ICommand? DoubleClickCommand
    {
        get => (ICommand?)GetValue(DoubleClickCommandProperty);
        set => SetValue(DoubleClickCommandProperty, value);
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
    /// Gets or sets a command to execute when the pointer goes down on a visual element.
    /// </summary>
    public ICommand? VisualElementsPointerDownCommand
    {
        get => (ICommand?)GetValue(VisualElementsPointerDownCommandProperty);
        set => SetValue(VisualElementsPointerDownCommandProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => (IEnumerable<ChartElement<SkiaSharpDrawingContext>>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
    }

    #endregion

    /// <summary>
    /// Called when the template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (Template.FindName("canvas", this) is not MotionCanvas canvas)
            throw new Exception(
                $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

        this.canvas = canvas;

        if (SyncContext != null)
            this.canvas.CanvasCore.Sync = SyncContext;

        InitializeCore();

        if (core is null) throw new Exception("Core not found!");
        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public abstract IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic);

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public abstract IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point);

    internal Point GetCanvasPosition()
    {
        return canvas is null ? throw new Exception("Canvas not found") : canvas.TranslatePoint(new Point(0, 0), this);
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        Dispatcher.Invoke(action);
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected abstract void InitializeCore();

    /// <summary>
    /// Called when a dependency property changes.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    /// <returns></returns>
    protected static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
        var chart = (Chart)o;
        if (chart.core is null) return;
        chart.core.Update();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (core is null) return;
        core.Update();
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

    /// <summary>
    /// Sets the local value of a dependency property, specified by its dependency property identifier.
    /// If the object has not yet finished initializing, does so without changing its value source.
    /// </summary>
    /// <param name="dp">The identifier of the dependency property to set.</param>
    /// <param name="value">The new local value.</param>
    protected void SetValueOrCurrentValue(DependencyProperty dp, object? value)
    {
        if (IsInitialized)
            SetValue(dp, value);
        else
            SetCurrentValue(dp, value);
    }

    private void Chart_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (PointerReleasedCommand is null) return;
        var p = e.GetPosition(this);
        var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
        if (PointerReleasedCommand.CanExecute(args)) PointerReleasedCommand.Execute(args);
    }

    private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (PointerPressedCommand is null) return;
        var p = e.GetPosition(this);
        var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
        if (PointerPressedCommand.CanExecute(args)) PointerPressedCommand.Execute(args);
    }

    private void Chart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DoubleClickCommand is null) return;
        var p = e.GetPosition(this);
        var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
        if (DoubleClickCommand.CanExecute(args)) DoubleClickCommand.Execute(args);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var p = e.GetPosition(this);

        if (PointerMoveCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerMoveCommand.CanExecute(args)) PointerMoveCommand.Execute(args);
        }

        core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        core?.InvokePointerLeft();
    }

    private void Chart_Loaded(object sender, RoutedEventArgs e)
    {
        core?.Load();
    }

    private void Chart_Unloaded(object sender, RoutedEventArgs e)
    {
        core?.Unload();
        OnUnloaded();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        core?.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        core?.Update();
    }

    /// <summary>
    /// Called before the chart is unloaded.
    /// </summary>
    protected virtual void OnUnloaded() { }

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
