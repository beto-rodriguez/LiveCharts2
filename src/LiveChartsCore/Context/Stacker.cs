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
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public class Stacker<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private int stackCount = 0;
        private int stackMaxLength = 0;
        private Dictionary<IDataSeries<TDrawingContext>, int> stackPositions = new Dictionary<IDataSeries<TDrawingContext>, int>();
        private int knownMaxLenght = 0;
        private List<List<StackedValue>> stack = new List<List<StackedValue>>();
        private List<float> totals = new List<float>();

        public Stacker()
        {
        }

        public int MaxLenght => stackMaxLength;

        public int GetSeriesStackPosition(IDataSeries<TDrawingContext> series)
        {
            if (!stackPositions.TryGetValue(series, out var i))
            {
                var n = new List<StackedValue>(knownMaxLenght);
                stack.Add(n);
                i = stackCount++;
                stackPositions[series] = i;
            }

            return i;
        }

        public float StackPoint(IOutOfContextChartPoint point, int seriesStackPosition)
        {
            var index = unchecked((int)point.SecondaryValue);
            var start = seriesStackPosition == 0 ? 0 : stack[seriesStackPosition - 1][index].End;
            var value = point.PrimaryValue;

            var si = stack[seriesStackPosition];
            if (si.Count < point.SecondaryValue + 1)
            {
                si.Add(new StackedValue { Start = start, End = start + value });
                totals.Add(value);
                knownMaxLenght++;
            }
            else
            {
                si[index] = new StackedValue { Start = start, End = start + value };
            }

            var total = totals[index] + value;
            totals[index] = total;

            return total;
        }

        public StackedValue GetStack(int seriesStackPosition, IOutOfContextChartPoint point)
        {
            var index = unchecked((int)point.SecondaryValue);
            var p = stack[seriesStackPosition][index];

            return new StackedValue
            {
                Start = p.Start,
                End = p.End
            };
        }
    }
}
