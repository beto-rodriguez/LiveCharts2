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
using System.ComponentModel;

namespace LiveChartsCore.Kernel.Observers;

/// <summary>
/// A class that tracks the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CollectionDeepObserver"/> class.
/// </remarks>
/// <param name="onChange">
/// An action that is called when a property in the tracked item.
/// </param>
/// <param name="onInitialized">
/// An action that is called when the observer is initialized with an instance.
/// </param>
/// <param name="onDisposed">
/// An action that is called when the observer is disposed.
/// </param>
public class PropertyChangeObserver(
    Action onChange,
    Action<object>? onInitialized = null,
    Action<object>? onDisposed = null)
        : IObserver
{
    private INotifyPropertyChanged? _trackedElement;

    /// <inheritdoc cref="IObserver.Initialize(object?)"/>
    public void Initialize(object? instance)
    {
        if (_trackedElement == instance)
            return;

        if (_trackedElement is not null)
            Dispose();

        if (instance is not INotifyPropertyChanged inpc) return;

        onInitialized?.Invoke(instance);
        inpc.PropertyChanged += OnPropertyChanged;

        _trackedElement = inpc;
    }

    /// <inheritdoc cref="IObserver.Dispose"/>
    public void Dispose()
    {
        if (_trackedElement is null) return;

        _trackedElement.PropertyChanged -= OnPropertyChanged;
        onDisposed?.Invoke(_trackedElement);

        _trackedElement = null;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => onChange();
}
