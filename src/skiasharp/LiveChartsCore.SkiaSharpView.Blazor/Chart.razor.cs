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
using LiveChartsCore.Painting;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IChartView" />
public partial class Chart : IBlazorChart, IDisposable, IChartView
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private JsFlexibleContainer _jsFlexibleContainer = null!;

    /// <summary>
    /// The core.
    /// </summary>
    protected LiveChartsCore.Chart? core;

    /// <summary>
    /// The motion canvas.
    /// </summary>
    protected MotionCanvas? motionCanvas;

    private double _canvasWidth;
    private double _canvasHeight;

    private CollectionDeepObserver<ChartElement>? _visualsObserver;
    private LegendPosition _legendPosition = LiveCharts.DefaultSettings.LegendPosition;
    private Margin? _drawMargin = null;
    private TooltipPosition _tooltipPosition = LiveCharts.DefaultSettings.TooltipPosition;
    private IEnumerable<ChartElement> _visuals = [];

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        LiveCharts.Configure(config => config.UseDefaults());

        _visualsObserver = new CollectionDeepObserver<ChartElement>(
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

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

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
    public LiveChartsCore.Chart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor { get; set; }

    LvcSize IChartView.ControlSize => motionCanvas is null
        ? new LvcSize()
        : new LvcSize { Width = (float)_canvasWidth, Height = (float)_canvasHeight };

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => motionCanvas?.CanvasCore ?? throw new Exception("canvas not found!");

    /// <inheritdoc cref="IChartView.Title"/>
    [Parameter]
    public VisualElement? Title { get; set; }

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

    /// <inheritdoc cref="IChartView.Legend" />
    [Parameter]
    public IChartLegend? Legend { get; set; } = LiveCharts.DefaultSettings.GetTheme().DefaultLegend();

    /// <inheritdoc cref="IChartView.LegendTextPaint" />
    [Parameter]
    public Paint? LegendTextPaint { get; set; }
        = (Paint?)LiveCharts.DefaultSettings.LegendTextPaint;

    /// <inheritdoc cref="IChartView.LegendBackgroundPaint" />
    [Parameter]
    public Paint? LegendBackgroundPaint { get; set; }
        = (Paint?)LiveCharts.DefaultSettings.LegendBackgroundPaint;

    /// <inheritdoc cref="IChartView.LegendTextSize" />
    [Parameter]
    public double LegendTextSize { get; set; }
        = LiveCharts.DefaultSettings.LegendTextSize;

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    [Parameter]
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.Tooltip" />
    [Parameter]
    public IChartTooltip? Tooltip { get; set; }

    /// <inheritdoc cref="IChartView.TooltipTextPaint" />
    [Parameter]
    public Paint? TooltipTextPaint { get; set; }
        = (Paint?)LiveCharts.DefaultSettings.TooltipTextPaint;

    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint" />
    [Parameter]
    public Paint? TooltipBackgroundPaint { get; set; }
        = (Paint?)LiveCharts.DefaultSettings.TooltipBackgroundPaint;

    /// <inheritdoc cref="IChartView.TooltipTextSize" />
    [Parameter]
    public double TooltipTextSize { get; set; }
        = LiveCharts.DefaultSettings.TooltipTextSize;

    /// <inheritdoc cref="IChartView.AutoUpdateEnabled" />
    [Parameter]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    [Parameter]
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <inheritdoc cref="IChartView.VisualElements" />
    [Parameter]
    public IEnumerable<ChartElement> VisualElements
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

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public virtual IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent) =>
        throw new NotImplementedException();

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public virtual IEnumerable<IChartElement> GetVisualsAt(LvcPointD point) =>
        throw new NotImplementedException();

    void IChartView.InvokeOnUIThread(Action action) => _ = InvokeAsync(action);

    /// <summary>
    /// Called when the core is initalized.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void InitializeCore() => throw new NotImplementedException();

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    protected void OnPropertyChanged()
    {
        if (core is null || ((IChartView)this).DesignerMode) return;
        core.Update();
    }

    private void OnCoreUpdateFinished(IChartView chart) => UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart) => UpdateStarted?.Invoke(this);

    private void OnCoreMeasuring(IChartView chart) => Measuring?.Invoke(this);

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
    protected virtual void OnDisposing() => _visualsObserver = null!;

    /// <summary>
    /// Called when the pointer leaves the control.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerOut(PointerEventArgs e) => core?.InvokePointerLeft();

    private async void OnResized(JsFlexibleContainer container)
    {
        if (_dom is null) return;

        var canvasBounds = await _dom.GetBoundingClientRect(CanvasContainerElement);
        _canvasWidth = canvasBounds.Width;
        _canvasHeight = canvasBounds.Height;

        core?.Update();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged();

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged();

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView.OnVisualElementPointerDown(
       IEnumerable<IInteractable> visualElements, LvcPoint pointer) => VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs(CoreChart, visualElements, pointer));

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints) =>
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

    void IChartView.Invalidate() => CoreCanvas.Invalidate();

    async void IDisposable.Dispose()
    {
        OnDisposing();

        if (_dom is null) return;
        await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
