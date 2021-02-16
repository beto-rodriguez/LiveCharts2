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
        private Dictionary<ISeries<TDrawingContext>, int> stackPositions = new Dictionary<ISeries<TDrawingContext>, int>();
        private int knownMaxLenght = 0;
        private List<List<StackedValue>> stack = new List<List<StackedValue>>();
        private List<float> totals = new List<float>();
        private SeriesDirection direction;

        public int MaxLenght => stackMaxLength;

        public SeriesDirection Orientation => direction;

        public Stacker(SeriesDirection direction)
        {
            this.direction = direction;
        }

        public int GetSeriesStackPosition(ISeries<TDrawingContext> series)
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

        public float StackPoint(ICartesianCoordinate point, int seriesStackPosition)
        {
            var start = seriesStackPosition == 0 ? 0 : stack[seriesStackPosition - 1][point.Index].End;
            var value = direction == SeriesDirection.Horizontal ? point.X : point.Y;

            var si = stack[seriesStackPosition];
            if (si.Count < point.Index + 1)
            {
                si.Add(new StackedValue { Start = start, End = start + value });
                totals.Add(value);
                knownMaxLenght++;
            }
            else
            {
                si[point.Index] = new StackedValue { Start = start, End = start + value };
            }

            var total = totals[point.Index] + value;
            totals[point.Index] = total;

            return total;
        }

        public StackedValue GetStack(int seriesStackPosition, ICartesianCoordinate point)
        {
            var p = stack[seriesStackPosition][point.Index];

            return new StackedValue
            {
                Start = p.Start,
                End = p.End
            };
        }
    }
}
