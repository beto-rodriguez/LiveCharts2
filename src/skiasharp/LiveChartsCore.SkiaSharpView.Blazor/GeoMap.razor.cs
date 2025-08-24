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

using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.Components;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IGeoMapView"/>
public partial class GeoMap : IGeoMapView, IDisposable
{
#pragma warning disable IDE0032 // Use auto property, blazor ref
    private MotionCanvas _motionCanvas = null!;
#pragma warning restore IDE0032 // Use auto property
    private CollectionDeepObserver? _seriesObserver;
    private GeoMapChart? _core;
    private DrawnMap? _activeMap;

    /// <summary>
    /// Called when the control initializes.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _activeMap = Maps.GetWorldMap();
    }

    /// <summary>
    /// Called when the control renders.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;

        _core = new GeoMapChart(this);

        _motionCanvas.SizeChanged +=
            () =>
                _core.Update();

        _seriesObserver = new CollectionDeepObserver(() => _core?.Update());
        _core.Update();

        if (_motionCanvas is null) throw new Exception("MotionCanvas not found");
    }

    /// <inheritdoc cref="IGeoMapView.Canvas"/>
    public CoreMotionCanvas Canvas => _motionCanvas?.CanvasCore ?? throw new Exception("MotionCanvas not found.");

    /// <inheritdoc cref="IGeoMapView.AutoUpdateEnabled" />
    [Parameter]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView.DesignerMode" />
    bool IGeoMapView.DesignerMode => false;

    /// <inheritdoc cref="IGeoMapView.SyncContext" />
    [Parameter]
    public object SyncContext { get; set; } = new();

    /// <inheritdoc cref="IGeoMapView.ViewCommand" />
    [Parameter]
    public object? ViewCommand
    {
        get;
        set
        {
            field = value;
            if (value is not null) _core?.ViewTo(value);
        }
    } = null;

    /// <inheritdoc cref="IGeoMapView.ActiveMap"/>
    [Parameter]
    public DrawnMap ActiveMap
    {
        get => _activeMap ?? throw new Exception($"{nameof(ActiveMap)} is required.");
        set
        {
            _activeMap = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView.Width"/>
    float IGeoMapView.Width => _motionCanvas.Width;

    /// <inheritdoc cref="IGeoMapView.Height"/>
    float IGeoMapView.Height => _motionCanvas.Height;

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    [Parameter]
    public MapProjection MapProjection { get; set { field = value; OnPropertyChanged(); } } = MapProjection.Default;

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    [Parameter]
    public Paint? Stroke
    {
        get;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Stroke;
            field = value;
            OnPropertyChanged();
        }
    } = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { PaintStyle = PaintStyle.Stroke };

    /// <inheritdoc cref="IGeoMapView.Fill"/>
    [Parameter]
    public Paint? Fill
    {
        get;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Fill;
            field = value;
            OnPropertyChanged();
        }
    } = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { PaintStyle = PaintStyle.Fill };

    /// <inheritdoc cref="IGeoMapView.Series"/>
    [Parameter]
    public IEnumerable<IGeoSeries> Series
    {
        get;
        set
        {
            _seriesObserver?.Dispose();
            _seriesObserver?.Initialize(value);
            field = value;
            OnPropertyChanged();
        }
    } = [];

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        _ = InvokeAsync(action);

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged() =>
        _core?.Update();

    void IDisposable.Dispose()
    {
        Series = [];
        _seriesObserver = null!;

        Canvas.Dispose();

        _core?.Unload();
    }
}
