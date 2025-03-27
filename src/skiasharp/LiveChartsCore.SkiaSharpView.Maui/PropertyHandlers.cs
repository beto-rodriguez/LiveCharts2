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
using System.Collections.Specialized;
using System;
using Microsoft.Maui.Controls;
using LiveChartsCore.Kernel;
using System.Collections;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Maui;

internal class PropertyHandlers<TChart>
    where TChart : ChartView
{
    public static void OnChanged(BindableObject bo, object o, object n)
    {
        var chart = (TChart)bo;
        chart.CoreChart.Update();
    }

    public static void OnUIElementChanged(BindableObject bo, object o, object n)
    {
        var chart = (TChart)bo;

        if (o is View oldView)
            _ = chart.CanvasView.Children.Remove(oldView);

        if (n is View newView)
            chart.CanvasView.Children.Add(newView);

        chart.CoreChart?.Update();
    }

    public static BindableProperty.BindingPropertyChangedDelegate OnUIElementsCollectionChanged<TCollection>(
        Func<TChart, CollectionDeepObserver<TCollection>> observerGetter)
    {
        return (bo, o, n) =>
        {
            var chart = (TChart)bo;
            var observer = observerGetter(chart);

            var oldCollection = (IEnumerable<TCollection>?)o;
            observer?.Dispose(oldCollection);

            var newCollection = (IEnumerable<TCollection>?)n;
            observer?.Initialize(newCollection);

            if (newCollection is INotifyCollectionChanged incc)
            {
                // add and remove items from the UI when the collection changes.
                incc.CollectionChanged += (s, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            AddItemsToCanvas(chart.CanvasView, e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            RemoveItemsFromCanvas(chart.CanvasView, e.OldItems);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            RemoveItemsFromCanvas(chart.CanvasView, e.OldItems);
                            AddItemsToCanvas(chart.CanvasView, e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            foreach (var child in chart.CanvasView.Children.OfType<TCollection>().ToArray())
                            {
                                if (child is not View view) continue;
                                _ = chart.CanvasView.Children.Remove(view);
                            }
                            AddItemsToCanvas(chart.CanvasView, s as IEnumerable);
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        default:
                            break;
                    }
                };
            }

            RemoveItemsFromCanvas(chart.CanvasView, oldCollection);
            AddItemsToCanvas(chart.CanvasView, newCollection);

            chart.CoreChart.Update();
        };
    }

    private static void AddItemsToCanvas(MotionCanvas canvas, IEnumerable? items)
    {
        if (items is null) return;

        foreach (var item in items)
        {
            if (item is not View view) continue;
            canvas.Children.Add(view);
        }
    }

    private static void RemoveItemsFromCanvas(MotionCanvas canvas, IEnumerable? items)
    {
        if (items is null) return;

        foreach (var item in items)
        {
            if (item is not View view) continue;
            _ = canvas.Children.Remove(view);
        }
    }
}
