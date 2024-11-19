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

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Defines the visual elements event arguments.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VisualElementsEventArgs"/> class.
/// </remarks>
/// <param name="chart">The chart.</param>
/// <param name="pointerLocation">The pointer location.</param>
/// <param name="visualElements">The visual elements.</param>
public class VisualElementsEventArgs(
    IChart chart,
    IEnumerable<CoreVisualElement> visualElements,
    LvcPoint pointerLocation)
{
    private CoreVisualElement? _closer;

    /// <summary>
    /// Gets the chart.
    /// </summary>
    public IChart Chart { get; } = chart;

    /// <summary>
    /// Gets or sets the pointer location.
    /// </summary>
    public LvcPoint PointerLocation { get; } = pointerLocation;

    /// <summary>
    /// Gets the closest visual element to the pointer position.
    /// </summary>
    public CoreVisualElement? ClosestToPointerVisualElement => _closer ??= FindClosest();

    /// <summary>
    /// Gets all the visual elements that were found.
    /// </summary>
    public IEnumerable<CoreVisualElement> VisualElements { get; } = visualElements;

    private CoreVisualElement? FindClosest()
    {
        return VisualElements.Select(visual =>
        {
            var size = visual.Measure(Chart);

            return new
            {
                distance = Math.Sqrt(
                    Math.Pow(PointerLocation.X - (visual.X + size.Width * 0.5), 2) +
                    Math.Pow(PointerLocation.Y - (visual.Y + size.Height * 0.5), 2)),
                visual
            };
        })
       .OrderBy(p => p.distance)
       .FirstOrDefault()?.visual;
    }
}
