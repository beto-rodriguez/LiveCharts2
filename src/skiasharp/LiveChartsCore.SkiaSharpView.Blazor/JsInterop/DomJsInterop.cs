using LiveChartsCore.SkiaSharpView.Blazor.JsInterop.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor
{
    public class DomJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private static readonly Dictionary<string, List<Action<DOMRect>>> s_resizeEvent = new();

        public DomJsInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() =>
                jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/LiveChartsCore.SkiaSharpView.Blazor/DOMInterop.js")
                .AsTask());
        }

        public async ValueTask<DOMRect> GetBoundingClientRect(ElementReference elementReference)
        {
            var module = await _moduleTask.Value;

            return await module.InvokeAsync<DOMRect>("DOMInterop.getBoundingClientRect", elementReference);
        }

        /// <summary>
        /// Sets the css top and left properties of he given element to the specified coordinates.
        /// </summary>
        /// <param name="elementReference">The html element.</param>
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

        public void RemoveOnResizeListener(string elementId)
        {
            _ = s_resizeEvent.Remove(elementId);
        }

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
}
