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
using LiveChartsCore.Kernel.Sketches;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines the stacker helper class.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public class Stacker<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly Dictionary<IChartSeries<TDrawingContext>, int> _stackPositions = new();
        private readonly List<Dictionary<double, StackedValue>> _stack = new();
        private readonly Dictionary<double, double> _totals = new();
        private int _stackCount = 0;
        private int _knownMaxLenght = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Stacker{TDrawingContext}"/> class.
        /// </summary>
        public Stacker()
        {
        }

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int MaxLength { get; } = 0;

        /// <summary>
        /// Gets the series stack position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetSeriesStackPosition(IChartSeries<TDrawingContext> series)
        {
            if (!_stackPositions.TryGetValue(series, out var i))
            {
                var n = new Dictionary<double, StackedValue>(_knownMaxLenght);
                _stack.Add(n);
                i = _stackCount++;
                _stackPositions[series] = i;
            }

            return i;
        }

        /// <summary>
        /// Stacks the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="seriesStackPosition">The series stack position.</param>
        /// <returns></returns>
        public double StackPoint(ChartPoint point, int seriesStackPosition)
        {
            var index = point.SecondaryValue;

            double start;

            if (seriesStackPosition == 0)
            {
                start = 0;
            }
            else
            {
                var c = _stack[seriesStackPosition - 1];
                if (c.Count - 1 < index)
                    throw new IndexOutOfRangeException(
                        $"The {nameof(ISeries.Values)} property of all the stacked series of the same kind must have the same length. " +
                        $"Use '0' or 'double.NaN' when you need to skip a point.");
                start = c[index].End;
            }

            var value = point.PrimaryValue;

            var si = _stack[seriesStackPosition];

            if (!si.TryGetValue(point.SecondaryValue, out var currentStack))
            {
                currentStack = new StackedValue { Start = start, End = start };
                si.Add(index, currentStack);
                if (!_totals.TryGetValue(index, out var _)) _totals.Add(index, 0);
                _knownMaxLenght++;
            }

            currentStack.End += value;

            var total = _totals[index] + value;
            _totals[index] = total;

            return total;
        }

        /// <summary>
        /// Gets the stack.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="seriesStackPosition">The series stack position.</param>
        /// <returns></returns>
        public StackedValue GetStack(ChartPoint point, int seriesStackPosition)
        {
            var index = point.SecondaryValue;
            var p = _stack[seriesStackPosition][index];

            return new StackedValue
            {
                Start = p.Start,
                End = p.End,
                Total = _totals[index]
            };
        }
    }
}
