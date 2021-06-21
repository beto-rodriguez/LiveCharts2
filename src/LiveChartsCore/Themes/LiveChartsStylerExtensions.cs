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
using System;
using LiveChartsCore.Kernel.Sketches;

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
        public static VisualsStyle<TDrawingContext> HasRuleForCharts<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IChartView<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ChartBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IChartView{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForAxes<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IAxis<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.AxisBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        /// Defines a style builder for <see cref="DrawMarginFrame{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForDrawMargin<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<DrawMarginFrame<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.DrawMarginFrameBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IChartSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForAnySeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IChartSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.SeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IPieSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForPieSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IPieSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.PieSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IPieSeries{TDrawingContext}"/> objects when used as gauges.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForGaugeSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IPieSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.GaugeSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IPieSeries{TDrawingContext}"/> objects when used as gauges fills.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForGaugeFillSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IPieSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.GaugeFillSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="ILineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForLineSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<ILineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.LineSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IStepLineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStepLineSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IStepLineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StepLineSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked <see cref="IStepLineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStackedStepLineSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IStepLineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedStepLineSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for tacked <see cref="ILineSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStackedLineSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<ILineSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedLineSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForBarSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.BarSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for vertical <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForColumnSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ColumnSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for horizontal <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForRowSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ColumnSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStackedBarSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IStackedBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedBarSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked vertical <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStackedColumnSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IStackedBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedColumnSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for stacked horizontal <see cref="IBarSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForStackedRowSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IStackedBarSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.StackedRowSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IScatterSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForScatterSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IScatterSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.ScatterSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IHeatSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForHeatSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IHeatSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.HeatSeriesBuilder.Add(predicate);
            return styler;
        }

        /// <summary>
        ///  Defines a style builder for <see cref="IFinancialSeries{TDrawingContext}"/> objects.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="styler">The styler.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static VisualsStyle<TDrawingContext> HasRuleForFinancialSeries<TDrawingContext>(
            this VisualsStyle<TDrawingContext> styler,
            Action<IFinancialSeries<TDrawingContext>> predicate)
            where TDrawingContext : DrawingContext
        {
            styler.FinancialSeriesBuilder.Add(predicate);
            return styler;
        }
    }
}
