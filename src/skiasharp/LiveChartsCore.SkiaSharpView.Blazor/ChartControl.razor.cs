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

// ==============================================================================
// 
// this file contains the Blazor specific code for the ChartControl class,
// the rest of the code can be found in the _Shared project.
// 
// ==============================================================================

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
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

        InitializeObservers();

        // will be initialized in OnAfterRender, because we need the canvas element reference
        CoreChart = null!;
    }

    /// <inheritdoc cref="IChartView.CoreCanvas"/>
    public MotionCanvas CanvasView => _motionCanvas;

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

        InitializeCoreChart();

        _jsFlexibleContainer.Resized +=
            container => CoreChart?.Update();

        CoreChart.Load();
    }

    bool IChartView.DesignerMode => false;
    bool IChartView.IsDarkMode => false; // Is this possible in Blazor?
    LvcColor IChartView.BackColor { get; set; } // is this even used in livecharts? isnt this obsolete?

    LvcSize IChartView.ControlSize => new() { Width = (float)_jsFlexibleContainer.Width, Height = (float)_jsFlexibleContainer.Height };

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

    void IDisposable.Dispose()
    {
        Observe.Dispose();
        CoreChart.Unload();
    }
}
