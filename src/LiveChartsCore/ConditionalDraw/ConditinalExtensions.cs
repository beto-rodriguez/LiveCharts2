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
/// Defines some conditional drawing extensions, things will draw according to this conditions...
/// </summary>
public static class ConditionalDrawExtensions
{
    /// <summary>
    /// Executes the given action when a point is measured, this metod just subscribes to the
    /// <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}.PointMeasured"/> event, but with a simple syntax.
    /// </summary>
    /// <typeparam name="TModel">TThe type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="series">The target series.</param>
    /// <param name="predicate">The action to execute.</param>
    /// <returns>The series.</returns>
    /// <remarks>
    /// The action is subscribed to the <see cref="Series{TModel, TVisual, TLabel, TDrawingContext}.PointMeasured"/> event.
    /// </remarks>
    public static Series<TModel, TVisual, TLabel, TDrawingContext> OnPointMeasured<TModel, TVisual, TLabel, TDrawingContext>(
        this Series<TModel, TVisual, TLabel, TDrawingContext> series, Action<ChartPoint<TModel, TVisual, TLabel>> predicate)
            where TDrawingContext : DrawingContext
            where TVisual : class, IGeometry<TDrawingContext>, new()
            where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        series.PointMeasured += predicate;
        return series;
    }

    /// <summary>
    /// Returns a <see cref="ConditionalPaintBuilder{TModel, TVisual, TLabel, TDrawingContext}"/> for the given paint.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="series">The series.</param>
    /// <param name="paint">The paint.</param>
    /// <returns></returns>
    [Obsolete($"Use {nameof(OnPointMeasured)} instead.")]
    public static ConditionalPaintBuilder<TModel, TVisual, TLabel, TDrawingContext> WithConditionalPaint<TModel, TVisual, TLabel, TDrawingContext>(
        this Series<TModel, TVisual, TLabel, TDrawingContext> series, IPaint<TDrawingContext> paint)
            where TDrawingContext : DrawingContext
            where TVisual : class, IGeometry<TDrawingContext>, new()
            where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        return new ConditionalPaintBuilder<TModel, TVisual, TLabel, TDrawingContext>(series, paint);
    }
}
