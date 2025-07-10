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

namespace LiveChartsCore.Kernel.Observers;

/// <summary>
/// An <see cref="IObserver"/> collection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ChartObserver"/> class.
/// </remarks>
/// <param name="updateChart">A delegate that updates the chart.</param>
/// <param name="addToUI">A delegate that adds an element to the UI.</param>
/// <param name="removeFromUI">A delegate that removes an element from the UI.</param>
public class ChartObserver(
    Action updateChart,
    Action<object>? addToUI = null,
    Action<object>? removeFromUI = null)
        : Dictionary<string, IObserver>
{
    /// <summary>
    /// Observes a collection of items, and tracks changes in the collection and in each item.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ChartObserver Collection(string key)
    {
        this[key] = new CollectionDeepObserver(updateChart, addToUI, removeFromUI);
        return this;
    }

    /// <summary>
    /// Observes a property change in an item, and updates the chart when the property changes.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ChartObserver Property(string key)
    {
        this[key] = new PropertyChangeObserver(updateChart, addToUI, removeFromUI);
        return this;
    }

    /// <summary>
    /// Disposes the contained observers.
    /// </summary>
    public void Dispose()
    {
        foreach (var item in Values)
            item.Dispose();

        Clear();

        updateChart = null!;
        addToUI = null!;
        removeFromUI = null!;
    }
}
