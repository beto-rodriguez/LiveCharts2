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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System;

namespace LiveChartsCore.Themes
{
    /// <summary>
    /// Defines the LiveCharts styler extensions.
    /// </summary>
    public static class LiveChartsStylerExtensions
    {
        /// <summary>
        ///  Defines a style builder for <see cref="IChartView{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForCharts<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IChartView<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ChartBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IChartView{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForAxes<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IAxis<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.AxisBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IDrawableSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForAnySeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IDrawableSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.SeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IPieSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForPieSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IPieSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.PieSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="ILineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForLineSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<ILineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.LineSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for tacked <see cref="ILineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForStackedLineSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<ILineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedLineSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForBarSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.BarSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for vertical <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForColumnSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ColumnSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for horizontal <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForRowSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ColumnSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForStackedBarSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedBarSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked vertical <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForStackedColumnSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedColumnSeriesBuilder = predicate;
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked horizontal <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsInitializer<TDrawingContext> ForStackedRowSeries<TDrawingContext>(
            this VisualsInitializer<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedRowSeriesBuilder = predicate;
            return styler;
        }
    }
}
