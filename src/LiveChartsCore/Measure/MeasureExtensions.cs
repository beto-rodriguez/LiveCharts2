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
using LiveChartsCore.Kernel;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines some measure extensions.
/// </summary>
public static class MeasureExtensions
{
    /// <summary>
    /// Splits an enumerable of chartpoints by each null gap.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="onDeleteNullPoint">Called when a point was deleted.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ChartPoint>> SplitByNullGaps(
        this IEnumerable<ChartPoint> points,
        Action<ChartPoint> onDeleteNullPoint)
    {
        using var builder = new GapsBuilder(points.GetEnumerator());
        while (!builder.Finished) yield return YieldReturnUntilNextNullChartPoint(builder, onDeleteNullPoint);
    }

    /// <summary>
    /// Builds a anumerator with the necessary data to build an Spline.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static IEnumerable<SplineData> AsSplineData(this IEnumerable<ChartPoint> source)
    {
        using var e = source.GetEnumerator();

        if (!e.MoveNext()) yield break;
        var data = new SplineData(e.Current);

        if (!e.MoveNext())
        {
            yield return data;
            yield break;
        }

        data.GoNext(e.Current);

        while (e.MoveNext())
        {
            yield return data;
            data.IsFirst = false;
            data.GoNext(e.Current);
        }

        data.IsFirst = false;
        yield return data;

        data.GoNext(data.Next);
        yield return data;
    }

    private static IEnumerable<ChartPoint> YieldReturnUntilNextNullChartPoint(
        GapsBuilder builder,
        Action<ChartPoint> onDeleteNullPoint)
    {
        while (builder.Enumerator.MoveNext())
        {
            if (builder.Enumerator.Current.Coordinate.IsEmpty || builder.Enumerator.Current.IsNull)
            {
                var wasEmpty = builder.IsEmpty;
                builder.IsEmpty = true;
                onDeleteNullPoint(builder.Enumerator.Current);
                if (!wasEmpty) yield break; // if there are no points then do not return an empty enumerable...
            }
            else
            {
                yield return builder.Enumerator.Current;
                builder.IsEmpty = false;
            }
        }

        builder.Finished = true;
    }

    private class GapsBuilder : IDisposable
    {
        public GapsBuilder(IEnumerator<ChartPoint> enumerator)
        {
            Enumerator = enumerator;
        }

        public IEnumerator<ChartPoint> Enumerator { get; }

        public bool IsEmpty { get; set; } = true;

        public bool Finished { get; set; } = false;

        public void Dispose()
        {
            Enumerator.Dispose();
        }
    }

    internal class SplineData
    {
        public SplineData(ChartPoint start)
        {
            Previous = start;
            Current = start;
            Next = start;
            AfterNext = start;
        }

        public ChartPoint Previous { get; set; }

        public ChartPoint Current { get; set; }

        public ChartPoint Next { get; set; }

        public ChartPoint AfterNext { get; set; }

        public bool IsFirst { get; set; } = true;

        public void GoNext(ChartPoint point)
        {
            Previous = Current;
            Current = Next;
            Next = AfterNext;
            AfterNext = point;
        }
    }
}
