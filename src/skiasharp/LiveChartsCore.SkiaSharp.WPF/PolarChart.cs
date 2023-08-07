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
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="IPolarChartView{TDrawingContext}" />
public class PolarChart : Chart, IPolarChartView<SkiaSharpDrawingContext>
{
    #region fields

    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<IPolarAxis> _angleObserver;
    private readonly CollectionDeepObserver<IPolarAxis> _radiusObserver;

    #endregion

    static PolarChart()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarChart), new FrameworkPropertyMetadata(typeof(PolarChart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    public PolarChart()
    {
        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _angleObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _radiusObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        SetCurrentValue(AngleAxesProperty, new ObservableCollection<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            });
        SetCurrentValue(RadiusAxesProperty, new ObservableCollection<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            });
        SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetCurrentValue(VisualElementsProperty, new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>());
        SetCurrentValue(SyncContextProperty, new object());

        MouseWheel += OnMouseWheel;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;

        tooltip = new SKDefaultTooltip();
    }

    #region dependency properties

    /// <summary>
    /// The fit to bounds property.
    /// </summary>
    public static readonly DependencyProperty FitToBoundsProperty =
        DependencyProperty.Register(nameof(FitToBounds), typeof(bool), typeof(PolarChart),
            new PropertyMetadata(false, OnDependencyPropertyChanged));

    /// <summary>
    /// The total angle property.
    /// </summary>
    public static readonly DependencyProperty TotalAngleProperty =
        DependencyProperty.Register(nameof(TotalAngle), typeof(double), typeof(PolarChart),
            new PropertyMetadata(360d, OnDependencyPropertyChanged));

    /// <summary>
    /// The inner radius property.
    /// </summary>
    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register(nameof(InnerRadius), typeof(double), typeof(PolarChart),
            new PropertyMetadata(0d, OnDependencyPropertyChanged));

    /// <summary>
    /// The initial rotation property.
    /// </summary>
    public static readonly DependencyProperty InitialRotationProperty =
        DependencyProperty.Register(nameof(InitialRotation), typeof(double), typeof(PolarChart),
            new PropertyMetadata(LiveCharts.DefaultSettings.PolarInitialRotation, OnDependencyPropertyChanged));

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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ISeries> ? value : new ObservableCollection<ISeries>();
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<IPolarAxis>
                        ? value
                        : new List<IPolarAxis>()
                        {
                                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
                        };
                }));

    /// <summary>
    /// The y axes property
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
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<IPolarAxis>
                        ? value
                        : new List<IPolarAxis>()
                        {
                                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
                        };
                }));

    #endregion

    #region properties

    PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core =>
        core is null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)core;

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

    #endregion

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        return scaler.ToChartValues(point.X, point.Y);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        var r = scaler.ToPixels(point.X, point.Y);

        return new LvcPointD { X = (float)r.X, Y = (float)r.Y };
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public override IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return core is not PolarChart<SkiaSharpDrawingContext> cc
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

        core = new PolarChart<SkiaSharpDrawingContext>(this, config => config.UseDefaults(), canvas.CanvasCore);
        legend = new SKDefaultLegend(); // Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloaded"/>
    protected override void OnUnloaded()
    {
        core?.Unload();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (core is null) return;
        core.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (core is null) return;
        core.Update();
    }

    private void OnMouseWheel(object? sender, MouseWheelEventArgs e)
    {
        //if (core is null) throw new Exception("core not found");
        //var c = (PolarChart<SkiaSharpDrawingContext>)core;
        //var p = e.GetPosition(this);
        //c.Zoom(new PointF((float)p.X, (float)p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
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
}
