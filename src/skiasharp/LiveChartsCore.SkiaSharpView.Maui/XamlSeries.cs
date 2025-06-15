
// this file defines the xaml classes, most of the work is done automatically by the generator.
// The generator just wraps LiveCharts object in a Xaml object, then when a
// property changes in the Xaml object, it updates the LiveCharts object.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0032 // Use auto property
#pragma warning disable IDE0052 // Remove unread private members

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

public abstract class XamlSeries : EmptyContentView
{
    public static readonly BindableProperty AdditionalVisualStatesProperty = BindableProperty.Create(
        nameof(AdditionalVisualStates), typeof(ICollection<ChartPointState>), typeof(XamlSeries),
        defaultValueCreator: DefaultValueCreator);

    public ICollection<ChartPointState> AdditionalVisualStates =>
        (ICollection<ChartPointState>)GetValue(AdditionalVisualStatesProperty);

    protected abstract ISeries WrappedSeries { get; }

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
        if (sender is not ChartPointState.SetterCollection setters) return;

        WrappedSeries.VisualStates[setters.Name] = setters.AsStatesCollection();
    }
}
