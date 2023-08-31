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
using System.Windows;
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
public class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
{
    #region fields

    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _xObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _yObserver;
    private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;

    #endregion

    static CartesianChart()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    public CartesianChart()
    {
        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        SetCurrentValue(XAxesProperty, new ObservableCollection<ICartesianAxis>()
            {
               LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            });
        SetCurrentValue(YAxesProperty, new ObservableCollection<ICartesianAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            });
        SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetCurrentValue(SectionsProperty, new ObservableCollection<Section<SkiaSharpDrawingContext>>());
        SetCurrentValue(VisualElementsProperty, new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>());
        SetCurrentValue(SyncContextProperty, new object());

        MouseWheel += OnMouseWheel;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        ManipulationDelta += OnManipulationDelta;
        ManipulationStarting += OnManipulationStarting;

        tooltip = new SKDefaultTooltip();
        legend = new SKDefaultLegend();
    }

    #region dependency properties

    /// <summary>
    /// The series property
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ISeries> ? value : new ObservableCollection<ISeries>();
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ICartesianAxis>
                        ? value
                        : new List<ICartesianAxis>()
                        {
                                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
                        };
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ICartesianAxis>
                        ? value
                        : new List<ICartesianAxis>()
                        {
                                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
                        };
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<Section<SkiaSharpDrawingContext>>
                    ? value
                    : new List<Section<SkiaSharpDrawingContext>>();
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
            nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(Chart),
            new PropertyMetadata(LiveCharts.DefaultSettings.TooltipFindingStrategy, OnDependencyPropertyChanged));

    #endregion

    #region properties

    CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
        core is null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

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
        set => SetValueOrCurrentValue(ZoomModeProperty, value);
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
        set => SetValueOrCurrentValue(ZoomingSpeedProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy" />
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty);
        set => SetValue(TooltipFindingStrategyProperty, value);
    }

    #endregion

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
    [Obsolete("Use the ScalePixelsToData method instead.")]
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is null) throw new Exception("core not found");
        var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (core is not CartesianChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public override IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return core is not CartesianChart<SkiaSharpDrawingContext> cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(core, point));
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <exception cref="Exception">canvas not found</exception>
    protected override void InitializeCore()
    {
        if (canvas is null) throw new Exception("canvas not found");

        var zoomingSection = new RectangleGeometry();
        var zoomingSectionPaint = new SolidColorPaint
        {
            IsFill = true,
            Color = new SkiaSharp.SKColor(33, 150, 243, 50),
            ZIndex = int.MaxValue
        };
        zoomingSectionPaint.AddGeometryToPaintTask(canvas.CanvasCore, zoomingSection);
        canvas.CanvasCore.AddDrawableTask(zoomingSectionPaint);

        core = new CartesianChart<SkiaSharpDrawingContext>(
            this, config => config.UseDefaults(), canvas.CanvasCore, zoomingSection);

        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloaded"/>
    protected override void OnUnloaded()
    {
        core?.Unload();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        core?.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        core?.Update();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (Keyboard.Modifiers > 0) return;
        _ = CaptureMouse();
        var p = e.GetPosition(this);
        core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), e.ChangedButton == MouseButton.Right);
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(this);
        core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y), e.ChangedButton == MouseButton.Right);
        ReleaseMouseCapture();
    }

    private void OnMouseWheel(object? sender, MouseWheelEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)core;
        var p = e.GetPosition(this);
        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)core;
        if (e.DeltaManipulation.Scale.Y != 1)
        {
            LvcPoint p = new((float)e.ManipulationOrigin.X, (float)e.ManipulationOrigin.Y);
            c.Zoom(p, ZoomDirection.DefinedByScaleFactor, e.DeltaManipulation.Scale.Y, true);
            e.Handled = true;
            return;
        }
        if (e.DeltaManipulation.Translation.X != 0 || e.DeltaManipulation.Translation.Y != 0)
        {
            LvcPoint p = new((float)e.DeltaManipulation.Translation.X, (float)e.DeltaManipulation.Translation.Y);
            c.Pan(p, false);
            e.Handled = true;
            return;
        }
    }
    private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
    {
        e.ManipulationContainer = this;
        e.Handled = true;
    }
}
