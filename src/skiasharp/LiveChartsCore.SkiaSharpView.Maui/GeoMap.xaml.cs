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
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using SkiaSharp;
using Paint = LiveChartsCore.Painting.Paint;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="IGeoMapView"/>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GeoMap : ContentView, IGeoMapView
{
    private readonly CollectionDeepObserver<IGeoSeries> _seriesObserver;
    private readonly GeoMapChart _core;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap"/> class.
    /// </summary>
    public GeoMap()
    {
        InitializeComponent();
        LiveCharts.Configure(config => config.UseDefaults());
        _core = new GeoMapChart(this);

        SizeChanged += GeoMap_SizeChanged;

        _seriesObserver = new CollectionDeepObserver<IGeoSeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
            (object? sender, PropertyChangedEventArgs e) => _core?.Update(),
            true);

        SetValue(SeriesProperty, Enumerable.Empty<IGeoSeries>());
        SetValue(ActiveMapProperty, Maps.GetWorldMap());
        SetValue(SyncContextProperty, new object());
    }

    #region dependency props

    /// <summary>
    /// The active map property
    /// </summary>
    public static readonly BindableProperty ActiveMapProperty =
        BindableProperty.Create(
            nameof(ActiveMap), typeof(DrawnMap), typeof(GeoMap), null, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The sync context property
    /// </summary>
    public static readonly BindableProperty SyncContextProperty =
        BindableProperty.Create(
            nameof(SyncContext), typeof(object), typeof(GeoMap), null, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The view command property
    /// </summary>
    public static readonly BindableProperty ViewCommandProperty =
        BindableProperty.Create(
            nameof(ViewCommand), typeof(object), typeof(GeoMap), null, BindingMode.Default, null,
            (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (GeoMap)o;
                chart._core.ViewTo(newValue);
            });

    /// <summary>
    /// The map projection property
    /// </summary>
    public static readonly BindableProperty MapProjectionProperty =
        BindableProperty.Create(
            nameof(MapProjection), typeof(MapProjection), typeof(GeoMap),
            MapProjection.Default, BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The stroke property
    /// </summary>
    public static readonly BindableProperty StrokeProperty =
        BindableProperty.Create(
            nameof(Stroke), typeof(Paint), typeof(GeoMap),
              new SolidColorPaint(new SKColor(255, 255, 255, 255)) { IsStroke = true },
              BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The fill property
    /// </summary>
    public static readonly BindableProperty FillProperty =
       BindableProperty.Create(
           nameof(Fill), typeof(Paint), typeof(GeoMap),
            new SolidColorPaint(new SKColor(240, 240, 240, 255)) { IsStroke = false },
            BindingMode.Default, null, OnBindablePropertyChanged);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly BindableProperty SeriesProperty =
       BindableProperty.Create(
           nameof(Series), typeof(IEnumerable<IGeoSeries>), typeof(GeoMap),
           Enumerable.Empty<IGeoSeries>(),
           BindingMode.Default, null, (BindableObject o, object oldValue, object newValue) =>
           {
               var chart = (GeoMap)o;
               var seriesObserver = chart._seriesObserver;
               seriesObserver?.Dispose((IEnumerable<IGeoSeries>)oldValue);
               seriesObserver?.Initialize((IEnumerable<IGeoSeries>)newValue);
               chart._core.Update();
           });

    #endregion

    #region props

    /// <inheritdoc cref="IGeoMapView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView.DesignerMode" />
    bool IGeoMapView.DesignerMode => false;

    /// <inheritdoc cref="IGeoMapView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.ViewCommand" />
    public object? ViewCommand
    {
        get => GetValue(ViewCommandProperty);
        set => SetValue(ViewCommandProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Canvas"/>
    public CoreMotionCanvas Canvas => canvas.CanvasCore;

    /// <inheritdoc cref="IGeoMapView.ActiveMap"/>
    public DrawnMap ActiveMap
    {
        get => (DrawnMap)GetValue(ActiveMapProperty);
        set => SetValue(ActiveMapProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Width"/>
    float IGeoMapView.Width => (float)canvas.Width;

    /// <inheritdoc cref="IGeoMapView.Height"/>
    float IGeoMapView.Height => (float)canvas.Height;

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    public MapProjection MapProjection
    {
        get => (MapProjection)GetValue(MapProjectionProperty);
        set => SetValue(MapProjectionProperty, value);
    }

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    public Paint? Stroke
    {
        get => (Paint)GetValue(StrokeProperty);
        set
        {
            if (value is not null) value.IsStroke = true;
            SetValue(StrokeProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView.Fill"/>
    public Paint? Fill
    {
        get => (Paint)GetValue(FillProperty);
        set
        {
            if (value is not null) value.IsStroke = false;
            SetValue(FillProperty, value);
        }
    }

    /// <inheritdoc cref="IGeoMapView.Series"/>
    public IEnumerable<IGeoSeries> Series
    {
        get => (IEnumerable<IGeoSeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    #endregion

    /// <inheritdoc cref="NavigableElement.OnParentSet"/>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent == null)
        {
            _core.Unload();
        }
    }

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        _ = MainThread.InvokeOnMainThreadAsync(action);

    private void GeoMap_SizeChanged(object? sender, EventArgs e) =>
        _core?.Update();

    private void PanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (_core is null) return;
        if (e.StatusType != GestureStatus.Running) return;

        var delta = new LvcPoint((float)e.TotalX, (float)e.TotalY);
        _core.Pan(delta);
    }

    private void PinchGestureRecognizer_PinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status != GestureStatus.Running || Math.Abs(e.Scale - 1) < 0.05 || _core is null) return;

        var p = e.ScaleOrigin;
        var w = ((IGeoMapView)this).Width;
        var h = ((IGeoMapView)this).Width;

        _core.ViewTo(
            new ZoomOnPointerView(
                new LvcPoint((float)(p.X * w), (float)(p.Y * h)),
                e.Scale > 1 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut));
    }

    private static void OnBindablePropertyChanged(BindableObject o, object oldValue, object newValue)
    {
        var chart = (GeoMap)o;
        if (chart._core is null) return;
        chart._core.Update();
    }
}
