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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IChartView{TDrawingContext}" />
public sealed partial class GeoMap : UserControl, IGeoMapView<SkiaSharpDrawingContext>
{
    private readonly GeoMap<SkiaSharpDrawingContext> _core;
    private CollectionDeepObserver<IGeoSeries> _seriesObserver;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap"/> class.
    /// </summary>
    public GeoMap()
    {
        InitializeComponent();
        LiveCharts.Configure(config => config.UseDefaults());
        _core = new GeoMap<SkiaSharpDrawingContext>(this);

        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerWheelChanged += OnWheelChanged;
        PointerExited += OnPointerExited;

        SizeChanged += GeoMap_SizeChanged;

        _seriesObserver = new CollectionDeepObserver<IGeoSeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
            (object? sender, PropertyChangedEventArgs e) => _core.Update(),
            true);

        SetValue(SeriesProperty, Enumerable.Empty<IGeoSeries>());
        SetValue(ActiveMapProperty, Maps.GetWorldMap<SkiaSharpDrawingContext>());
        SetValue(SyncContextProperty, new object());

        Unloaded += GeoMap_Unloaded;
    }

    #region dependency props

    /// <summary>
    /// The active map property
    /// </summary>
    public static readonly DependencyProperty ActiveMapProperty =
        DependencyProperty.Register(nameof(ActiveMap), typeof(CoreMap<SkiaSharpDrawingContext>), typeof(GeoMap),
            new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly DependencyProperty SyncContextProperty =
       DependencyProperty.Register(
           nameof(SyncContext), typeof(object), typeof(GeoMap), new PropertyMetadata(null, OnDependencyPropertyChanged));

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly DependencyProperty ViewCommandProperty =
       DependencyProperty.Register(
           nameof(ViewCommand), typeof(object), typeof(GeoMap), new PropertyMetadata(null,
               (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
               {
                   var chart = (GeoMap)o;
                   chart._core.ViewTo(args.NewValue);
               }));

    /// <summary>
    /// The map projection property
    /// </summary>
    public static readonly DependencyProperty MapProjectionProperty =
        DependencyProperty.Register(nameof(MapProjection), typeof(MapProjection), typeof(GeoMap),
            new PropertyMetadata(MapProjection.Default, OnDependencyPropertyChanged));

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(nameof(Series), typeof(IEnumerable<IGeoSeries>),
            typeof(GeoMap), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
            {
                var chart = (GeoMap)o;
                var seriesObserver = chart._seriesObserver;
                seriesObserver?.Dispose((IEnumerable<IGeoSeries>)args.OldValue);
                seriesObserver?.Initialize((IEnumerable<IGeoSeries>)args.NewValue);
                chart._core.Update();
            }));

    /// <summary>
    /// The stroke property
    /// </summary>
    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(
            nameof(Stroke), typeof(IPaint<SkiaSharpDrawingContext>), typeof(GeoMap),
            new PropertyMetadata(new SolidColorPaint(new SKColor(255, 255, 255, 255)) { IsStroke = true }, OnDependencyPropertyChanged));

    /// <summary>
    /// The fill property
    /// </summary>
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill), typeof(IPaint<SkiaSharpDrawingContext>), typeof(GeoMap),
            new PropertyMetadata(new SolidColorPaint(new SKColor(240, 240, 240, 255)) { IsFill = true }, OnDependencyPropertyChanged));

    #endregion

    #region properties

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
    bool IGeoMapView<SkiaSharpDrawingContext>.DesignerMode => Windows.ApplicationModel.DesignMode.DesignModeEnabled;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ViewCommand" />
    public object? ViewCommand
    {
        get => GetValue(ViewCommandProperty);
        set => SetValue(ViewCommandProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
    public MotionCanvas<SkiaSharpDrawingContext> Canvas => canvas.CanvasCore;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
    public CoreMap<SkiaSharpDrawingContext> ActiveMap
    {
        get => (CoreMap<SkiaSharpDrawingContext>)GetValue(ActiveMapProperty);
        set => SetValue(ActiveMapProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Width"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Width => (float)ActualWidth;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Height"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Height => (float)ActualHeight;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
    public MapProjection MapProjection
    {
        get => (MapProjection)GetValue(MapProjectionProperty);
        set => SetValue(MapProjectionProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
    public IPaint<SkiaSharpDrawingContext>? Stroke
    {
        get => (IPaint<SkiaSharpDrawingContext>)GetValue(StrokeProperty);
        set
        {
            if (value is not null) value.IsStroke = true;
            SetValue(StrokeProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
    public IPaint<SkiaSharpDrawingContext>? Fill
    {
        get => (IPaint<SkiaSharpDrawingContext>)GetValue(FillProperty);
        set
        {
            if (value is not null) value.IsFill = true;
            SetValue(FillProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Series"/>
    public IEnumerable<IGeoSeries> Series
    {
        get => (IEnumerable<IGeoSeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    #endregion

    void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
    {
        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            () => action());
    }

    private void GeoMap_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _core.Update();
    }

    private void GeoMap_Unloaded(object sender, RoutedEventArgs e)
    {
        _core.Unload();
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _ = CapturePointer(e.Pointer);
        var p = e.GetCurrentPoint(this);
        _core?.InvokePointerDown(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(this);
        _core?.InvokePointerMove(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(this);
        _core?.InvokePointerUp(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
        ReleasePointerCapture(e.Pointer);
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _core?.InvokePointerLeft();
    }

    private void OnWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (_core == null) throw new Exception("core not found");
        var p = e.GetCurrentPoint(this);

        _core.ViewTo(
            new ZoomOnPointerView(
                new LvcPoint((float)p.Position.X, (float)p.Position.Y),
                p.Properties.MouseWheelDelta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut));
    }

    private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
        var chart = (GeoMap)o;
        chart._core.Update();
    }
}
