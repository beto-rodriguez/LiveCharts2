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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;

namespace LiveChartsCore.ConditionalDraw;

/// <summary>
/// Defines a <see cref="ConditionalPaintBuilder{TModel, TVisual, TLabel, TDrawingContext}"/> instance.
/// </summary>
[Obsolete("Replaced by MaPoints extensiion.")]
public class ConditionalPaintBuilder<TModel, TVisual, TLabel, TDrawingContext>
    where TDrawingContext : DrawingContext
    where TVisual : class, IGeometry<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private bool _isPaintInCanvas = false;
    private object _clipFor = new();
    private readonly Series<TModel, TVisual, TLabel, TDrawingContext> _series;
    private readonly IPaint<TDrawingContext> _paint;
    private Func<ChartPoint<TModel, TVisual, TLabel>, bool>? _whenPredicate;

    /// <summary>
    /// Initializes a new builder for the given paint.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="paint">The paint.</param>
    public ConditionalPaintBuilder(Series<TModel, TVisual, TLabel, TDrawingContext> series, IPaint<TDrawingContext> paint)
    {
        _series = series;
        _paint = paint;
    }

    /// <summary>
    /// Applies the paint when the given condition is true.
    /// </summary>
    /// <returns></returns>
    public Series<TModel, TVisual, TLabel, TDrawingContext> When(
        Func<ChartPoint<TModel, TVisual, TLabel>, bool> predicate)
    {
        _whenPredicate = predicate;
        _series.PointMeasured += OnMeasured;
        return _series;
    }

    /// <summary>
    /// Un-subscribes the generated event handlers from the target series.
    /// </summary>
    public void UnSubscribe()
    {
        _series.PointMeasured -= OnMeasured;
    }

    private void OnMeasured(ChartPoint<TModel, TVisual, TLabel> point)
    {
        if (_whenPredicate is null || point.Visual is null) return;

        var isTriggered = _whenPredicate.Invoke(point);
        var canvas = ((Chart<TDrawingContext>)point.Context.Chart.CoreChart).Canvas;
        var drawable = (IDrawable<TDrawingContext>?)point.Visual; // see note #20221909
        if (drawable is null) return;

        if (!_isPaintInCanvas)
        {
            canvas.AddDrawableTask(_paint);
            if (_paint.ZIndex == 0) _paint.ZIndex = int.MaxValue;
            _isPaintInCanvas = true;
        }

        if (point.Context.Chart.CoreChart.MeasureWork != _clipFor)
        {
            _clipFor = point.Context.Chart.CoreChart.MeasureWork;
            if (point.Context.Chart.CoreChart is CartesianChart<TDrawingContext> cartesianChart)
            {
                var drawLocation = cartesianChart.DrawMarginLocation;
                var drawMarginSize = cartesianChart.DrawMarginSize;
                _paint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            }
        }

        if (isTriggered)
        {
            _paint.AddGeometryToPaintTask(canvas, drawable);

            foreach (var paint in _series.GetPaintTasks())
            {
                if (paint is null) continue;
                paint.RemoveGeometryFromPainTask(canvas, drawable);
            }
        }
        else
        {
            _paint.RemoveGeometryFromPainTask(canvas, drawable);
        }
    }
}
