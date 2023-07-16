
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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Command arguments that describe a pointer event in a LiveChart view.
/// </summary>
public class PointerCommandArgs : ChartCommandArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointerCommandArgs"/> class.
    /// </summary>
    /// <param name="chart">The chart that fired the event.</param>
    /// <param name="pointerPosition">The pointer position.</param>
    /// <param name="originalEventArgs">The original event args.</param>
    public PointerCommandArgs(
        IChartView chart,
        LvcPointD pointerPosition,
        object originalEventArgs)
            : base(chart)
    {
        PointerPosition = pointerPosition;
        OriginalEventArgs = originalEventArgs;
    }

    /// <summary>
    /// Gets the pointer position relative to the chart.
    /// </summary>
    public LvcPointD PointerPosition { get; set; }

    /// <summary>
    /// Gets the framework-specific event arguments.
    /// </summary>
    public object OriginalEventArgs { get; set; }
}
