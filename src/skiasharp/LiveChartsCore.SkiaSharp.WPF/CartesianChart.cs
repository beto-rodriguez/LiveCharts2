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
using System.Windows;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
public class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
{
    #region fields

    private CollectionDeepObserver<ISeries> _seriesObserver;
    private CollectionDeepObserver<ICartesianAxis> _xObserver;
    private CollectionDeepObserver<ICartesianAxis> _yObserver;
    private CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;

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
               LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            });
        SetCurrentValue(YAxesProperty, new ObservableCollection<ICartesianAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            });
        SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetCurrentValue(SectionsProperty, new ObservableCollection<Section<SkiaSharpDrawingContext>>());

        MouseWheel += OnMouseWheel;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
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
                    seriesObserver.Dispose((IEnumerable<ISeries>)args.OldValue);
                    seriesObserver.Initialize((IEnumerable<ISeries>)args.NewValue);
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
                    observer.Dispose((IEnumerable<ICartesianAxis>)args.OldValue);
                    observer.Initialize((IEnumerable<ICartesianAxis>)args.NewValue);
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ICartesianAxis>
                        ? value
                        : new List<ICartesianAxis>()
                        {
                                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
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
                    observer.Dispose((IEnumerable<ICartesianAxis>)args.OldValue);
                    observer.Initialize((IEnumerable<ICartesianAxis>)args.NewValue);
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ICartesianAxis>
                        ? value
                        : new List<ICartesianAxis>()
                        {
                                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
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
                    observer.Dispose((IEnumerable<Section<SkiaSharpDrawingContext>>)args.OldValue);
                    observer.Initialize((IEnumerable<Section<SkiaSharpDrawingContext>>)args.NewValue);
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
            new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomMode));

    /// <summary>
    /// The zooming speed property
    /// </summary>
    public static readonly DependencyProperty ZoomingSpeedProperty =
        DependencyProperty.Register(
            nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
            new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomSpeed));

    /// <summary>
    /// The tool tip finding strategy property
    /// </summary>
    public static readonly DependencyProperty TooltipFindingStrategyProperty =
        DependencyProperty.Register(
            nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(Chart),
            new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, OnDependencyPropertyChanged));

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
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is null) throw new Exception("core not found");
        var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <exception cref="Exception">canvas not found</exception>
    protected override void InitializeCore()
    {
        if (canvas is null) throw new Exception("canvas not found");

        core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
        legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
        tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloaded"/>
    protected override void OnUnloaded()
    {
        Series = Array.Empty<ISeries>();
        XAxes = Array.Empty<ICartesianAxis>();
        YAxes = Array.Empty<ICartesianAxis>();
        Sections = Array.Empty<RectangularSection>();
        _seriesObserver = null!;
        _xObserver = null!;
        _yObserver = null!;
        _sectionsObserver = null!;
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
        core.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
        core.Update();
    }

    private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _ = CaptureMouse();
        var p = e.GetPosition(this);
        core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var p = e.GetPosition(this);
        core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y));
        ReleaseMouseCapture();
    }

    private void OnMouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)core;
        var p = e.GetPosition(this);
        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }
}
