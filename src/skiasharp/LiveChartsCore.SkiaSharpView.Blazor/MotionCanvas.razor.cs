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

using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Blazor.JsInterop;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp.Views.Blazor;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="CoreMotionCanvas"/>
public partial class MotionCanvas : IDisposable, IRenderMode
{
    private SKGLView? _glView;
    private DotNetObjectReference<MotionCanvas>? _dotNetRef;
    private DomJsInterop? _dom;
    private IFrameTicker _ticker = null!;

    /// <summary>
    /// Gets the recommended rendering settings for Blazor.
    /// </summary>
    public static RenderingSettings RecommendedBlazorRenderingSettings { get; }
        = new()
        {
            // Actually only GL view is supported in Blazor.
            UseGPU = true,

            // uses requestAnimationFrame under the hood.
            TryUseVSync = true,

            // A fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };

    static MotionCanvas()
    {
        LiveCharts.Configure(config => config.UseDefaults(RecommendedBlazorRenderingSettings));
    }

    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="CoreMotionCanvas"/> (core).
    /// </summary>
    public CoreMotionCanvas CanvasCore { get; } = new();

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

    /// <summary>
    /// Gets or sets the wheel changed callback.
    /// </summary>
    [Parameter]
    public EventCallback<WheelEventArgs> OnWheelCallback { get; set; }

    /// <summary>
    /// Gets or sets the pointer leave callback.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerOutCallback { get; set; }

    event CoreMotionCanvas.FrameRequestHandler IRenderMode.FrameRequest
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Called when the pointer goes down.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerDown(PointerEventArgs e) =>
        _ = OnPointerDownCallback.InvokeAsync(e);

    /// <summary>
    /// Called when the pointer moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerMove(PointerEventArgs e) =>
        _ = OnPointerMoveCallback.InvokeAsync(e);

    /// <summary>
    /// Called when the pointer goes up.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerUp(PointerEventArgs e) =>
        _ = OnPointerUpCallback.InvokeAsync(e);

    /// <summary>
    /// Called when the wheel moves.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnWheel(WheelEventArgs e) => _ = OnWheelCallback.InvokeAsync(e);

    /// <summary>
    /// Called when the pointer leaves the control.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPointerOut(PointerEventArgs e) =>
        _ = OnPointerOutCallback.InvokeAsync(e);

    private void OnPaintGlSurface(SKPaintGLSurfaceEventArgs e) =>
        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(CanvasCore, e.Surface.Canvas, SkiaSharp.SKColor.Empty));

    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;

        _dom ??= new DomJsInterop(JS);
        _dotNetRef = DotNetObjectReference.Create(this);

        _ticker = LiveCharts.RenderingSettings.TryUseVSync
            ? new RequestAnimationFrameTicker(_dom, _dotNetRef)
            : new AsyncLoopTicker();

        _ticker.InitializeTicker(CanvasCore, this);
    }

    void IDisposable.Dispose()
    {
        _ticker?.DisposeTicker();
        _ticker = null!;
        _glView?.Dispose();
        _ = (_dom?.StopFrameTicker(_dotNetRef!));
        _dotNetRef?.Dispose();
        _dotNetRef = null;
        _dom = null;
    }

    /// <summary>
    /// Called when the frame ticker ticks.
    /// </summary>
    [JSInvokable]
    public void OnFrameTick()
    {
        if (CanvasCore.IsValid) return;
        _glView.Invalidate();
    }

    void IRenderMode.InitializeRenderMode(CoreMotionCanvas canvas) =>
        throw new NotImplementedException();

    void IRenderMode.InvalidateRenderer() =>
        _glView?.Invalidate();

    void IRenderMode.DisposeRenderMode() =>
        throw new NotImplementedException();

    internal class RequestAnimationFrameTicker(
        DomJsInterop jsInterop, DotNetObjectReference<MotionCanvas> dotnetRef)
            : IFrameTicker
    {
        public async void InitializeTicker(CoreMotionCanvas canvas, IRenderMode renderMode)
        {
            await jsInterop.StartFrameTicker(dotnetRef);
            CoreMotionCanvas.s_tickerName = $"{nameof(RequestAnimationFrameTicker)}";
        }

        public async void DisposeTicker() =>
            await jsInterop.StopFrameTicker(dotnetRef);
    }
}
