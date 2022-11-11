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
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <summary>
/// The LiveCharts-Blazor config.
/// </summary>
public static class LiveChartsBlazor
{
    /// <summary>
    /// Gets the given series as miniatures paint tasks.
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    public static List<PaintSchedule<SkiaSharpDrawingContext>> GetSeriesAsMiniaturePaints(ISeries series)
    {
        var skSeries = (IChartSeries<SkiaSharpDrawingContext>)series;
        return skSeries.CanvasSchedule.PaintSchedules;
    }

    /// <summary>
    /// Gets the given series as minitaures style.
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    public static string GetSeriesMiniatureStyle(ISeries series)
    {
        var skSeries = (IChartSeries<SkiaSharpDrawingContext>)series;
        return $"width: {skSeries.CanvasSchedule.Width}px; height: {skSeries.CanvasSchedule.Height}px";
    }

    /// <summary>
    /// Gets the given series as miniatures data.
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    public static Sketch<SkiaSharpDrawingContext> GetSeriesMiniatureData(ISeries series)
    {
        var skSeries = (IChartSeries<SkiaSharpDrawingContext>)series;
        return skSeries.CanvasSchedule;
    }
}
