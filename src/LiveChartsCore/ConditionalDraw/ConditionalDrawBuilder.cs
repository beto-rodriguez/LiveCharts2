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
/// Defines a <see cref="ConditionalDrawBuilder{TModel, TVisual, TLabel, TDrawingContext}"/> instance.
/// </summary>
public class ConditionalDrawBuilder<TModel, TVisual, TLabel, TDrawingContext>
    where TDrawingContext : DrawingContext
    where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private bool _isPaintInCanvas = false;
    private readonly Series<TModel, TVisual, TLabel, TDrawingContext> _series;
    private readonly IPaint<TDrawingContext> _paint;
    private Func<ChartPoint<TModel, TVisual, TLabel>, bool>? _whenPredicate;

    /// <summary>
    /// Initializes a new builder for the given paint.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="paint">The paint.</param>
    public ConditionalDrawBuilder(Series<TModel, TVisual, TLabel, TDrawingContext> series, IPaint<TDrawingContext> paint)
    {
        _series = series;
        _paint = paint;

        // let's just make things work magically
        // just set an 'above all' z-index.
        if (paint.ZIndex == 0) paint.ZIndex = ((ISeries)series).SeriesId + 1050;
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
    public void Unsubscribe()
    {
        _series.PointMeasured -= OnMeasured;
    }

    private void OnMeasured(ChartPoint<TModel, TVisual, TLabel> point)
    {
        if (_whenPredicate is null || point.Visual is null) return;

        var isTriggered = _whenPredicate.Invoke(point);
        var canvas = ((Chart<TDrawingContext>)point.Context.Chart.CoreChart).Canvas;
        var drawable = (IDrawable<TDrawingContext>)point.Visual; // see note #20221909

        if (!_isPaintInCanvas)
        {
            canvas.AddDrawableTask(_paint);
            _isPaintInCanvas = true;
        }

        if (isTriggered)
        {
            _paint.AddGeometryToPaintTask(canvas, drawable);
        }
        else
        {
            _paint.RemoveGeometryFromPainTask(canvas, drawable);
        }
    }
}
