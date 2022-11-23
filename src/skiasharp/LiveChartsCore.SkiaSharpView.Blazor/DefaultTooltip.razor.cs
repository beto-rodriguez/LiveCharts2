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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}"/>
public partial class DefaultTooltip : IChartTooltip<SkiaSharpDrawingContext>, IDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private ElementReference _wrapper;
    private IBlazorChart? _chart = null;

    /// <summary>
    /// Called when the control renders.
    /// </summary>
    /// <param name="firstRender"></param>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        _dom ??= new DomJsInterop(JS);
    }

    /// <summary>
    /// Gets ir sets the class.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = "closed";

    /// <summary>
    /// Gets or sets the tooltip legend.
    /// </summary>
    [Parameter]
    public RenderFragment<ChartPoint[]>? TooltipTemplate { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    public ChartPoint[] Points { get; set; } = Array.Empty<ChartPoint>();

    async void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        if (_dom is null) return;

        var blazorChart = (IBlazorChart)chart.View;
        _chart = blazorChart;

        _chart.TooltipClass = "";
        Points = tooltipPoints.ToArray();

        await InvokeAsync(StateHasChanged);
        var clientRect = await _dom.GetBoundingClientRect(_wrapper);
        var tooltipSize = new LvcSize((float)clientRect.Width, (float)clientRect.Height);

        var location = tooltipPoints.GetTooltipLocation(tooltipSize, chart);

        await _dom.SetPosition(_wrapper, location.X, location.Y, blazorChart.CanvasContainerElement);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        if (_chart is null) return;

        // ToDo:
        // the LiveCharts API should inject the chart to this method.

        _chart.TooltipClass = "closed";
    }

    async void IDisposable.Dispose()
    {
        if (_dom is null) return;
        await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
