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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="IGeoMapView"/>
public partial class GeoMap : UserControl, IGeoMapView
{
    private readonly CollectionDeepObserver _seriesObserver;
    private readonly GeoMapChart _core;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap"/> class.
    /// </summary>
    public GeoMap()
    {
        InitializeComponent();
        LiveCharts.Configure(config => config.UseDefaults());
        _core = new GeoMapChart(this);
        _seriesObserver = new CollectionDeepObserver(() => _core?.Update());

        Background = new SolidColorBrush(new Color(0, 0, 0, 0));

        PointerWheelChanged += OnPointerWheelChanged;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerLeave;

        //Shapes = Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>();
        ActiveMap = Maps.GetWorldMap();
        SyncContext = new object();

        DetachedFromVisualTree += GeoMap_DetachedFromVisualTree;
    }

    #region dependency props

    /// <summary>
    /// The active map property.
    /// </summary>
    public static readonly AvaloniaProperty<DrawnMap?> ActiveMapProperty =
       AvaloniaProperty.Register<CartesianChart, DrawnMap?>(nameof(ActiveMap), null, inherits: true);

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
          [], inherits: true);

    /// <summary>
    /// The stroke property.
    /// </summary>
    public static readonly AvaloniaProperty<Paint> StrokeProperty =
      AvaloniaProperty.Register<CartesianChart, Paint>(nameof(Stroke),
          new SolidColorPaint(new SKColor(255, 255, 255, 255), 1) { PaintStyle = PaintStyle.Stroke }, inherits: true);

    /// <summary>
    /// The fill color property.
    /// </summary>
    public static readonly AvaloniaProperty<Paint> FillProperty =
      AvaloniaProperty.Register<CartesianChart, Paint>(nameof(Fill),
           new SolidColorPaint(new SKColor(240, 240, 240, 255)) { PaintStyle = PaintStyle.Fill }, inherits: true);

    #endregion

    #region props

    /// <inheritdoc cref="IGeoMapView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView.DesignerMode" />
    bool IGeoMapView.DesignerMode => Design.IsDesignMode;

    /// <inheritdoc cref="IGeoMapView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty)!;
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.ViewCommand" />
    public object? ViewCommand
    {
        get => GetValue(ViewCommandProperty);
        set => SetValue(ViewCommandProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Canvas"/>
    public CoreMotionCanvas Canvas
    {
        get
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            return canvas!.CanvasCore;
        }
    }

    /// <inheritdoc cref="IGeoMapView.ActiveMap"/>
    public DrawnMap ActiveMap
    {
        get => (DrawnMap)GetValue(ActiveMapProperty)!;
        set => SetValue(ActiveMapProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Width"/>
    float IGeoMapView.Width => (float)Bounds.Width;

    /// <inheritdoc cref="IGeoMapView.Height"/>
    float IGeoMapView.Height => (float)Bounds.Height;

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    public MapProjection MapProjection
    {
        get => (MapProjection)GetValue(MapProjectionProperty)!;
        set => SetValue(MapProjectionProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    public Paint? Stroke
    {
        get => (Paint?)GetValue(StrokeProperty);
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Stroke;
            _ = SetValue(StrokeProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView.Fill"/>
    public Paint? Fill
    {
        get => (Paint?)GetValue(FillProperty);
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Fill;
            _ = SetValue(FillProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView.Series"/>
    public IEnumerable<IGeoSeries> Series
    {
        get => (IEnumerable<IGeoSeries>)GetValue(SeriesProperty)!;
        set => SetValue(SeriesProperty, value);
    }

    #endregion

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        Dispatcher.UIThread.Post(action);

    /// <inheritdoc cref="OnPropertyChanged(AvaloniaPropertyChangedEventArgs)"/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IsPointerOver)) return;

        if (change.Property.Name == nameof(Series))
        {
            _seriesObserver?.Dispose();
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

    private void OnPointerLeave(object? sender, PointerEventArgs e) =>
        _core?.InvokePointerLeft();

    private void GeoMap_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_core is null) return;
        _core.Unload();
    }

    private void InitializeComponent() =>
        AvaloniaXamlLoader.Load(this);
}
