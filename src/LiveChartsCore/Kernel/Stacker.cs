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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines the stacker helper class.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class Stacker<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private readonly Dictionary<IChartSeries<TDrawingContext>, int> _stackPositions = new();
    private readonly List<Dictionary<double, StackedValue>> _stack = new();
    private readonly Dictionary<double, StackedTotal> _totals = new();
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
        var coordinate = point.Coordinate;

        var index = coordinate.SecondaryValue;
        var value = coordinate.PrimaryValue;
        var positiveStart = 0d;
        var negativeStart = 0d;

        if (seriesStackPosition > 0)
        {
            var ssp = seriesStackPosition;
            var found = false;

            // keep diging until you find a stack in the same position.
            while (ssp >= 0 && !found && ssp - 1 >= 0)
            {
                var stackCol = _stack[ssp - 1];
                if (stackCol.TryGetValue(index, out var previousActiveStack))
                {
                    positiveStart = previousActiveStack.End;
                    negativeStart = previousActiveStack.NegativeEnd;
                    found = true;
                }
                else
                {
                    ssp--;
                }
            }
        }

        var si = _stack[seriesStackPosition];

        if (!si.TryGetValue(coordinate.SecondaryValue, out var currentStack))
        {
            currentStack = new StackedValue
            {
                Start = positiveStart,
                End = positiveStart,
                NegativeStart = negativeStart,
                NegativeEnd = negativeStart
            };
            si.Add(index, currentStack);
            if (!_totals.TryGetValue(index, out var _)) _totals.Add(index, new());
            _knownMaxLenght++;
        }

        if (value >= 0)
        {
            currentStack.End += value;
            var positiveTotal = _totals[index].Positive + value;
            _totals[index].Positive = positiveTotal;

            return positiveTotal;
        }
        else
        {
            currentStack.NegativeEnd += value;
            var negativeTotal = _totals[index].Negative + value;
            _totals[index].Negative = negativeTotal;

            return negativeTotal;
        }
    }

    /// <summary>
    /// Gets the stack.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="seriesStackPosition">The series stack position.</param>
    /// <returns></returns>
    public StackedValue GetStack(ChartPoint point, int seriesStackPosition)
    {
        var index = point.Coordinate.SecondaryValue;
        var p = _stack[seriesStackPosition][index];

        return new StackedValue
        {
            Start = p.Start,
            End = p.End,
            Total = _totals[index].Positive,
            NegativeStart = p.NegativeStart,
            NegativeEnd = p.NegativeEnd,
            NegativeTotal = _totals[index].Negative
        };
    }
}
