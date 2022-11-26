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

using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IChartLegend{TDrawingContext}"/>
public partial class DefaultLegend : IChartLegend<SkiaSharpDrawingContext>, IDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private DomJsInterop? _dom;
    private ElementReference _wrapper;

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
    /// Gets or sets the legend template.
    /// </summary>
    [Parameter]
    public RenderFragment<ISeries[]>? LegendTemplate { get; set; }

    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    public ISeries[] Series { get; set; } = Array.Empty<ISeries>();

    async void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var series = chart.ChartSeries.Where(x => x.IsVisibleAtLegend);
        var legendOrientation = LegendOrientation.Auto;
        var legendPosition = chart.LegendPosition;

        if (legendOrientation != LegendOrientation.Auto)
            throw new Exception("Custom orientation is not supported.");

        Series = series.ToArray();

        var blazorChart = (IBlazorChart)chart.View;

        // by default the chart css is a flex box with row direction
        switch (legendPosition)
        {
            case LegendPosition.Hidden:
                blazorChart.LegendClass = "closed";
                break;
            case LegendPosition.Top:
                blazorChart.LegendClass = "start row-fx";
                blazorChart.ContainerClass = "column";
                break;
            case LegendPosition.Left:
                blazorChart.LegendClass = "start column";
                blazorChart.ContainerClass = "";
                break;
            case LegendPosition.Right:
                blazorChart.LegendClass = "";
                blazorChart.ContainerClass = "";
                break;
            case LegendPosition.Bottom:
                blazorChart.LegendClass = "row-fx";
                blazorChart.ContainerClass = "column";
                break;
            default:
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    async void IDisposable.Dispose()
    {
        if (_dom is null) return;
        await ((IAsyncDisposable)_dom).DisposeAsync();
    }
}
