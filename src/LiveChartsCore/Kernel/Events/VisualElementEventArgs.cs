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
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Defines the visual elements event arguments.
/// </summary>
public class VisualElementEventArgs<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VisualElementsEventArgs{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    /// <param name="visualElement">The visual elements.</param>
    public VisualElementEventArgs(
        Chart<TDrawingContext> chart, VisualElement<TDrawingContext> visualElement, LvcPoint pointerLocation)
    {
        Chart = chart;
        PointerLocation = pointerLocation;
        VisualElement = visualElement;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualElementsEventArgs{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    /// <param name="visualElement">The visual element.</param>
    public VisualElementEventArgs(
        IChart chart, VisualElement<TDrawingContext> visualElement, LvcPoint pointerLocation)
    {
        Chart = (Chart<TDrawingContext>)chart;
        PointerLocation = pointerLocation;
        VisualElement = visualElement;
    }

    /// <summary>
    /// Gets the chart.
    /// </summary>
    public Chart<TDrawingContext> Chart { get; }

    /// <summary>
    /// Gets or sets the pointer location.
    /// </summary>
    public LvcPoint PointerLocation { get; }

    /// <summary>
    /// Gets the visual elements found.
    /// </summary>
    public VisualElement<TDrawingContext> VisualElement { get; }
}
