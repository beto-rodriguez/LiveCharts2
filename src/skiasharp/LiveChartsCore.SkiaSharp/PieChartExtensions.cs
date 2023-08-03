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

// Ignore Spelling: Skia Lvc

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the pie chart esxtensions.
/// </summary>
public static class PieChartExtensions
{
    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <param name="gaugeOptions">Defines whether the series are treaded as gauge.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel, DoughnutGeometry, LabelGeometry>> AsPieSeries<TModel>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel, DoughnutGeometry, LabelGeometry>>? builder = null,
        GaugeOptions gaugeOptions = GaugeOptions.None)
    {
        return AsPieSeries<TModel, DoughnutGeometry, LabelGeometry>(source, builder, gaugeOptions);
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <param name="gaugeOptions">Defines whether the series are treaded as gauge.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel, TVisual, LabelGeometry>> AsPieSeries<TModel, TVisual>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel, TVisual, LabelGeometry>>? builder = null,
        GaugeOptions gaugeOptions = GaugeOptions.None)
            where TVisual : class, IDoughnutGeometry<SkiaSharpDrawingContext>, new()
    {
        return AsPieSeries<TModel, TVisual, LabelGeometry>(source, builder, gaugeOptions);
    }

    /// <summary>
    /// Converts an IEnumerable to an ObservableCollection of pie series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <param name="gaugeOptions">Defines whether the series are treaded as gauge.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<TModel, TVisual, TLabel>> AsPieSeries<TModel, TVisual, TLabel>(
        this IEnumerable<TModel> source,
        Action<TModel, PieSeries<TModel, TVisual, TLabel>>? builder = null,
        GaugeOptions gaugeOptions = GaugeOptions.None)
            where TVisual : class, IDoughnutGeometry<SkiaSharpDrawingContext>, new()
            where TLabel : class, ILabelGeometry<SkiaSharpDrawingContext>, new()
    {
        var count = source.Count();
        var isGauge = gaugeOptions > 0;
        builder ??= (instance, series) => { };
        var i = 0;

        return new ObservableCollection<PieSeries<TModel, TVisual, TLabel>>(
            source.Select(instance =>
            {
                var series = new PieSeries<TModel, TVisual, TLabel>(isGauge);

                ObservableCollection<TModel> values;
                if (gaugeOptions == GaugeOptions.Gauge)
                {
                    values = new ObservableCollection<TModel>();
                    while (values.Count < count - 1)
                    {
                        values.Add(default!);
                    }
                    values.Insert(i, instance);
                }
                else
                {
                    values = new ObservableCollection<TModel> { instance };
                }

                series.Values = values;

                if (isGauge) series.HoverPushout = 0;
                builder(instance, series);

                i++;

                return series;
            })
            .ToArray());
    }

    /// <summary>
    /// Converts a collection of int to an ObservableCollection of pie series, formatter to build a gauge.
    /// </summary>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>> AsGauge(
        this IEnumerable<int> source,
        Action<ObservableValue, PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>>? builder = null)
    {
        return AsGauge(source.Select(x => new ObservableValue(x)), builder);
    }

    /// <summary>
    /// Converts a collection of float to an ObservableCollection of pie series, formatter to build a gauge.
    /// </summary>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>> AsGauge(
        this IEnumerable<float> source,
        Action<ObservableValue, PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>>? builder = null)
    {
        return AsGauge(source.Select(x => new ObservableValue(x)), builder);
    }

    /// <summary>
    /// Converts a collection of decimal to an ObservableCollection of pie series, formatter to build a gauge.
    /// </summary>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>> AsGauge(
        this IEnumerable<decimal> source,
        Action<ObservableValue, PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>>? builder = null)
    {
        return AsGauge(source.Select(x => new ObservableValue((double)x)), builder);
    }

    /// <summary>
    /// Converts a collection of double to an ObservableCollection of pie series, formatter to build a gauge.
    /// </summary>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>> AsGauge(
        this IEnumerable<double> source,
        Action<ObservableValue, PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>>? builder = null)
    {
        return AsGauge(source.Select(x => new ObservableValue(x)), builder);
    }

    /// <summary>
    /// Converts an ObservableValue collection to an ObservableCollection of pie series, formatter to build a gauge.
    /// </summary>
    /// <param name="source">The data source.</param>
    /// <param name="builder">An optional builder.</param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>> AsGauge(
        this IEnumerable<ObservableValue> source,
        Action<ObservableValue, PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>>? builder = null)
    {
        var series = AsPieSeries(source, builder, GaugeOptions.Gauge);

        series.Add(new PieSeries<ObservableValue, DoughnutGeometry, LabelGeometry>(true, true)
        {
            ZIndex = -1,
            IsFillSeries = true,
            Values = new ObservableValue[] { new(0) }
        });

        return series;
    }

    /// <summary>
    /// Defines the gauge options.
    /// </summary>
    public enum GaugeOptions
    {
        /// <summary>
        /// Not a gauge.
        /// </summary>
        None,
        /// <summary>
        /// A gauge.
        /// </summary>
        Gauge,
        /// <summary>
        /// An angular gauge.
        /// </summary>
        AngularGauge
    }
}
