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

namespace LiveChartsCore.Kernel
{
    public interface IDataFactoryProvider<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        public DataProvider<TModel, TDrawingContext> GetProvider<TModel>();
    }

    //public class DataProvider<TModel, TVisual, TLabel, TDrawingContext> : IDataProvider
    //    where TDrawingContext : DrawingContext
    //    where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    //    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    //{
    //    public static Func<IChart, ISeries, ChartPoint[], ChartPoint[]> factoryHandler = (chart, series, points) => points;
    //    internal readonly Dictionary<int, ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>> byValueVisualMap = new();
    //    internal readonly Dictionary<TModel, ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>> byReferenceVisualMap = new();
    //    private object fetchedFor = new();
    //    private ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>[] fetched = new ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>[0];

    //    public DataProvider()
    //    {
    //        factoryHandler = (chart, series, points) =>
    //        {
    //            var sauce = points.ToArray();
    //            var cs = sauce.Length;

    //            var size = chart.View.ControlSize;
    //            var m = size.Width > size.Height ? size.Width : size.Height;

    //            var q = m * 1; // 2 means: how many points per pixel (aprox);
    //            var f = cs / q;

    //            var r = Simplify(sauce, f).ToArray();
    //            var c = r.Length;

    //            return r;
    //        };
    //    }

    //    public IEnumerable<ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>> Fetch(
    //        Series<TModel, TVisual, TLabel, TDrawingContext> series, IChart chart)
    //    {
    //        //if (fetchedFor == chart.MeasureWorker) return fetched;

    //        fetchedFor = chart.MeasureWorker;
    //        var p = GetMappedPoints(series, chart).ToArray();
    //        var h = factoryHandler(chart, series, p);

    //        return fetched = h.Cast<ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>>().ToArray();
    //    }

    //    DimensinalBounds? b = null;
    //    public DimensinalBounds GetCartesianBounds(
    //        CartesianChart<TDrawingContext> chart, IDrawableSeries<TDrawingContext> series, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
    //    {
    //        if (b != null) return b;

    //        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

    //        var bounds = new DimensinalBounds();
    //        foreach (var point in series.Fetch(chart))
    //        {
    //            var primary = point.PrimaryValue;
    //            var secondary = point.SecondaryValue;

    //            // it has more sense to override this method and call the stack, only if the series requires so.
    //            if (stack != null) primary = stack.StackPoint(point);

    //            bounds.PrimaryBounds.AppendValue(primary);
    //            bounds.SecondaryBounds.AppendValue(secondary);
    //        }

    //        b = bounds;
    //        return bounds;
    //    }

    //    public DimensinalBounds GetPieBounds(PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
    //    {
    //        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());
    //        if (stack == null) throw new NullReferenceException("Unexpected null stacker");

    //        var bounds = new DimensinalBounds();
    //        foreach (var point in series.Fetch(chart))
    //        {
    //            stack.StackPoint(point);
    //            bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
    //            bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
    //            bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
    //        }

    //        return bounds;
    //    }

    //    private IEnumerable<ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>> GetMappedPoints(
    //        Series<TModel, TVisual, TLabel, TDrawingContext> series, IChart chart)
    //    {
    //        if (series.Values == null) yield break;

    //        var mapper = series.Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
    //        var index = 0;

    //        if (series.IsValueType)
    //        {
    //            foreach (var item in series.Values)
    //            {
    //                if (!byValueVisualMap.TryGetValue(index, out var cp))
    //                    byValueVisualMap[index] =
    //                        cp = new ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>(chart.View, series);

    //                cp.Context.Index = index++;
    //                cp.Context.DataSource = item;
    //                mapper(item, cp);

    //                yield return cp;
    //            }
    //        }
    //        else
    //        {
    //            foreach (var item in series.Values)
    //            {
    //                if (!byReferenceVisualMap.TryGetValue(item, out var cp))
    //                    byReferenceVisualMap[item] =
    //                        cp = new ChartVisualPoint<TModel, TVisual, TLabel, TDrawingContext>(chart.View, series);

    //                cp.Context.Index = index++;
    //                cp.Context.DataSource = item;
    //                mapper(item, cp);

    //                yield return cp;
    //            }
    //        }
    //    }

    //    #region mourner simplify 

    //    private double GetSquareDistance(ChartPoint p1, ChartPoint p2)
    //    {
    //        var dx = p1.SecondaryValue - p2.SecondaryValue;
    //        var dy = p1.PrimaryValue - p2.PrimaryValue;

    //        return (dx * dx) + (dy * dy);
    //    }

    //    // square distance from a point to a segment
    //    private double GetSquareSegmentDistance(ChartPoint p, ChartPoint p1, ChartPoint p2)
    //    {
    //        var x = p1.SecondaryValue;
    //        var y = p1.PrimaryValue;
    //        var dx = p2.SecondaryValue - x;
    //        var dy = p2.PrimaryValue - y;

    //        if (!dx.Equals(0.0) || !dy.Equals(0.0))
    //        {
    //            var t = ((p.SecondaryValue - x) * dx + (p.PrimaryValue - y) * dy) / (dx * dx + dy * dy);

    //            if (t > 1)
    //            {
    //                x = p2.SecondaryValue;
    //                y = p2.PrimaryValue;
    //            }
    //            else if (t > 0)
    //            {
    //                x += dx * t;
    //                y += dy * t;
    //            }
    //        }

    //        dx = p.SecondaryValue - x;
    //        dy = p.PrimaryValue - y;

    //        return (dx * dx) + (dy * dy);
    //    }

    //    // rest of the code doesn't care about point format

    //    // basic distance-based simplification
    //    private List<ChartPoint> SimplifyRadialDistance(ChartPoint[] points, double sqTolerance)
    //    {
    //        var prevPoint = points[0];
    //        var newPoints = new List<ChartPoint> { prevPoint };
    //        ChartPoint point = null;

    //        for (var i = 1; i < points.Length; i++)
    //        {
    //            point = points[i];

    //            if (GetSquareDistance(point, prevPoint) > sqTolerance)
    //            {
    //                newPoints.Add(point);
    //                prevPoint = point;
    //            }
    //        }

    //        if (point != null && !prevPoint.Equals(point))
    //            newPoints.Add(point);

    //        return newPoints;
    //    }

    //    // simplification using optimized Douglas-Peucker algorithm with recursion elimination
    //    private List<ChartPoint> SimplifyDouglasPeucker(ChartPoint[] points, double sqTolerance)
    //    {
    //        var len = points.Length;
    //        var markers = new int?[len];
    //        int? first = 0;
    //        int? last = len - 1;
    //        int? index = 0;
    //        var stack = new List<int?>();
    //        var newPoints = new List<ChartPoint>();

    //        markers[first.Value] = markers[last.Value] = 1;

    //        while (last != null)
    //        {
    //            var maxSqDist = 0.0d;

    //            for (int? i = first + 1; i < last; i++)
    //            {
    //                var sqDist = GetSquareSegmentDistance(points[i.Value], points[first.Value], points[last.Value]);

    //                if (sqDist > maxSqDist)
    //                {
    //                    index = i;
    //                    maxSqDist = sqDist;
    //                }
    //            }

    //            if (maxSqDist > sqTolerance)
    //            {
    //                markers[index.Value] = 1;
    //                stack.AddRange(new[] { first, index, index, last });
    //            }


    //            if (stack.Count > 0)
    //            {
    //                last = stack[stack.Count - 1];
    //                stack.RemoveAt(stack.Count - 1);
    //            }
    //            else
    //                last = null;

    //            if (stack.Count > 0)
    //            {
    //                first = stack[stack.Count - 1];
    //                stack.RemoveAt(stack.Count - 1);
    //            }
    //            else
    //                first = null;
    //        }

    //        for (var i = 0; i < len; i++)
    //        {
    //            if (markers[i] != null)
    //                newPoints.Add(points[i]);
    //        }

    //        return newPoints;
    //    }

    //    /// <summary>
    //    /// Simplifies a list of points to a shorter list of points.
    //    /// </summary>
    //    /// <param name="points">Points original list of points</param>
    //    /// <param name="tolerance">Tolerance tolerance in the same measurement as the point coordinates</param>
    //    /// <param name="highestQuality">Enable highest quality for using Douglas-Peucker, set false for Radial-Distance algorithm</param>
    //    /// <returns>Simplified list of points</returns>
    //    public List<ChartPoint> Simplify(
    //        ChartPoint[] points, double tolerance = 0.3, bool highestQuality = false)
    //    {
    //        if (points == null || points.Length == 0)
    //            return new List<ChartPoint>();

    //        var sqTolerance = tolerance * tolerance;

    //        if (highestQuality)
    //            return SimplifyDouglasPeucker(points, sqTolerance);

    //        List<ChartPoint> points2 = SimplifyRadialDistance(points, sqTolerance);
    //        return SimplifyDouglasPeucker(points2.ToArray(), sqTolerance);
    //    }

    //    #endregion
    //}
}
