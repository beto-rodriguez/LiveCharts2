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
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <summary>
/// The geo map control.
/// </summary>
public class GeoMap : Panel, IGeoMapView
{
    private readonly MotionCanvas _motionCanvas = new();
    private readonly GeoMapChart _core;
    private CollectionDeepObserver _seriesObserver;
    private IEnumerable<IGeoSeries> _series = [];
    private DrawnMap _activeMap;
    private MapProjection _mapProjection = MapProjection.Default;
    private Paint? _stroke = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { PaintStyle = PaintStyle.Stroke };
    private Paint? _fill = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { PaintStyle = PaintStyle.Fill };
    private object? _viewCommand = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap"/> class.
    /// </summary>
    public GeoMap()
    {
        LiveCharts.Configure(config => config.UseDefaults());
        _activeMap = Maps.GetWorldMap();

        _core = new GeoMapChart(this);
        _seriesObserver = new CollectionDeepObserver(() => _core?.Update());

        var c = _motionCanvas;

        c.MouseDown += OnMouseDown;
        c.MouseMove += OnMouseMove;
        c.MouseUp += OnMouseUp;
        c.MouseLeave += OnMouseLeave;
        c.MouseWheel += OnMouseWheel;

        SizeChanged += GeoMap_Resize;

        BackgroundColor = global::Eto.Drawing.Colors.White;

        Content = _motionCanvas;
    }

    /// <inheritdoc cref="IGeoMapView.Canvas"/>
    public CoreMotionCanvas Canvas => _motionCanvas.CanvasCore;

    /// <inheritdoc cref="IGeoMapView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView.DesignerMode" />
    bool IGeoMapView.DesignerMode => false;

    /// <inheritdoc cref="IGeoMapView.SyncContext" />
    public object SyncContext { get; set; } = new();

    /// <inheritdoc cref="IGeoMapView.ViewCommand" />
    public object? ViewCommand
    {
        get => _viewCommand;
        set
        {
            _viewCommand = value;
            if (value is not null) _core.ViewTo(value);
        }
    }
    /// <inheritdoc cref="IGeoMapView.ActiveMap"/>
    public DrawnMap ActiveMap { get => _activeMap; set { _activeMap = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IGeoMapView.Width"/>
    float IGeoMapView.Width => ClientSize.Width;

    /// <inheritdoc cref="IGeoMapView.Height"/>
    float IGeoMapView.Height => ClientSize.Height;

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    public MapProjection MapProjection { get => _mapProjection; set { _mapProjection = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    public Paint? Stroke
    {
        get => _stroke;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Stroke;
            _stroke = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView.Fill"/>
    public Paint? Fill
    {
        get => _fill;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Fill;
            _fill = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView.Series"/>
    public IEnumerable<IGeoSeries> Series
    {
        get => _series;
        set
        {
            _seriesObserver.Dispose();
            _series = value;
            OnPropertyChanged();
        }
    }

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        Application.Instance.InvokeAsync(action).Wait();

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged() => _core?.Update();

    /// <inheritdoc cref="Control.OnUnLoad(EventArgs)"/>
    protected override void OnUnLoad(EventArgs e)
    {
        base.OnUnLoad(e);

        _core?.Unload();

        Series = [];
        _seriesObserver = null!;

        Canvas.Dispose();
    }

    private void GeoMap_Resize(object? sender, EventArgs e) => _core?.Update();

    private void OnMouseDown(object? sender, MouseEventArgs e) => _core?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y));
    private void OnMouseMove(object? sender, MouseEventArgs e) => _core?.InvokePointerMove(new LvcPoint(e.Location.X, e.Location.Y));

    private void OnMouseUp(object? sender, MouseEventArgs e) => _core?.InvokePointerUp(new LvcPoint(e.Location.X, e.Location.Y));

    private void OnMouseLeave(object? sender, EventArgs e) => _core?.InvokePointerLeft();

    private void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (_core is null) throw new Exception("core not found");
        var p = e.Location;
        _core.ViewTo(
            new ZoomOnPointerView(new LvcPoint(p.X, p.Y), e.Delta.Height > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut));
        e.Handled = true;
    }
}
