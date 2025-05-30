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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// Base class for views that display a chart.
/// </summary>
public abstract class ChartView : ContentView
{
    static ChartView()
    {
        if (!LiveChartsCoreMauiAppBuilderExtensions.AreHandlersRegistered)
        {
            throw new InvalidOperationException(
                "Since rc5 version, `.UseLiveCharts()` and `.UseSkiaSharp()` must be " +
                "chained to `.UseMauiApp<T>()`, in the MauiProgram.cs file. For more info see:" +
                "https://livecharts.dev/docs/Maui/2.0.0-rc5/Overview.Installation");
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartView"/> class.
    /// </summary>
    protected ChartView()
    {
        Content = new MotionCanvas();
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content;

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public abstract Chart CoreChart { get; }

    /// <summary>
    /// The series items source property
    /// </summary>
    public static readonly BindableProperty SeriesSourceProperty =
          BindableProperty.Create(
              nameof(SeriesSource), typeof(IEnumerable<object>), typeof(PieChart), null, BindingMode.Default, null,
              OnSeriesSourceChanged);

    /// <summary>
    /// The series template property
    /// </summary>
    public static readonly BindableProperty SeriesTemplateProperty =
          BindableProperty.Create(
              nameof(SeriesTemplate), typeof(DataTemplate), typeof(PieChart), null, BindingMode.Default, null,
              OnSeriesSourceChanged);

    /// <summary>
    /// Gets or sets the Series items source.
    /// </summary>
    public IEnumerable<object> SeriesSource
    {
        get => (IEnumerable<object>)GetValue(SeriesSourceProperty);
        set => SetValue(SeriesSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the series template.
    /// </summary>
    public DataTemplate SeriesTemplate
    {
        get => (DataTemplate)GetValue(SeriesTemplateProperty);
        set => SetValue(SeriesTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    public abstract ICollection<ISeries> Series { get; set; }

    internal virtual void OnPressed(object? sender, Behaviours.Events.PressedEventArgs args) { }
    internal virtual void OnMoved(object? sender, Behaviours.Events.ScreenEventArgs args) { }
    internal virtual void OnReleased(object? sender, Behaviours.Events.PressedEventArgs args) { }
    internal virtual void OnScrolled(object? sender, Behaviours.Events.ScrollEventArgs args) { }
    internal virtual void OnPinched(object? sender, Behaviours.Events.PinchEventArgs args) { }
    internal virtual void OnExited(object? sender, Behaviours.Events.EventArgs args) { }

    private static void OnSeriesSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chart = (ChartView)bindable;

        if (oldValue is INotifyCollectionChanged oldIncc)
            oldIncc.CollectionChanged -= chart.OnSeriesItemsSourceCollectionChanged;

        if (chart.SeriesSource is null || chart.SeriesTemplate is null) return;

        chart.InitializeSeriesSource();
    }

    private void InitializeSeriesSource()
    {
        if (SeriesSource is null || SeriesTemplate is null) return;

        var inflatedSeries = SeriesSource
            .Select(InflateSeriesTemplate)
            .ToArray();

        if (SeriesSource is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged += OnSeriesItemsSourceCollectionChanged;
            var observableSeries = new ObservableCollection<ISeries>(inflatedSeries);
            Series = observableSeries;
        }
        else
        {
            Series = inflatedSeries;
        }
    }

    internal void OnSeriesItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var newItem in e.NewItems ?? Array.Empty<object>())
                {
                    var inflatedSeries = InflateSeriesTemplate(newItem);
                    Series.Add(inflatedSeries);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                var seriesHash = Series.ToDictionary(x => ((View)x).BindingContext, x => x);
                foreach (var oldItem in e.OldItems ?? Array.Empty<object>())
                {
                    if (!seriesHash.TryGetValue(oldItem, out var seriesObject)) continue;
                    _ = Series.Remove(seriesObject);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                InitializeSeriesSource();
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            default:
                break;
        }
    }

    private ISeries InflateSeriesTemplate(object item)
    {
        if (SeriesTemplate.CreateContent() is not View template)
            throw new InvalidOperationException("The template must be a View.");
        if (template is not ISeries series)
            throw new InvalidOperationException("The template is not a valid series.");

        template.BindingContext = item;

        return series;
    }
}
