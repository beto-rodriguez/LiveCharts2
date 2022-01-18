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

using System.Collections.Generic;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Defines the chart point event arguments.
/// </summary>
public class ChartPointEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPointEventArgs"/> class.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    public ChartPointEventArgs(IEnumerable<ChartPoint> points, LvcPoint pointerLocation)
    {
        Points = points;
        PointerLocation = pointerLocation;
    }

    /// <summary>
    /// Gets a list of the points triggered by the event.
    /// </summary>
    public IEnumerable<ChartPoint> Points { get; }

    /// <summary>
    /// Gets the pointer location when the event was called.
    /// </summary>
    public LvcPoint PointerLocation { get; }
}

/// <summary>
/// Defines the chart point event arguments.
/// </summary>
public class ChartPointEventArgs<TModel, TVisual, TLabel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPointEventArgs{TModel, TVisual, TLabel}"/> class.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="pointerLocation">The pointer location.</param>
    public ChartPointEventArgs(IEnumerable<ChartPoint<TModel, TVisual, TLabel>> points, LvcPoint pointerLocation)
    {
        Points = points;
        PointerLocation = pointerLocation;
    }

    /// <summary>
    /// Gets a list of the points triggered by the event.
    /// </summary>
    public IEnumerable<ChartPoint<TModel, TVisual, TLabel>> Points { get; }

    /// <summary>
    /// Gets the pointer location when the event was called.
    /// </summary>
    public LvcPoint PointerLocation { get; }
}

