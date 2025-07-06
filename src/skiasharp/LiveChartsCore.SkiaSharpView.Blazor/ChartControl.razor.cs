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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IChartView" />
public abstract partial class ChartControl : IBlazorChart, IDisposable, IChartView
{
    private JsFlexibleContainer _jsFlexibleContainer = null!;
#pragma warning disable IDE0032 // Use auto property, blazor ref
    private MotionCanvas _motionCanvas = null!;
#pragma warning restore IDE0032 // Use auto property

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    protected ChartControl()
    {
        // on blazor by default we use the GPU
        // just because it looks MUCH better
        // the user can disable this feature by calling
        // LiveCharts.UseGPU = false;
        // or by setting the UseGPU property to false in the chart
        LiveCharts.SetUseGPUIfNotSetByUser(true);

        LiveCharts.Configure(config => config.UseDefaults());

        Observe = new ChartObserver(() => CoreChart?.Update())
            .Collection(nameof(Series))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title));

        // will be initialized in OnAfterRender, because we need the canvas element reference
        CoreChart = null!;
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => _motionCanvas;

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public Chart CoreChart { get; private set; }

    /// <summary>
    /// Gets the chart observer.
    /// </summary>
    protected ChartObserver Observe { get; private set; }

    /// <summary>
    /// Gets the actual class.
    /// </summary>
    public string ContainerActualClass => $"lvc-chart-container {ContainerClass}";

    /// <inheritdoc cref="IBlazorChart.CanvasContainerElement"/>
    public ElementReference CanvasContainerElement { get; private set; }

    /// <inheritdoc cref="IBlazorChart.ContainerClass"/>
    public string ContainerClass { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;

        CoreChart = CreateCoreChart();

        CoreChart.Measuring += OnCoreMeasuring;
        CoreChart.UpdateStarted += OnCoreUpdateStarted;
        CoreChart.UpdateFinished += OnCoreUpdateFinished;
        _jsFlexibleContainer.Resized +=
            container => CoreChart?.Update();

        CoreChart.Load();
    }

    bool IChartView.DesignerMode => false;
    bool IChartView.IsDarkMode => false; // Is this possible in Blazor?
    LvcColor IChartView.BackColor { get; set; } // is this even used in livecharts? isnt this obsolete?

    LvcSize IChartView.ControlSize => new() { Width = (float)_jsFlexibleContainer.Width, Height = (float)_jsFlexibleContainer.Height };

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get; set; }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => field; set { field = value; CoreChart.Update(); } }

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get; set { field = value; CoreChart?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => CanvasView.CanvasCore;

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

    void IChartView.InvokeOnUIThread(Action action) => _ = InvokeAsync(action);

    /// <summary>
    /// Creates the core chart instance for rendering and manipulation.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by derived classes to provide     a specific
    /// chart type. The returned <see cref="Chart"/> object represents the     foundational chart structure, which can
    /// be further customized or populated     with data.</remarks>
    /// <returns>A <see cref="Chart"/> object that serves as the base chart instance.</returns>
    protected abstract Chart CreateCoreChart();

    private void OnCoreUpdateFinished(IChartView chart) => UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart) => UpdateStarted?.Invoke(this);

    private void OnCoreMeasuring(IChartView chart) => Measuring?.Invoke(this);

    /// <summary>
    /// Called when the pointer goes down.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerDown(PointerEventArgs e)
    {
        CoreChart?.InvokePointerDown(new LvcPoint((float)e.OffsetX, (float)e.OffsetY), e.Button == 2);
        _ = OnPointerDownCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called when the pointer moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerMove(PointerEventArgs e)
    {
        CoreChart?.InvokePointerMove(new LvcPoint((float)e.OffsetX, (float)e.OffsetY));
        _ = OnPointerMoveCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called when the pointer goes up.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerUp(PointerEventArgs e)
    {
        CoreChart?.InvokePointerUp(new LvcPoint((float)e.OffsetX, (float)e.OffsetY), e.Button == 2);
        _ = OnPointerUpCallback.InvokeAsync(e);
    }

    /// <summary>
    /// Called then mouse wheel moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnWheel(WheelEventArgs e) { }

    /// <summary>
    /// Called when the pointer leaves the control.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerOut(PointerEventArgs e) => CoreChart?.InvokePointerLeft();

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(
        LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
            => CoreChart.GetPointsAt(point, strategy, findPointFor);

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
        => CoreChart.GetVisualsAt(point);

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints) =>
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer) =>
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs(CoreChart, visualElements, pointer));

    void IChartView.Invalidate() => CoreCanvas.Invalidate();

    void IDisposable.Dispose()
    {
        Observe.Dispose();
        CoreChart.Unload();
    }
}
