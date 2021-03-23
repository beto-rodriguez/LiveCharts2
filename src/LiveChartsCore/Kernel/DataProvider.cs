// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.Kernel
{
    public class DataProvider<TModel, TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        protected readonly Dictionary<int, ChartPoint> byValueVisualMap = new();
        protected readonly Dictionary<TModel, ChartPoint> byReferenceVisualMap = new();

        public virtual IEnumerable<ChartPoint> Fetch(ISeries<TModel> series, IChart chart)
        {
            if (series.Values == null) yield break;

            var t = typeof(TModel);
            var isValueType = t.IsValueType;

            var mapper = series.Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
            var index = 0;

            if (isValueType)
            {
                foreach (var item in series.Values)
                {
                    if (!byValueVisualMap.TryGetValue(index, out var cp))
                        byValueVisualMap[index] = cp = new ChartPoint(chart.View, series);

                    cp.Context.Index = index++;
                    cp.Context.DataSource = item;

                    mapper(item, cp);

                    yield return cp;
                }
            }
            else
            {
                foreach (var item in series.Values)
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out var cp))
                        byReferenceVisualMap[item] = cp = new ChartPoint(chart.View, series);

                    cp.Context.Index = index++;
                    cp.Context.DataSource = item;
                    mapper(item, cp);

                    yield return cp;
                }
            }
        }

        public virtual DimensinalBounds GetCartesianBounds(CartesianChart<TDrawingContext> chart, IDrawableSeries<TDrawingContext> series, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

            var bounds = new DimensinalBounds();
            foreach (var point in series.Fetch(chart))
            {
                var primary = point.PrimaryValue;
                var secondary = point.SecondaryValue;

                if (stack != null) primary = stack.StackPoint(point);

                bounds.PrimaryBounds.AppendValue(primary);
                bounds.SecondaryBounds.AppendValue(secondary);
            }

            return bounds;
        }

        public virtual DimensinalBounds GetPieBounds(PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
        {
            var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());
            if (stack == null) throw new NullReferenceException("Unexpected null stacker");

            var bounds = new DimensinalBounds();
            foreach (var point in series.Fetch(chart))
            {
                stack.StackPoint(point);
                bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
                bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
                bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
            }

            return bounds;
        }
    }
}
