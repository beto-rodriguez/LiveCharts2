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

// Ignore Spelling: Crosshair Subticks Subseparators

using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore;

/// <summary>
/// A helper class that configures shared axes.
/// </summary>
public static class SharedAxes
{
    /// <summary>
    /// Sets the specified axes as shared.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="axes">The axes to share.</param>
    public static void Set<TDrawingContext>(params ICartesianAxis<TDrawingContext>[] axes)
        where TDrawingContext : DrawingContext
    {
        var sharedInstance = new HashSet<CartesianChart<TDrawingContext>>();

        foreach (var axis in axes)
        {
            axis.SharedWith = axes
                .Where(x => x != axis)
                .ToArray();

            axis.MeasureStarted += (IChart chart, ICartesianAxis obj) =>
            {
                var cc = (CartesianChart<TDrawingContext>)chart;
                cc.SubscribeSharedEvents(sharedInstance);
            };
        }
    }
}
