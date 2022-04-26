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

using LiveChartsCore.SkiaSharpView.Blazor.JsInterop.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <summary>
/// An object that handles the comminication with the DOM.
/// </summary>
public class DomJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private static readonly Dictionary<string, List<Action<DOMRect>>> s_resizeEvent = new();

    /// <summary>
    /// Initialized a new instance of the <see cref="DomJsInterop"/> class.
    /// </summary>
    /// <param name="jsRuntime"></param>
    public DomJsInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/LiveChartsCore.SkiaSharpView.Blazor/domInterop.js")
            .AsTask());
    }

    /// <summary>
    /// Gets the bounding client rectangle of the given element.
    /// </summary>
    /// <param name="elementReference">The HTMl element reference.</param>
    /// <returns></returns>
    public async ValueTask<DOMRect> GetBoundingClientRect(ElementReference elementReference)
    {
        var module = await _moduleTask.Value;

        return await module.InvokeAsync<DOMRect>("DOMInterop.getBoundingClientRect", elementReference);
    }

    /// <summary>
    /// Sets the css top and left properties of he given element to the specified coordinates.
    /// </summary>
    /// <param name="elementReference">The HTML element.</param>
    /// <param name="x">The x coordinate (left property in css).</param>
    /// <param name="y">The y coordinate (top property in css).</param>
    /// <param name="relativeTo">Indicates whether the function should add the given element postion to each coordinate.</param>
    /// <returns></returns>
    public async ValueTask SetPosition(
        ElementReference elementReference, double x, double y, ElementReference? relativeTo = null)
    {
        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("DOMInterop.setPosition", elementReference, x, y, relativeTo);
    }

    /// <summary>
    /// Registers a handler for the resize observer for the given HTML element.
    /// </summary>
    /// <param name="element">The HTML element.</param>
    /// <param name="elementId">The elemnt id.</param>
    /// <param name="handler">The handler.</param>
    /// <returns></returns>
    public async ValueTask OnResize(ElementReference element, string elementId, Action<DOMRect> handler)
    {
        if (!s_resizeEvent.TryGetValue(elementId, out var actions))
        {
            actions = new List<Action<DOMRect>>();
            s_resizeEvent.Add(elementId, actions);
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("DOMInterop.registerResizeListener", element, elementId);
        }

        actions.Add(handler);
    }

    /// <summary>
    /// Removes the handler from the specified element id.
    /// </summary>
    /// <param name="elementId">The element id.</param>
    public void RemoveOnResizeListener(string elementId)
    {
        _ = s_resizeEvent.Remove(elementId);
    }

    /// <summary>
    /// Removes the given resize handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void RemoveOnResizeListener(Action<DOMRect> handler)
    {
        foreach (var elementId in s_resizeEvent.Keys.ToArray())
        {
            var actions = s_resizeEvent[elementId];

            foreach (var action in actions)
            {
                if (action != handler) continue;

                _ = actions.Remove(handler);
                if (actions.Count == 0) _ = s_resizeEvent.Remove(elementId);
            }
        }
    }

    /// <summary>
    /// Called when a HTML element was resized.
    /// </summary>
    /// <param name="elementId"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    [JSInvokable("InvokeResize")]
    public static Task InvokeResize(string elementId, DOMRect newSize)
    {
        if (!s_resizeEvent.TryGetValue(elementId, out var actions))
        {
            // it was probably already disposed.
            return Task.CompletedTask;
        }

        foreach (var handler in actions)
        {
            handler.Invoke(newSize);
        }

        return Task.CompletedTask;
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
