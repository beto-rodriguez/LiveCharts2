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
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Kernel.Events;

/// <summary>
/// Defines the visual elements event arguments.
/// </summary>
public class VisualElementsEventArgs<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private VisualElement<TDrawingContext>? _closer;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualElementsEventArgs{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="pointerLocation">The pointer location.</param>
    /// <param name="visualElements">The visual elements.</param>
    public VisualElementsEventArgs(IEnumerable<VisualElement<TDrawingContext>> visualElements, LvcPoint pointerLocation)
    {
        PointerLocation = pointerLocation;
        VisualElements = visualElements;
    }

    /// <summary>
    /// Gets or sets the pointer location.
    /// </summary>
    public LvcPoint PointerLocation { get; }

    /// <summary>
    /// Gets the closest visual element to the pointer position.
    /// </summary>
    public VisualElement<TDrawingContext>? ClosestToPointerVisualElement => _closer ??= FindClosest();

    /// <summary>
    /// Gets all the visual elements that were found.
    /// </summary>
    public IEnumerable<VisualElement<TDrawingContext>> VisualElements { get; }

    private VisualElement<TDrawingContext>? FindClosest()
    {
        return VisualElements.FindClosestTo(PointerLocation);
    }
}
