﻿// The MIT License(MIT)
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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
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

    private LegendPosition _legendPosition = LiveCharts.CurrentSettings.DefaultLegendPosition;
    private LegendOrientation _legendOrientation = LiveCharts.CurrentSettings.DefaultLegendOrientation;
    private Margin? _drawMargin = null;
    private TooltipPosition _tooltipPosition = LiveCharts.CurrentSettings.DefaultTooltipPosition;

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);
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

        if (_dom is null) _dom = new DomJsInterop(JS);
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
    public string LegendClass { get; set; } = string.Empty;

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

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin { get => _drawMargin; set { _drawMargin = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.SyncContext" />
    [Parameter]
    public object SyncContext { get => CoreCanvas.Sync; set { CoreCanvas.Sync = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [Parameter]
    public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    [Parameter]
    public Func<float, float>? EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    [Parameter]
    public LegendPosition LegendPosition { get => _legendPosition; set { _legendPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.LegendOrientation" />
    [Parameter]
    public LegendOrientation LegendOrientation { get => _legendOrientation; set { _legendOrientation = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    //[Parameter]
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; private set; } = null!;

    /// <inheritdoc cref="IChartView.LegendPosition" />
    [Parameter]
    public TooltipPosition TooltipPosition { get => _tooltipPosition; set { _tooltipPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get; private set; } = null!;

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    [Parameter]
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    [Parameter]
    public TimeSpan UpdaterThrottler
    {
        get => core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
        set
        {
            if (core is null) throw new Exception("core not set yet.");
            core.UpdaterThrottler = value;
        }
    }

    /// <summary>
    /// Gets or sets the legend template.
    /// </summary>
    [Parameter]
    public RenderFragment<ISeries[]>? LegendTemplate { get; set; }

    /// <summary>
    /// Gets or sets the tooltip legend.
    /// </summary>
    [Parameter]
    public RenderFragment<ChartPoint[]>? TooltipTemplate { get; set; }

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

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        if (Tooltip is null || core is null) return;

        Tooltip.Show(points, core);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        if (Tooltip is null || core is null) return;

        core.ClearTooltipData();
        Tooltip.Hide();
    }

    /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
    public void SetTooltipStyle(LvcColor background, LvcColor textColor)
    {
        //TooltipBackColor = Color.FromArgb(background.A, background.R, background.G, background.B);
        //TooltipTextColor = Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B);
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
        core?.InvokePointerDown(new LvcPoint((float)e.OffsetX, (float)e.OffsetY));
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
        core?.InvokePointerUp(new LvcPoint((float)e.OffsetX, (float)e.OffsetY));
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
    protected virtual void OnDisposing() { }

    /// <summary>
    /// Called when the pointer leaves the control.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerLeave(PointerEventArgs e)
    {
        HideTooltip();
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

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
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
