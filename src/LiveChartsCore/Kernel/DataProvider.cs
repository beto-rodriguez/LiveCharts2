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

using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a data provider.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TDrawingContext"></typeparam>
    public class DataProvider<TModel, TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly Dictionary<int, ChartPoint> _byValueVisualMap = new();
        private readonly Dictionary<TModel, ChartPoint> _byReferenceVisualMap = new();

        /// <summary>
        /// Fetches the the points for the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
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
                    if (!_byValueVisualMap.TryGetValue(index, out var cp))
                        _byValueVisualMap[index] = cp = new ChartPoint(chart.View, series);

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
                    if (!_byReferenceVisualMap.TryGetValue(item, out var cp))
                        _byReferenceVisualMap[item] = cp = new ChartPoint(chart.View, series);

                    cp.Context.Index = index++;
                    cp.Context.DataSource = item;
                    mapper(item, cp);

                    yield return cp;
                }
            }
        }

        /// <summary>
        /// Gets the cartesian bounds.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="series">The series.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public virtual DimensionalBounds GetCartesianBounds(
            CartesianChart<TDrawingContext> chart,
            IDrawableSeries<TDrawingContext> series,
            IAxis<TDrawingContext> x,
            IAxis<TDrawingContext> y)
        {
            var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

            var xMin = x.MinLimit ?? float.MinValue;
            var xMax = x.MaxLimit ?? float.MaxValue;
            var yMin = y.MinLimit ?? float.MinValue;
            var yMax = y.MaxLimit ?? float.MaxValue;

            var bounds = new DimensionalBounds();
            foreach (var point in series.Fetch(chart))
            {
                var primary = point.PrimaryValue;
                var secondary = point.SecondaryValue;
                var tertiary = point.TertiaryValue;

                if (stack != null) primary = stack.StackPoint(point);

                _ = bounds.PrimaryBounds.AppendValue(primary);
                _ = bounds.SecondaryBounds.AppendValue(secondary);
                _ = bounds.TertiaryBounds.AppendValue(tertiary);

                if (primary >= yMin && primary <= yMax && secondary >= xMin && secondary <= xMax)
                {
                    _ = bounds.VisiblePrimaryBounds.AppendValue(primary);
                    _ = bounds.VisibleSecondaryBounds.AppendValue(secondary);
                    _ = bounds.VisibleTertiaryBounds.AppendValue(tertiary);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Gets the pie bounds.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Unexpected null stacker</exception>
        public virtual DimensionalBounds GetPieBounds(PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
        {
            var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());
            if (stack == null) throw new NullReferenceException("Unexpected null stacker");

            var bounds = new DimensionalBounds();
            foreach (var point in series.Fetch(chart))
            {
                _ = stack.StackPoint(point);
                _ = bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
                _ = bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
                _ = bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
            }

            return bounds;
        }

        /// <summary>
        /// Clears the visuals in the cache.
        /// </summary>
        /// <returns></returns>
        public virtual void RestartVisuals()
        {
            foreach (var item in _byReferenceVisualMap)
            {
                if (item.Value.Context.Visual is not IAnimatable visual) continue;
                visual.RemoveTransitions();
            }
            _byReferenceVisualMap.Clear();

            foreach (var item in _byValueVisualMap)
            {
                if (item.Value.Context.Visual is not IAnimatable visual) continue;
                visual.RemoveTransitions();
            }
            _byValueVisualMap.Clear();
        }
    }
}
