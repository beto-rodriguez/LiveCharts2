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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="IGeoMapView{TDrawingContext}"/>
public partial class GeoMap : UserControl, IGeoMapView<SkiaSharpDrawingContext>
{
    private readonly CollectionDeepObserver<IGeoSeries> _seriesObserver;
    private readonly GeoMap<SkiaSharpDrawingContext> _core;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap"/> class.
    /// </summary>
    public GeoMap()
    {
        InitializeComponent();
        LiveCharts.Configure(config => config.UseDefaults());
        _core = new GeoMap<SkiaSharpDrawingContext>(this);
        _seriesObserver = new CollectionDeepObserver<IGeoSeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
            (object? sender, PropertyChangedEventArgs e) => _core?.Update(),
            true);

        Background = new SolidColorBrush(new Color(0, 0, 0, 0));

        PointerWheelChanged += OnPointerWheelChanged;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerLeave;

        //Shapes = Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>();
        ActiveMap = Maps.GetWorldMap<SkiaSharpDrawingContext>();
        SyncContext = new object();

        DetachedFromVisualTree += GeoMap_DetachedFromVisualTree;
    }

    #region dependency props

    /// <summary>
    /// The active map property.
    /// </summary>
    public static readonly AvaloniaProperty<CoreMap<SkiaSharpDrawingContext>?> ActiveMapProperty =
       AvaloniaProperty.Register<CartesianChart, CoreMap<SkiaSharpDrawingContext>?>(nameof(ActiveMap), null, inherits: true);

    /// <summary>
    /// The active map property.
    /// </summary>
    public static readonly AvaloniaProperty<object?> SyncContextProperty =
       AvaloniaProperty.Register<CartesianChart, object?>(nameof(SyncContext), null, inherits: true);

    /// <summary>
    /// The active map property.
    /// </summary>
    public static readonly AvaloniaProperty<object?> ViewCommandProperty =
       AvaloniaProperty.Register<CartesianChart, object?>(nameof(ViewCommand), null, inherits: true);

    /// <summary>
    /// The projection property.
    /// </summary>
    public static readonly AvaloniaProperty<MapProjection> MapProjectionProperty =
       AvaloniaProperty.Register<CartesianChart, MapProjection>(nameof(MapProjection), MapProjection.Default, inherits: true);

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<IGeoSeries>> SeriesProperty =
      AvaloniaProperty.Register<CartesianChart, IEnumerable<IGeoSeries>>(nameof(Series),
          Enumerable.Empty<IGeoSeries>(), inherits: true);

    /// <summary>
    /// The stroke property.
    /// </summary>
    public static readonly AvaloniaProperty<IPaint<SkiaSharpDrawingContext>> StrokeProperty =
      AvaloniaProperty.Register<CartesianChart, IPaint<SkiaSharpDrawingContext>>(nameof(Stroke),
          new SolidColorPaint(new SKColor(255, 255, 255, 255), 1) { IsStroke = true }, inherits: true);

    /// <summary>
    /// The fill color property.
    /// </summary>
    public static readonly AvaloniaProperty<IPaint<SkiaSharpDrawingContext>> FillProperty =
      AvaloniaProperty.Register<CartesianChart, IPaint<SkiaSharpDrawingContext>>(nameof(Fill),
           new SolidColorPaint(new SKColor(240, 240, 240, 255)) { IsFill = true }, inherits: true);

    #endregion

    #region props

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
    bool IGeoMapView<SkiaSharpDrawingContext>.DesignerMode => Design.IsDesignMode;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty)!;
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ViewCommand" />
    public object? ViewCommand
    {
        get => GetValue(ViewCommandProperty);
        set => SetValue(ViewCommandProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
    public MotionCanvas<SkiaSharpDrawingContext> Canvas
    {
        get
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            return canvas!.CanvasCore;
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
    public CoreMap<SkiaSharpDrawingContext> ActiveMap
    {
        get => (CoreMap<SkiaSharpDrawingContext>)GetValue(ActiveMapProperty)!;
        set => SetValue(ActiveMapProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Width"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Width => (float)Bounds.Width;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Height"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Height => (float)Bounds.Height;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
    public MapProjection MapProjection
    {
        get => (MapProjection)GetValue(MapProjectionProperty)!;
        set => SetValue(MapProjectionProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
    public IPaint<SkiaSharpDrawingContext>? Stroke
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(StrokeProperty);
        set
        {
            if (value is not null) value.IsStroke = true;
            _ = SetValue(StrokeProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
    public IPaint<SkiaSharpDrawingContext>? Fill
    {
        get => (IPaint<SkiaSharpDrawingContext>?)GetValue(FillProperty);
        set
        {
            if (value is not null) value.IsFill = true;
            _ = SetValue(FillProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Series"/>
    public IEnumerable<IGeoSeries> Series
    {
        get => (IEnumerable<IGeoSeries>)GetValue(SeriesProperty)!;
        set => SetValue(SeriesProperty, value);
    }

    #endregion

    void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }

    /// <inheritdoc cref="OnPropertyChanged(AvaloniaPropertyChangedEventArgs)"/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IsPointerOver)) return;

        if (change.Property.Name == nameof(Series))
        {
            _seriesObserver?.Dispose((IEnumerable<IGeoSeries>?)change.OldValue);
            _seriesObserver?.Initialize((IEnumerable<IGeoSeries>?)change.NewValue);
        }

        if (change.Property.Name == nameof(ViewCommand))
        {
            _core.ViewTo(ViewCommand);
            return;
        }

        _core?.Update();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_core is null) return;

        var p = e.GetPosition(this);

        _core.ViewTo(
            new ZoomOnPointerView(
                new LvcPoint((float)p.X, (float)p.Y),
                e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut));
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var p = e.GetPosition(this);
        foreach (var w in desktop.Windows) w.PointerReleased += OnWindowPointerReleased;
        _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var p = e.GetPosition(this);
        _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        foreach (var w in desktop.Windows) w.PointerReleased -= OnWindowPointerReleased;
        var p = e.GetPosition(this);
        _core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnPointerLeave(object? sender, PointerEventArgs e)
    {
        _core?.InvokePointerLeft();
    }

    private void GeoMap_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        if (_core is null) return;
        _core.Unload();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
