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
/// A control in the UI that is able to report its size via JS interop.
/// </summary>
public partial class JsFlexibleContainer : IDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private readonly string _id = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the width.
    /// </summary>
    public double Width { get; private set; }

    /// <summary>
    /// Gets the height.
    /// </summary>
    public double Height { get; private set; }

    /// <summary>
    /// Gets the container.
    /// </summary>
    public ElementReference Container { get; private set; }

    /// <summary>
    /// Ges whether the control is disposing.
    /// </summary>
    public bool Disposing { get; private set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    [Parameter]
    public RenderFragment? Content { get; set; }

    /// <summary>
    /// Gets or sets the container class.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Called when the control is resized.
    /// </summary>
    public event Action<JsFlexibleContainer>? Resized;

    /// <summary>
    /// Called when the control is rendered.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_dom is null) _dom = new DomJsInterop(JS);

        var wrapperBounds = await _dom.GetBoundingClientRect(Container);

        Width = wrapperBounds.Width;
        Height = wrapperBounds.Height;

        await _dom.OnResize(Container, _id, OnContainerResized);
    }

    /// <summary>
    /// Called when the container was resized.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void OnContainerResized(DOMRect newSize)
    {
        Width = newSize.Width;
        Height = newSize.Height;

        Resized?.Invoke(this);
    }

    async void IDisposable.Dispose()
    {
        if (_dom is null) return;
        _dom.RemoveOnResizeListener(_id);
        Disposing = true;
        await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
