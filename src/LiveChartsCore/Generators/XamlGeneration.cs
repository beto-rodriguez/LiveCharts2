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

#pragma warning disable IDE0060 // Remove unused parameter

using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Providers;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Generators;

internal class XamlGeneration
{
    public static void OnAxisIntervalChanged(
        IXamlWrapper<ICartesianAxis> axis, TimeSpan oldValue, TimeSpan newValue)
    {
        axis.WrappedObject.UnitWidth = newValue.Ticks;
        axis.WrappedObject.MinStep = newValue.Ticks;
    }

    public static void OnDateTimeAxisDateFormatterChanged(

        IXamlWrapper<ICartesianAxis> axis, Func<DateTime, string> oldValue, Func<DateTime, string> newValue) =>
            axis.WrappedObject.Labeler = value => newValue(value.AsDate());

    public static void OnTimeSpanAxisFormatterChanged(
        IXamlWrapper<ICartesianAxis> axis, Func<TimeSpan, string> oldValue, Func<TimeSpan, string> newValue) =>
            axis.WrappedObject.Labeler = value => newValue(value.AsTimeSpan());

    public static void OnAxisLogBaseChanged(
        IXamlWrapper<ICartesianAxis> axis, double oldValue, double newValue) =>
            axis.WrappedObject.SetLogBase(newValue);

    public static void SetupGaugeSeries(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
    {
        var pieSeries = series.WrappedObject;

        pieSeries.Values = new ObservableCollection<ObservableValue> { value };
        var baseSeries = (IInternalSeries)pieSeries;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge;

        dataFactory.ValuesTransform = (chart, values) =>
        {
            var seriesArray = ((PieChartEngine)chart).Series
                .Where(x => x is IPieSeries pie && !pie.IsFillSeries)
                .ToArray();

            var index = 0;
            foreach (var series in seriesArray)
            {
                if (series == pieSeries) break;
                index++;
            }

            var transformedValues = new ObservableValue?[seriesArray.Length];

            for (var i = 0; i < seriesArray.Length; i++)
            {
                transformedValues[i] = i == index
                    ? value
                    : null;
            }

            return transformedValues;
        };
    }

    public static void SetupBackgroundGaugeSeries(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
    {
        var pieSeries = series.WrappedObject;

        pieSeries.Values = new ObservableCollection<ObservableValue> { value };
        pieSeries.ZIndex = -1;
        pieSeries.IsFillSeries = true;
        pieSeries.IsVisibleAtLegend = false;

        var baseSeries = (IInternalSeries)pieSeries;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge | SeriesProperties.GaugeFill;

        dataFactory.ValuesTransform = (chart, values) =>
        {
            var seriesArray = ((PieChartEngine)chart).Series
               .Where(x => x is IPieSeries pie && !pie.IsFillSeries)
               .ToArray();

            var transformedValues = new ObservableValue?[seriesArray.Length];

            for (var i = 0; i < seriesArray.Length; i++)
                transformedValues[i] = new(0);

            return transformedValues;
        };
    }

    public static void SetupAngularGaugeSeries(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
    {
        var pieSeries = series.WrappedObject;

        pieSeries.Values = new ObservableCollection<ObservableValue> { value };
        pieSeries.HoverPushout = 0;
        pieSeries.IsHoverable = false;
        pieSeries.DataLabelsPaint = null;
        pieSeries.AnimationsSpeed = TimeSpan.FromSeconds(0);
        pieSeries.IsRelativeToMinValue = true;

        var baseSeries = (IInternalSeries)pieSeries;
        baseSeries.SeriesProperties |= SeriesProperties.Gauge;

        dataFactory.ValuesTransform = (chart, values) => values;
    }
}
