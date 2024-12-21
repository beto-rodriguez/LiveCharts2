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

using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IGeoMapView"/>
public partial class GeoMap : IGeoMapView, IDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private JsFlexibleContainer _jsFlexibleContainer = null!;
    private ElementReference _canvasContainer;
    private MotionCanvas? _motionCanvas;
    private double _canvasWidth;
    private double _canvasHeight;
    private CollectionDeepObserver<IGeoSeries>? _seriesObserver;
    private GeoMapChart? _core;
    private IEnumerable<IGeoSeries> _series = [];
    private DrawnMap? _activeMap;
    private MapProjection _mapProjection = MapProjection.Default;
    private Paint? _stroke = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { PaintStyle = PaintStyle.Stroke };
    private Paint? _fill = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { PaintStyle = PaintStyle.Fill };
    private object? _viewCommand = null;

    /// <summary>
    /// Called when the control initializes.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        LiveCharts.Configure(config => config.UseDefaults());
        _activeMap = Maps.GetWorldMap();
    }

    /// <summary>
    /// Called when the control renders.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        _core = new GeoMapChart(this);

        _seriesObserver = new CollectionDeepObserver<IGeoSeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
            (object? sender, PropertyChangedEventArgs e) => _core?.Update(),
            true);

        // ToDo: Pointer events.
        //var c = Controls[0].Controls[0];

        //c.MouseDown += OnMouseDown;
        //c.MouseMove += OnMouseMove;
        //c.MouseUp += OnMouseUp;
        //c.MouseLeave += OnMouseLeave;
        //c.MouseWheel += OnMouseWheel;

        //Resize += GeoMap_Resize;

        _dom ??= new DomJsInterop(JS);
        var canvasBounds = await _dom.GetBoundingClientRect(_canvasContainer);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        _core.Update();

        if (_motionCanvas is null) throw new Exception("MotionCanvas not found");
        _jsFlexibleContainer.Resized += OnResized;
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
        get => _viewCommand;
        set
        {
            _viewCommand = value;
            if (value is not null) _core?.ViewTo(value);
        }
    }
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
    float IGeoMapView.Width => (float)_canvasWidth;

    /// <inheritdoc cref="IGeoMapView.Height"/>
    float IGeoMapView.Height => (float)_canvasHeight;

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    [Parameter]
    public MapProjection MapProjection { get => _mapProjection; set { _mapProjection = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    [Parameter]
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
    [Parameter]
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
    [Parameter]
    public IEnumerable<IGeoSeries> Series
    {
        get => _series;
        set
        {
            _seriesObserver?.Dispose(_series);
            _seriesObserver?.Initialize(value);
            _series = value;
            OnPropertyChanged();
        }
    }

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        _ = InvokeAsync(action); //.Wait();

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged() =>
        _core?.Update();

    private async void OnResized(JsFlexibleContainer motionCanvas)
    {
        if (_dom is null) return;

        var canvasBounds = await _dom.GetBoundingClientRect(_canvasContainer);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        _core?.Update();
    }

    async void IDisposable.Dispose()
    {
        Series = [];
        _seriesObserver = null!;

        Canvas.Dispose();

        _core?.Unload();
        if (_dom is not null) await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
