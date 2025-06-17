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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// A base series class for XAML-based series in LiveCharts.
/// </summary>
public abstract class XamlSeries : EmptyContentView
{
    /// <summary>
    /// The AdditionalVisualStates property, which holds additional visual states for the series.
    /// </summary>
    public static readonly BindableProperty AdditionalVisualStatesProperty = BindableProperty.Create(
        nameof(AdditionalVisualStates), typeof(ICollection<ChartPointState>), typeof(XamlSeries),
        defaultValueCreator: DefaultValueCreator);

    /// <summary>
    /// Gets the additional visual states for the series.
    /// </summary>
    public ICollection<ChartPointState> AdditionalVisualStates =>
        (ICollection<ChartPointState>)GetValue(AdditionalVisualStatesProperty);

    /// <summary>
    /// Gets the wrapped series that this XAML series represents.
    /// </summary>
    protected abstract ISeries WrappedSeries { get; }

    /// <summary>
    /// Maps the specified value to the underlying series' values collection.
    /// </summary>
    protected void ValuesMap(object value) => WrappedSeries.Values = (IEnumerable)value;

    private static object DefaultValueCreator(BindableObject bindable)
    {
        var series = (XamlSeries)bindable;
        var observableCollection = new ObservableCollection<ChartPointState>();

        observableCollection.CollectionChanged += series.OnAdditionalVisualStatesCollectionChanged;

        return observableCollection;
    }

    private void OnAdditionalVisualStatesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems ?? Array.Empty<object>())
                {
                    var state = (ChartPointState)item;
                    state.Setters.CollectionChanged += OnSetCollectionChanged;
                    WrappedSeries.VisualStates[state.Name] = state.Setters.AsStatesCollection();
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems ?? Array.Empty<object>())
                {
                    var state = (ChartPointState)item;
                    state.Setters.CollectionChanged -= OnSetCollectionChanged;
                    _ = WrappedSeries.VisualStates.Remove(state.Name);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
            default:
                break;
        }
    }

    private void OnSetCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not Generators.BaseChartPointState.SetterCollection setters) return;

        WrappedSeries.VisualStates[setters.Name] = setters.AsStatesCollection();
    }
}
