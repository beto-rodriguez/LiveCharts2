﻿// The MIT License(MIT)
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
using System.Collections.Generic;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Themes
{
    /// <summary>
    /// Defines an object that must initialize live charts visual objects, this object defines how things will 
    /// be drawn by default, it is highly related to themes.
    /// </summary>
    public class VisualsStyle<TDrawingContext> where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the chart builder.
        /// </summary>
        /// <value>
        /// The chart builder.
        /// </value>
        public List<Action<IChartView<TDrawingContext>>> ChartBuilder { get; set; } = new List<Action<IChartView<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the axis builder.
        /// </summary>
        /// <value>
        /// The axis builder.
        /// </value>
        public List<Action<IAxis<TDrawingContext>>> AxisBuilder { get; set; } = new List<Action<IAxis<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the draw margin frame builder.
        /// </summary>
        /// <value>
        /// The draw margin frame builder.
        /// </value>
        public List<Action<DrawMarginFrame<TDrawingContext>>> DrawMarginFrameBuilder { get; set; } = new List<Action<DrawMarginFrame<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IChartSeries<TDrawingContext>>> SeriesBuilder { get; set; } = new List<Action<IChartSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the pie series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IPieSeries<TDrawingContext>>> PieSeriesBuilder { get; set; } = new List<Action<IPieSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the gauge series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IPieSeries<TDrawingContext>>> GaugeSeriesBuilder { get; set; } = new List<Action<IPieSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the gauge fill series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IPieSeries<TDrawingContext>>> GaugeFillSeriesBuilder { get; set; } = new List<Action<IPieSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the Cartesian series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<ICartesianSeries<TDrawingContext>>> CartesianSeriesBuilder { get; set; } = new List<Action<ICartesianSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the stepline series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IStepLineSeries<TDrawingContext>>> StepLineSeriesBuilder { get; set; } = new List<Action<IStepLineSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the  stacked stepline series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IStepLineSeries<TDrawingContext>>> StackedStepLineSeriesBuilder { get; set; } = new List<Action<IStepLineSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the line series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<ILineSeries<TDrawingContext>>> LineSeriesBuilder { get; set; } = new List<Action<ILineSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the line series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IHeatSeries<TDrawingContext>>> HeatSeriesBuilder { get; set; } = new List<Action<IHeatSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the financial series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IFinancialSeries<TDrawingContext>>> FinancialSeriesBuilder { get; set; } = new List<Action<IFinancialSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the stacked line series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<ILineSeries<TDrawingContext>>> StackedLineSeriesBuilder { get; set; } = new List<Action<ILineSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the bar series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IBarSeries<TDrawingContext>>> BarSeriesBuilder { get; set; } = new List<Action<IBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the column series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IBarSeries<TDrawingContext>>> ColumnSeriesBuilder { get; set; } = new List<Action<IBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the row series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IBarSeries<TDrawingContext>>> RowSeriesBuilder { get; set; } = new List<Action<IBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the stacked bar series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IStackedBarSeries<TDrawingContext>>> StackedBarSeriesBuilder { get; set; } = new List<Action<IStackedBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the stacked column series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IStackedBarSeries<TDrawingContext>>> StackedColumnSeriesBuilder { get; set; } = new List<Action<IStackedBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the stacked row series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IStackedBarSeries<TDrawingContext>>> StackedRowSeriesBuilder { get; set; } = new List<Action<IStackedBarSeries<TDrawingContext>>>();

        /// <summary>
        /// Gets or sets the scatter series builder.
        /// </summary>
        /// <value>
        /// The pie series builder.
        /// </value>
        public List<Action<IScatterSeries<TDrawingContext>>> ScatterSeriesBuilder { get; set; } = new List<Action<IScatterSeries<TDrawingContext>>>();

        /// <summary>
        /// Constructs a chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void ApplyStyleToChart(IChartView<TDrawingContext> chart)
        {
            foreach (var rule in ChartBuilder) rule(chart);
        }

        /// <summary>
        /// Constructs an axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void ApplyStyleToAxis(IAxis<TDrawingContext> axis)
        {
            foreach (var rule in AxisBuilder) rule(axis);
        }

        /// <summary>
        /// Constructs a series.
        /// </summary>
        /// <param name="series">The series.</param>
        public virtual void ApplyStyleToSeries(IChartSeries<TDrawingContext> series)
        {
            foreach (var rule in SeriesBuilder) rule(series);

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                if ((series.SeriesProperties & SeriesProperties.Gauge) != 0)
                {
                    if ((series.SeriesProperties & SeriesProperties.GaugeFill) != 0)
                    {
                        foreach (var rule in GaugeFillSeriesBuilder) rule((IPieSeries<TDrawingContext>)series);
                    }
                    else
                    {
                        foreach (var rule in GaugeSeriesBuilder) rule((IPieSeries<TDrawingContext>)series);
                    }
                }
                else
                {
                    foreach (var rule in PieSeriesBuilder) rule((IPieSeries<TDrawingContext>)series);
                }
            }

            if ((series.SeriesProperties & SeriesProperties.CartesianSeries) == SeriesProperties.CartesianSeries)
            {
                foreach (var rule in CartesianSeriesBuilder) rule((ICartesianSeries<TDrawingContext>)series);
            }

            if ((series.SeriesProperties & SeriesProperties.Bar) == SeriesProperties.Bar &&
                (series.SeriesProperties & SeriesProperties.Stacked) != SeriesProperties.Stacked)
            {
                var barSeries = (IBarSeries<TDrawingContext>)series;
                foreach (var rule in BarSeriesBuilder) rule(barSeries);

                if ((series.SeriesProperties & SeriesProperties.PrimaryAxisVerticalOrientation) == SeriesProperties.PrimaryAxisVerticalOrientation)
                {
                    foreach (var rule in ColumnSeriesBuilder) rule(barSeries);
                }

                if ((series.SeriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation)
                {
                    foreach (var rule in RowSeriesBuilder) rule(barSeries);
                }
            }

            var stackedBarMask = SeriesProperties.Bar | SeriesProperties.Stacked;
            if ((series.SeriesProperties & stackedBarMask) == stackedBarMask)
            {
                var stackedBarSeries = (IStackedBarSeries<TDrawingContext>)series;
                foreach (var rule in StackedBarSeriesBuilder) rule(stackedBarSeries);

                if ((series.SeriesProperties & SeriesProperties.PrimaryAxisVerticalOrientation) == SeriesProperties.PrimaryAxisVerticalOrientation)
                {
                    foreach (var rule in StackedColumnSeriesBuilder) rule(stackedBarSeries);
                }

                if ((series.SeriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation)
                {
                    foreach (var rule in StackedRowSeriesBuilder) rule(stackedBarSeries);
                }
            }

            if ((series.SeriesProperties & SeriesProperties.Scatter) == SeriesProperties.Scatter)
            {
                foreach (var rule in ScatterSeriesBuilder) rule((IScatterSeries<TDrawingContext>)series);
            }

            if ((series.SeriesProperties & SeriesProperties.StepLine) == SeriesProperties.StepLine)
            {
                var stepSeries = (IStepLineSeries<TDrawingContext>)series;
                foreach (var rule in StepLineSeriesBuilder) rule(stepSeries);

                if ((series.SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked)
                {
                    foreach (var rule in StackedStepLineSeriesBuilder) rule(stepSeries);
                }
            }

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<TDrawingContext>)series;
                foreach (var rule in LineSeriesBuilder) rule(lineSeries);

                if ((series.SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked)
                {
                    foreach (var rule in StackedLineSeriesBuilder) rule(lineSeries);
                }
            }

            if ((series.SeriesProperties & SeriesProperties.Heat) == SeriesProperties.Heat)
            {
                var heatSeries = (IHeatSeries<TDrawingContext>)series;
                foreach (var rule in HeatSeriesBuilder) rule(heatSeries);
            }

            if ((series.SeriesProperties & SeriesProperties.Financial) == SeriesProperties.Financial)
            {
                var financialSeries = (IFinancialSeries<TDrawingContext>)series;
                foreach (var rule in FinancialSeriesBuilder) rule(financialSeries);
            }
        }
    }
}
