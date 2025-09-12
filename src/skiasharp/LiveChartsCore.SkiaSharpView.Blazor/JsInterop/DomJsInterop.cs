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

using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor.JsInterop;

/// <summary>
/// An object that handles the comminication with the DOM.
/// </summary>
/// <remarks>
/// Initialized a new instance of the <see cref="DomJsInterop"/> class.
/// </remarks>
/// <param name="jsRuntime"></param>
public class DomJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() =>
        jsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "./_content/LiveChartsCore.SkiaSharpView.Blazor/domInterop.js")
        .AsTask());

    /// <summary>
    /// Starts the frame ticker, this will call the OnFrameTick method in the DotNetObjectReference.
    /// </summary>
    /// <param name="motionCanvasRef">The dotnet ref to the motion canvas.</param>
    public async ValueTask StartFrameTicker(DotNetObjectReference<MotionCanvas> motionCanvasRef)
    {
        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("DOMInterop.startFrameTicker", motionCanvasRef);
    }

    /// <summary>
    /// Stops the frame ticker, this will stop calling the OnFrameTick method in the DotNetObjectReference.
    /// </summary>
    /// <returns></returns>
    public async ValueTask StopFrameTicker(DotNetObjectReference<MotionCanvas> motionCanvasRef)
    {
        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("DOMInterop.stopFrameTicker", motionCanvasRef);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
