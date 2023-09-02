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
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IChartView{TDrawingContext}" />
public partial class Chart : IBlazorChart, IDisposable, IChartView<SkiaSharpDrawingContext>
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private JsFlexibleContainer _jsFlexibleContainer = null!;

    /// <summary>
    /// The core.
    /// </summary>
    protected Chart<SkiaSharpDrawingContext>? core;

    /// <summary>
    /// The motion canvas.
    /// </summary>
    protected MotionCanvas? motionCanvas;

    private double _canvasWidth;
    private double _canvasHeight;

    private CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>? _visualsObserver;
    private LegendPosition _legendPosition = LiveCharts.DefaultSettings.LegendPosition;
    private Margin? _drawMargin = null;
    private TooltipPosition _tooltipPosition = LiveCharts.DefaultSettings.TooltipPosition;
    private IEnumerable<ChartElement<SkiaSharpDrawingContext>> _visuals = new List<ChartElement<SkiaSharpDrawingContext>>();

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        LiveCharts.Configure(config => config.UseDefaults());

        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
    }

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        InitializeCore();
        if (core is null || motionCanvas is null) throw new Exception("Core or MotionCanvas not found!");

        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;

        _dom ??= new DomJsInterop(JS);
        var canvasBounds = await _dom.GetBoundingClientRect(CanvasContainerElement);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        core?.Load();
        _jsFlexibleContainer.Resized += OnResized;
    }

    #region events

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElementsPointerDown"/>
    public event VisualElementsHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    #endregion

    #region properties

    /// <summary>
    /// Gets the actual class.
    /// </summary>
    public string ContainerActualClass => $"lvc-chart-container {ContainerClass}";

    /// <inheritdoc cref="IBlazorChart.CanvasContainerElement"/>
    public ElementReference CanvasContainerElement { get; private set; }

    /// <inheritdoc cref="IBlazorChart.ContainerClass"/>
    public string ContainerClass { get; set; } = string.Empty;

    /// <inheritdoc cref="IBlazorChart.LegendClass"/>
    public string LegendClass { get; set; } = "closed";

    /// <inheritdoc cref="IBlazorChart.TooltipClass"/>
    public string TooltipClass { get; set; } = "closed";

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => false;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor { get; set; }

    LvcSize IChartView.ControlSize => motionCanvas is null
        ? new LvcSize()
        : new LvcSize { Width = (float)_canvasWidth, Height = (float)_canvasHeight };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas?.CanvasCore ?? throw new Exception("canvas not found!");

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title"/>
    [Parameter]
    public VisualElement<SkiaSharpDrawingContext>? Title { get; set; }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    [Parameter]
    public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.SyncContext" />
    [Parameter]
    public object SyncContext { get => CoreCanvas.Sync; set { CoreCanvas.Sync = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [Parameter]
    public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.DefaultSettings.AnimationsSpeed;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [Parameter]
    public Func<float, float>? EasingFunction { get; set; } = LiveCharts.DefaultSettings.EasingFunction;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    [Parameter]
    public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    [Parameter]
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; set; } = new SKDefaultLegend();

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextPaint" />
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? LegendTextPaint { get; set; }
        = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.LegendTextPaint;

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendBackgroundPaint" />
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? LegendBackgroundPaint { get; set; }
        = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.LegendBackgroundPaint;

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextSize" />
    [Parameter]
    public double? LegendTextSize { get; set; } = LiveCharts.DefaultSettings.LegendTextSize;

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    [Parameter]
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    [Parameter]
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get; set; } = new SKDefaultTooltip();

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextPaint" />
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? TooltipTextPaint { get; set; }
        = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.TooltipTextPaint;

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipBackgroundPaint" />
    [Parameter]
    public IPaint<SkiaSharpDrawingContext>? TooltipBackgroundPaint { get; set; }
        = (IPaint<SkiaSharpDrawingContext>?)LiveCharts.DefaultSettings.TooltipBackgroundPaint;

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextSize" />
    [Parameter]
    public double? TooltipTextSize { get; set; }
        = LiveCharts.DefaultSettings.TooltipTextSize;

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    [Parameter]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    [Parameter]
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    [Parameter]
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => _visuals;
        set
        {
            _visualsObserver?.Dispose(_visuals);
            _visualsObserver?.Initialize(value);
            _visuals = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the pointer down callback.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerDownCallback { get; set; }

    /// <summary>
    /// Gets or sets the pointer move callback.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerMoveCallback { get; set; }

    /// <summary>
    /// Gets or sets the pointer up callback.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerUpCallback { get; set; }

    #endregion

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public virtual IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public virtual IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        throw new NotImplementedException();
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        _ = InvokeAsync(action);
    }

    /// <summary>
    /// Called when the core is initalized.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void InitializeCore()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged()
    {
        if (core is null || ((IChartView)this).DesignerMode) return;
        core.Update();
    }

    private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateFinished?.Invoke(this);
    }

    private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
    {
        Measuring?.Invoke(this);
    }

    /// <summary>
    /// Called when the pointer goes down.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerDown(PointerEventArgs e)
    {
        core?.InvokePointerDown(new LvcPoint((float)e.OffsetX, (float)e.OffsetY), e.Button == 2);
        _ = OnPointerDownCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called when the pointer moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerMove(PointerEventArgs e)
    {
        core?.InvokePointerMove(new LvcPoint((float)e.OffsetX, (float)e.OffsetY));
        _ = OnPointerMoveCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called when the pointer goes up.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerUp(PointerEventArgs e)
    {
        core?.InvokePointerUp(new LvcPoint((float)e.OffsetX, (float)e.OffsetY), e.Button == 2);
        _ = OnPointerUpCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called then mouse wheel moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnWheel(WheelEventArgs e) { }

    /// <summary>
    /// Called when the control is disposing.
    /// </summary>
    protected virtual void OnDisposing()
    {
        _visualsObserver = null!;
    }

    /// <summary>
    /// Called when the pointer leaves the control.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerOut(PointerEventArgs e)
    {
        core?.InvokePointerLeft();
    }

    private async void OnResized(JsFlexibleContainer container)
    {
        if (_dom is null) return;

        var canvasBounds = await _dom.GetBoundingClientRect(CanvasContainerElement);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        core?.Update();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView<SkiaSharpDrawingContext>.OnVisualElementPointerDown(
       IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs<SkiaSharpDrawingContext>(CoreChart, visualElements, pointer));
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }

    async void IDisposable.Dispose()
    {
        OnDisposing();

        if (_dom is null) return;
        await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
