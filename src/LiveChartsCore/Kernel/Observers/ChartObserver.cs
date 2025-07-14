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
public class ChartObserver : Dictionary<string, ChartObserver.Map>
{
    private bool _isInitialized = false;
    private readonly Action _updateChart;
    private readonly Action<object>? _addToUI;
    private readonly Action<object>? _removeFromUI;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartObserver"/> class.
    /// </summary>
    /// <param name="builder">The builder action that configures the observer.</param>
    /// <param name="updateChart">A delegate that updates the chart.</param>
    /// <param name="addToUI">A delegate that adds an element to the UI.</param>
    /// <param name="removeFromUI">A delegate that removes an element from the UI.</param>
    public ChartObserver(
        Func<ChartObserver, ChartObserver> builder,
        Action updateChart,
        Action<object>? addToUI = null,
        Action<object>? removeFromUI = null)
    {
        _updateChart = updateChart;
        _addToUI = addToUI;
        _removeFromUI = removeFromUI;

        _ = builder(this);
    }

    /// <summary>
    /// Gets a value indicating whether the observer is disposed.
    /// </summary>
    public bool IsDisposed => Values.Count == 0;

    /// <summary>
    /// Observes a collection of items, and tracks changes in the collection and in each item.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="propertyGetter"></param>
    /// <returns>The current instance.</returns>
    public ChartObserver Collection(string key, Func<object> propertyGetter)
    {
        return AddObserver(
            new CollectionDeepObserver(_updateChart, _addToUI, _removeFromUI), key, propertyGetter);
    }

    /// <summary>
    /// Observes a property change in an item, and updates the chart when the property changes.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="propertyGetter">A function that gets the value of the property.</param>
    /// <returns>The current instance.</returns>
    public ChartObserver Property(string key, Func<object> propertyGetter)
    {
        return AddObserver(
            new PropertyChangeObserver(_updateChart, _addToUI, _removeFromUI), key, propertyGetter);
    }

    /// <summary>
    /// Adds an observer to the collection with a specified key and property getter.
    /// </summary>
    /// <param name="observer">The observer instance to add.</param>
    /// <param name="key">The property name.</param>
    /// <param name="propertyGetter">A function that gets the value of the property.</param>
    /// <returns>The current instance.</returns>
    public ChartObserver AddObserver(IObserver observer, string key, Func<object> propertyGetter)
    {
        this[key] = new Map(key, observer, propertyGetter);
        return this;
    }

    /// <summary>
    /// Initializes the observers with the current values of the properties they observe.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
            return;

        foreach (var map in Values)
            map.Observer.Initialize(map.Getter());

        _isInitialized = true;
    }

    /// <summary>
    /// Disposes the contained observers.
    /// </summary>
    public void Dispose()
    {
        foreach (var map in Values)
            map.Observer.Dispose();

        _isInitialized = false;
    }

    /// <summary>
    /// Degfines an observer map.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="observer"> The observer instance.</param>
    /// <param name="getter">A function that gets the value of the property.</param>
    public class Map(string name, IObserver observer, Func<object> getter)
    {
        /// <summary>
        /// Gets the name of the observer map.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the observer instance.
        /// </summary>
        public IObserver Observer { get; } = observer;

        /// <summary>
        /// Gets the getter function for the property value.
        /// </summary>
        public Func<object> Getter { get; } = getter;
    }
}
