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
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Drawing;

/// <summary>
/// Defines a hover area.
/// </summary>
public abstract class HoverArea
{
    /// <summary>
    /// Gets the distance to a given point.
    /// </summary>
    /// <param name="point">The point to calculate the distance to.</param>
    /// <returns>The distance in pixels.</returns>
    public abstract double DistanceTo(LvcPoint point);

    /// <summary>
    /// Determines whether the pointer is over the area.
    /// </summary>
    /// <param name="pointerLocation">The pointer location.</param>
    /// <param name="strategy">The strategy.</param>
    /// <returns></returns>
    public abstract bool IsPointerOver(LvcPoint pointerLocation, TooltipFindingStrategy strategy);

    /// <summary>
    /// Suggests the tooltip placement.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="tooltipSize">Size of the tooltip.</param>
    public abstract void SuggestTooltipPlacement(TooltipPlacementContext context, LvcSize tooltipSize);
}
