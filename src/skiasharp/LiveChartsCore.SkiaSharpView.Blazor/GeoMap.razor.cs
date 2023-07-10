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
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IGeoMapView{TDrawingContext}"/>
public partial class GeoMap : IGeoMapView<SkiaSharpDrawingContext>, IDisposable
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
    private GeoMap<SkiaSharpDrawingContext>? _core;
    private IEnumerable<IGeoSeries> _series = Enumerable.Empty<IGeoSeries>();
    private CoreMap<SkiaSharpDrawingContext>? _activeMap;
    private MapProjection _mapProjection = MapProjection.Default;
    private IPaint<SkiaSharpDrawingContext>? _stroke = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { IsStroke = true };
    private IPaint<SkiaSharpDrawingContext>? _fill = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { IsFill = true };
    private object? _viewCommand = null;

    /// <summary>
    /// Called when the control initializes.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        LiveCharts.Configure(config => config.UseDefaults());
        _activeMap = Maps.GetWorldMap<SkiaSharpDrawingContext>();
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

        _core = new GeoMap<SkiaSharpDrawingContext>(this);

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

        if (_dom is null) _dom = new DomJsInterop(JS);
        var canvasBounds = await _dom.GetBoundingClientRect(_canvasContainer);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        _core.Update();

        if (_motionCanvas is null) throw new Exception("MotionCanvas not found");
        _jsFlexibleContainer.Resized += OnResized;
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
    public MotionCanvas<SkiaSharpDrawingContext> Canvas => _motionCanvas?.CanvasCore ?? throw new Exception("MotionCanvas not found.");

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
    [Parameter]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
    bool IGeoMapView<SkiaSharpDrawingContext>.DesignerMode => false;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
    [Parameter]
    public object SyncContext { get; set; } = new();

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ViewCommand" />
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
    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
    [Parameter]
    public CoreMap<SkiaSharpDrawingContext> ActiveMap
    {
        get => _activeMap ?? throw new Exception($"{nameof(ActiveMap)} is required.");
        set
        {
            _activeMap = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Width"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Width => (float)_canvasWidth;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Height"/>
    float IGeoMapView<SkiaSharpDrawingContext>.Height => (float)_canvasHeight;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
    [Parameter]
    public MapProjection MapProjection { get => _mapProjection; set { _mapProjection = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? Stroke
    {
        get => _stroke;
        set
        {
            if (value is not null) value.IsStroke = true;
            _stroke = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? Fill
    {
        get => _fill;
        set
        {
            if (value is not null) value.IsFill = true;
            _fill = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Series"/>
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

    void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
    {
        _ = InvokeAsync(action); //.Wait();
    }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged()
    {
        _core?.Update();
    }

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
        Series = Array.Empty<IGeoSeries>();
        _seriesObserver = null!;

        Canvas.Dispose();

        _core?.Unload();
        if (_dom is not null) await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
