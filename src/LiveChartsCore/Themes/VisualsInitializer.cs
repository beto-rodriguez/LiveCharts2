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
    /// Defines an object that must initialize live charts visual objects, this object defines how things will 
    /// be drawn by default, it is highly related to themes.
    /// </summary>
    public class VisualsInitializer<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the chart builder.
        /// </summary>
        /// <value>
        /// The chart builder.
        /// </value>
        public Action<IChartView<TDrawingContext>>? ChartBuilder { get; set; }

        /// <summary>
        /// Gets or sets the axis builder.
        /// </summary>
        /// <value>
        /// The axis builder.
        /// </value>
        public Action<IAxis<TDrawingContext>>? AxisBuilder { get; set; }

        /// <summary>
        /// Gets or sets the series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IDrawableSeries<TDrawingContext>>? SeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the pie series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IPieSeries<TDrawingContext>>? PieSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the Cartesian series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<ICartesianSeries<TDrawingContext>>? CartesianSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the line series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<ILineSeries<TDrawingContext>>? LineSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the stacked line series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<ILineSeries<TDrawingContext>>? StackedLineSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the bar series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IBarSeries<TDrawingContext>>? BarSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the column series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IBarSeries<TDrawingContext>>? ColumnSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the row series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IBarSeries<TDrawingContext>>? RowSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the stacked bar series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IStackedBarSeries<TDrawingContext>>? StackedBarSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the stacked column series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IStackedBarSeries<TDrawingContext>>? StackedColumnSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the stacked row series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IStackedBarSeries<TDrawingContext>>? StackedRowSeriesBuilder { get; set; }

        /// <summary>
        /// Gets or sets the scatter series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public Action<IScatterSeries<TDrawingContext>>? ScatterSeriesBuilder { get; set; }

        /// <summary>
        /// Constructs a chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void ApplyStyleToChart(IChartView<TDrawingContext> chart)
        {
            if (ChartBuilder == null) return;
            ChartBuilder(chart);
        }

        /// <summary>
        /// Constructs an axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void ApplyStyleToAxis(IAxis<TDrawingContext> axis)
        {
            if (AxisBuilder == null) return;
            AxisBuilder(axis);
        }

        /// <summary>
        /// Constructs a series.
        /// </summary>
        /// <param name="series">The series.</param>
        public virtual void ApplyStyleToSeries(IDrawableSeries<TDrawingContext> series)
        {
            SeriesBuilder?.Invoke(series);

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                PieSeriesBuilder?.Invoke((IPieSeries<TDrawingContext>)series);
            }

            if ((series.SeriesProperties & SeriesProperties.CartesianSeries) == SeriesProperties.CartesianSeries)
            {
                CartesianSeriesBuilder?.Invoke((ICartesianSeries<TDrawingContext>)series);
            }

            if ((series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar &&
                (series.SeriesProperties & SeriesProperties.Stacked) != SeriesProperties.Stacked)
            {
                var barSeries = (IBarSeries<TDrawingContext>)series;
                BarSeriesBuilder?.Invoke(barSeries);

                if ((series.SeriesProperties & SeriesProperties.VerticalOrientation) == SeriesProperties.VerticalOrientation)
                {
                    ColumnSeriesBuilder?.Invoke(barSeries);
                }

                if ((series.SeriesProperties & SeriesProperties.HorizontalOrientation) == SeriesProperties.HorizontalOrientation)
                {
                    RowSeriesBuilder?.Invoke(barSeries);
                }
            }

            var stackedBarMask = SeriesProperties.Bar | SeriesProperties.Stacked;
            if ((series.SeriesProperties & stackedBarMask) == stackedBarMask)
            {
                var stackedBarSeries = (IStackedBarSeries<TDrawingContext>)series;
                StackedBarSeriesBuilder?.Invoke(stackedBarSeries);

                if ((series.SeriesProperties & SeriesProperties.VerticalOrientation) == SeriesProperties.VerticalOrientation)
                {
                    StackedColumnSeriesBuilder?.Invoke(stackedBarSeries);
                }

                if ((series.SeriesProperties & SeriesProperties.HorizontalOrientation) == SeriesProperties.HorizontalOrientation)
                {
                    StackedRowSeriesBuilder?.Invoke(stackedBarSeries);
                }
            }

            if ((series.SeriesProperties & SeriesProperties.Scatter) == SeriesProperties.Scatter)
            {
                ScatterSeriesBuilder?.Invoke((IScatterSeries<TDrawingContext>)series);
            }

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<TDrawingContext>)series;
                LineSeriesBuilder?.Invoke(lineSeries);

                if ((series.SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked)
                {
                    StackedLineSeriesBuilder?.Invoke(lineSeries);
                }
            }
        }
    }
}
