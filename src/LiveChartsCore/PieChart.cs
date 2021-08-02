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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LiveChartsCore.Measure;
using LiveChartsCore.Kernel.Sketches;
using System.Diagnostics;
using System.Threading;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a pie chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="Chart{TDrawingContext}" />
    public class PieChart<TDrawingContext> : Chart<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly HashSet<ISeries> _everMeasuredSeries = new();
        private readonly IPieChartView<TDrawingContext> _chartView;
        private int _nextSeries = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="defaultPlatformConfig">The default platform configuration.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="lockOnMeasure">Indicates if the thread should lock the measure operation</param>
        public PieChart(
            IPieChartView<TDrawingContext> view,
            Action<LiveChartsSettings> defaultPlatformConfig,
            MotionCanvas<TDrawingContext> canvas,
            bool lockOnMeasure = false)
            : base(canvas, defaultPlatformConfig, lockOnMeasure)
        {
            _chartView = view;

            view.PointStates.Chart = this;
            foreach (var item in view.PointStates.GetStates())
            {
                if (item.Fill is not null)
                {
                    item.Fill.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Fill);
                }
                if (item.Stroke is not null)
                {
                    item.Stroke.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Stroke);
                }
            }
        }

        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public IPieSeries<TDrawingContext>[] Series { get; private set; } = new IPieSeries<TDrawingContext>[0];

        /// <summary>
        /// Gets the drawable series.
        /// </summary>
        /// <value>
        /// The drawable series.
        /// </value>
        public override IEnumerable<IChartSeries<TDrawingContext>> DrawableSeries => Series;

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public override IChartView<TDrawingContext> View => _chartView;

        /// <summary>
        /// Gets the value bounds.
        /// </summary>
        /// <value>
        /// The value bounds.
        /// </value>
        public Bounds ValueBounds { get; private set; } = new Bounds();

        /// <summary>
        /// Gets the index bounds.
        /// </summary>
        /// <value>
        /// The index bounds.
        /// </value>
        public Bounds IndexBounds { get; private set; } = new Bounds();

        /// <summary>
        /// Gets the pushout bounds.
        /// </summary>
        /// <value>
        /// The pushout bounds.
        /// </value>
        public Bounds PushoutBounds { get; private set; } = new Bounds();

        /// <summary>
        /// Finds the points near to the specified point.
        /// </summary>
        /// <param name="pointerPosition">The pointer position.</param>
        /// <returns></returns>
        public override IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition)
        {
            return _chartView.Series.SelectMany(
                series => series.FindPointsNearTo(this, pointerPosition, TooltipFindingStrategy.CompareAll));
        }

        /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
        public override void Update(ChartUpdateParams? chartUpdateParams = null)
        {
            chartUpdateParams ??= new ChartUpdateParams();

            if (chartUpdateParams.IsAutomaticUpdate && !View.AutoUpdateEnabled) return;

            if (!chartUpdateParams.Throttling)
            {
                updateThrottler.ForceCall();
                return;
            }

            updateThrottler.Call();
        }

        /// <summary>
        /// Measures this chart.
        /// </summary>
        /// <returns></returns>
        protected override void Measure()
        {
#if DEBUG
            if (LiveCharts.EnableLogging)
            {
                Trace.WriteLine(
                    $"[Cartesian chart measured]".PadRight(60) +
                    $"tread: {Thread.CurrentThread.ManagedThreadId}");
            }
#endif

            InvokeOnMeasuring();

            if (LockOnMeasure) Monitor.Enter(canvas.Sync);

            if (preserveFirstDraw)
            {
                IsFirstDraw = true;
                preserveFirstDraw = false;
            }

            MeasureWork = new object();

            viewDrawMargin = _chartView.DrawMargin;
            controlSize = _chartView.ControlSize;

            Series = _chartView.Series
                .Where(x => x.IsVisible)
                .Cast<IPieSeries<TDrawingContext>>()
                .ToArray();

            legendPosition = _chartView.LegendPosition;
            legendOrientation = _chartView.LegendOrientation;
            legend = _chartView.Legend;

            tooltipPosition = _chartView.TooltipPosition;
            tooltip = _chartView.Tooltip;

            animationsSpeed = _chartView.AnimationsSpeed;
            easingFunction = _chartView.EasingFunction;

            seriesContext = new SeriesContext<TDrawingContext>(Series);

            var theme = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
            if (theme.CurrentColors is null || theme.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            var forceApply = ThemeId != LiveCharts.CurrentSettings.ThemeId && !IsFirstDraw;

            ValueBounds = new Bounds();
            IndexBounds = new Bounds();
            PushoutBounds = new Bounds();
            foreach (var series in Series)
            {
                series.IsNotifyingChanges = false;

                if (series.SeriesId == -1) series.SeriesId = _nextSeries++;
                theme.ResolveSeriesDefaults(theme.CurrentColors, series, forceApply);

                var seriesBounds = series.GetBounds(this);

                ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Max);
                ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Min);
                IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Max);
                IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Min);
                PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Max);
                PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Min);

                series.IsNotifyingChanges = true;
            }

            if (legend is not null && SeriesMiniatureChanged(Series, LegendPosition))
            {
                legend.Draw(this);
                Update();
                preserveFirstDraw = IsFirstDraw;
            }

            if (viewDrawMargin is null)
            {
                var m = viewDrawMargin ?? new Margin();
                SetDrawMargin(controlSize, m);
            }

            // invalid dimensions, probably the chart is too small
            // or it is initializing in the UI and has no dimensions yet
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            var toDeleteSeries = new HashSet<ISeries>(_everMeasuredSeries);
            foreach (var series in Series)
            {
                series.Measure(this);
                series.RemoveOldPaints(View);
                _ = _everMeasuredSeries.Add(series);
                _ = toDeleteSeries.Remove(series);
            }

            foreach (var series in toDeleteSeries)
            {
                series.SoftDelete(View);
                _ = _everMeasuredSeries.Remove(series);
            }

            InvokeOnUpdateStarted();
            IsFirstDraw = false;
            ThemeId = LiveCharts.CurrentSettings.ThemeId;
            previousSeries = Series;
            previousLegendPosition = LegendPosition;

            if (LockOnMeasure) Monitor.Exit(canvas.Sync);

            Canvas.Invalidate();
        }

        /// <summary>
        /// Called when the updated the throttler is unlocked.
        /// </summary>
        /// <returns></returns>
        protected override void UpdateThrottlerUnlocked()
        {
            Measure();
        }
    }
}
