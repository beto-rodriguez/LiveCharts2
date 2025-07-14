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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
#if NET462
using System.Linq;
#endif

namespace LiveChartsCore.Kernel.Observers;

/// <summary>
/// A Class that tracks both, <see cref="INotifyCollectionChanged.CollectionChanged"/> event and 
/// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event of each element in the collection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CollectionDeepObserver"/> class.
/// </remarks>
/// <param name="onChange">
/// An action that is called when the collection items or a property in an item in the collection change.
/// </param>
/// <param name="onItemAdded">
/// if specified, this action is called for each new item in the collection.
/// This action is also called for each item when the collection is initialized.
/// </param>
/// <param name="onItemRemoved">
/// if specified, this acction is called each time an item is removed in the collection.
/// This action is also called for each item when the collection is disposed.
/// </param>
public class CollectionDeepObserver(
    Action onChange,
    Action<object>? onItemAdded = null,
    Action<object>? onItemRemoved = null)
        : IObserver
{
    private readonly HashSet<INotifyPropertyChanged> _itemsListening = [];
    private IEnumerable? _trackedCollection;

    /// <inheritdoc cref="IObserver.Initialize(object?)"/>
    public void Initialize(object? instance)
    {
        if (_trackedCollection == instance)
            return;

        if (_trackedCollection is not null)
            Dispose();

        if (instance is not IEnumerable enumerable) return;

        if (instance is INotifyCollectionChanged incc)
            incc.CollectionChanged += OnCollectionChanged;

        OnItemsAdded(enumerable);

        _trackedCollection = enumerable;
    }

    /// <inheritdoc cref="IObserver.Dispose"/>
    public void Dispose()
    {
        if (_trackedCollection is null) return;

        if (_trackedCollection is INotifyCollectionChanged incc)
            incc.CollectionChanged -= OnCollectionChanged;

        OnItemsRemoved(_trackedCollection);

        _trackedCollection = null;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                if (e.NewItems is not null)
                    OnItemsAdded(e.NewItems);

                break;

            case NotifyCollectionChangedAction.Remove:

                if (e.OldItems is not null)
                    OnItemsRemoved(e.OldItems);

                break;

            case NotifyCollectionChangedAction.Replace:

                if (e.OldItems is not null)
                    OnItemsRemoved(e.OldItems);

                if (e.NewItems is not null)
                    OnItemsAdded(e.NewItems);

                break;

            case NotifyCollectionChangedAction.Reset:

#if NET462
                OnItemsRemoved(_itemsListening.ToArray());
#else
                OnItemsRemoved(_itemsListening);
#endif
                _itemsListening.Clear();

                if (sender is IEnumerable enumerable)
                    OnItemsAdded(enumerable);

                break;

            case NotifyCollectionChangedAction.Move:
            default:

                break;
        }

        onChange();
    }

    private void OnItemsAdded(IEnumerable newItems)
    {
        foreach (var item in newItems)
        {
            if (item is INotifyPropertyChanged inpcItem)
            {
                if (_itemsListening.Add(inpcItem))
                {
                    inpcItem.PropertyChanged += OnItemPropertyChanged;
                }
            }

            onItemAdded?.Invoke(item);
        }
    }

    private void OnItemsRemoved(IEnumerable oldItems)
    {
        foreach (var item in oldItems)
        {
            if (item is INotifyPropertyChanged inpcItem && _itemsListening.Remove(inpcItem))
            {
                inpcItem.PropertyChanged -= OnItemPropertyChanged;
            }

            onItemRemoved?.Invoke(item);
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e) => onChange();
}
