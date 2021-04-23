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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a cartesina chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="Chart{TDrawingContext}" />
    public class CartesianChart<TDrawingContext> : Chart<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        internal readonly HashSet<ISeries> _everMeasuredSeries = new();
        internal readonly HashSet<IAxis<TDrawingContext>> _everMeasuredAxes = new();
        private readonly ICartesianChartView<TDrawingContext> _chartView;
        private int _nextSeries = 0;
        private double _zoomingSpeed = 0;
        private ZoomAndPanMode _zoomMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="defaultPlatformConfig">The default platform configuration.</param>
        /// <param name="canvas">The canvas.</param>
        public CartesianChart(
            ICartesianChartView<TDrawingContext> view,
            Action<LiveChartsSettings> defaultPlatformConfig,
            MotionCanvas<TDrawingContext> canvas)
            : base(canvas, defaultPlatformConfig)
        {
            _chartView = view;

            view.PointStates.Chart = this;
            foreach (var item in view.PointStates.GetStates())
            {
                if (item.Fill != null)
                {
                    item.Fill.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Fill);
                }
                if (item.Stroke != null)
                {
                    item.Stroke.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Stroke);
                }
            }
        }

        /// <summary>
        /// Gets the x axes.
        /// </summary>
        /// <value>
        /// The x axes.
        /// </value>
        public IAxis<TDrawingContext>[] XAxes { get; private set; } = new IAxis<TDrawingContext>[0];

        /// <summary>
        /// Gets the y axes.
        /// </summary>
        /// <value>
        /// The y axes.
        /// </value>
        public IAxis<TDrawingContext>[] YAxes { get; private set; } = new IAxis<TDrawingContext>[0];

        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public ICartesianSeries<TDrawingContext>[] Series { get; private set; } = new ICartesianSeries<TDrawingContext>[0];

        /// <summary>
        /// Gets the drawable series.
        /// </summary>
        /// <value>
        /// The drawable series.
        /// </value>
        public override IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries => Series;

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public override IChartView<TDrawingContext> View => _chartView;

        /// <inheritdoc cref="IChart.Update(bool)" />
        public override void Update(bool throttling = true)
        {
            if (!throttling)
            {
                updateThrottler.ForceCall();
                return;
            }

            updateThrottler.Call();
        }

        /// <summary>
        /// Finds the points near to the specified location.
        /// </summary>
        /// <param name="pointerPosition">The pointer position.</param>
        /// <returns></returns>
        public override IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition)
        {
            return _chartView.Series.SelectMany(series => series.FindPointsNearTo(this, pointerPosition));
        }

        /// <summary>
        /// Scales the specified point to the UI.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="xAxisIndex">Index of the x axis.</param>
        /// <param name="yAxisIndex">Index of the y axis.</param>
        /// <returns></returns>
        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            var xAxis = XAxes[xAxisIndex];
            var yAxis = YAxes[yAxisIndex];

            var xScaler = new Scaler(drawMaringLocation, drawMarginSize, xAxis);
            var yScaler = new Scaler(drawMaringLocation, drawMarginSize, yAxis);

            return new PointF(xScaler.ToChartValues(point.X), yScaler.ToChartValues(point.Y));
        }

        /// <summary>
        /// Zooms to the specified pivot.
        /// </summary>
        /// <param name="pivot">The pivot.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public void Zoom(PointF pivot, ZoomDirection direction)
        {
            if (YAxes == null || XAxes == null) return;

            var speed = _zoomingSpeed < 0.1 ? 0.1 : (_zoomingSpeed > 0.95 ? 0.95 : _zoomingSpeed);
            var m = direction == ZoomDirection.ZoomIn ? speed : 1 / speed;

            if ((_zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
            {
                for (var index = 0; index < XAxes.Length; index++)
                {
                    var xi = XAxes[index];
                    var px = new Scaler(drawMaringLocation, drawMarginSize, xi).ToChartValues(pivot.X);

                    var max = xi.MaxLimit == null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                    var min = xi.MinLimit == null ? xi.DataBounds.Min : xi.MinLimit.Value;

                    var l = max - min;

                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;
                    //if (target < xi.View.MinRange) return;
                    var mint = px - target * rMin;
                    var maxt = px + target * rMax;

                    if (maxt > xi.DataBounds.Max) maxt = xi.DataBounds.Max;
                    if (mint < xi.DataBounds.Min) mint = xi.DataBounds.Min;

                    xi.MaxLimit = maxt;
                    xi.MinLimit = mint;
                }
            }

            if ((_zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
            {
                for (var index = 0; index < YAxes.Length; index++)
                {
                    var yi = YAxes[index];
                    var px = new Scaler(drawMaringLocation, drawMarginSize, yi).ToChartValues(pivot.X);

                    var max = yi.MaxLimit == null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                    var min = yi.MinLimit == null ? yi.DataBounds.Min : yi.MinLimit.Value;

                    var l = max - min;

                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;
                    //if (target < xi.View.MinRange) return;
                    var mint = px - target * rMin;
                    var maxt = px + target * rMax;

                    if (maxt > yi.DataBounds.Max) maxt = yi.DataBounds.Max;
                    if (mint < yi.DataBounds.Min) mint = yi.DataBounds.Min;

                    yi.MaxLimit = maxt;
                    yi.MinLimit = mint;
                }
            }
        }

        /// <summary>
        /// Pans with the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        public void Pan(PointF delta)
        {
            if ((_zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
            {
                for (var index = 0; index < XAxes.Length; index++)
                {
                    var xi = XAxes[index];
                    var scale = new Scaler(drawMaringLocation, drawMarginSize, xi);
                    var dx = scale.ToChartValues(-delta.X) - scale.ToChartValues(0);

                    var max = xi.MaxLimit == null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                    var min = xi.MinLimit == null ? xi.DataBounds.Min : xi.MinLimit.Value;

                    xi.MaxLimit = max + dx > xi.DataBounds.Max ? xi.DataBounds.Max : max + dx;
                    xi.MinLimit = min + dx < xi.DataBounds.Min ? xi.DataBounds.Min : min + dx;
                }
            }

            if ((_zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
            {
                for (var index = 0; index < YAxes.Length; index++)
                {
                    var yi = XAxes[index];
                    var scale = new Scaler(drawMaringLocation, drawMarginSize, yi);
                    var dy = scale.ToChartValues(delta.Y) - scale.ToChartValues(0);

                    var max = yi.MaxLimit == null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                    var min = yi.MinLimit == null ? yi.DataBounds.Min : yi.MinLimit.Value;

                    yi.MaxLimit = max + dy > yi.DataBounds.Max ? yi.DataBounds.Max : max + dy;
                    yi.MinLimit = min + dy < yi.DataBounds.Min ? yi.DataBounds.Min : min + dy;
                }
            }
        }

        /// <summary>
        /// Measures this chart.
        /// </summary>
        /// <returns></returns>
        protected override void Measure()
        {
            seriesContext = new SeriesContext<TDrawingContext>(Series);

            Canvas.MeasuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
            var theme = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
            if (theme.CurrentColors == null || theme.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            lock (canvas.Sync)
            {
                // restart axes bounds and meta data
                foreach (var axis in XAxes)
                {
                    axis.Initialize(AxisOrientation.X);
                    theme.ResolveAxisDefaults(axis);
                }
                foreach (var axis in YAxes)
                {
                    axis.Initialize(AxisOrientation.Y);
                    theme.ResolveAxisDefaults(axis);
                }

                // get seriesBounds
                foreach (var series in Series)
                {
                    if (series.SeriesId == -1) series.SeriesId = _nextSeries++;
                    theme.ResolveSeriesDefaults(theme.CurrentColors, series);

                    var secondaryAxis = XAxes[series.ScalesXAt];
                    var primaryAxis = YAxes[series.ScalesYAt];

                    var seriesBounds = series.GetBounds(this, secondaryAxis, primaryAxis);

                    _ = secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.max);
                    _ = secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.min);
                    _ = secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisibleSecondaryBounds.max);
                    _ = secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisibleSecondaryBounds.min);
                    _ = primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.max);
                    _ = primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.min);
                    _ = primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisiblePrimaryBounds.max);
                    _ = primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisiblePrimaryBounds.min);
                }

                if (legend != null) legend.Draw(this);

                if (viewDrawMargin == null)
                {
                    var m = viewDrawMargin ?? new Margin();
                    float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                    SetDrawMargin(controlSize, m);

                    foreach (var axis in XAxes)
                    {
                        var s = axis.GetPossibleSize(this);
                        if (axis.Position == AxisPosition.Start)
                        {
                            // X Bottom
                            axis.Yo = m.Bottom + s.Height * 0.5f;
                            bs += s.Height;
                            m.Bottom = bs;
                            if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                            if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                        }
                        else
                        {
                            // X Top
                            axis.Yo = ts + s.Height * 0.5f;
                            ts += s.Height;
                            m.Top = ts;
                            if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                            if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;
                        }
                    }
                    foreach (var axis in YAxes)
                    {
                        var s = axis.GetPossibleSize(this);
                        var w = s.Width > m.Left ? s.Width : m.Left;
                        if (axis.Position == AxisPosition.Start)
                        {
                            // Y Left
                            axis.Xo = ls + w * 0.5f;
                            ls += w;
                            m.Left = ls;
                            if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                            if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                        }
                        else
                        {
                            // Y Right
                            axis.Xo = rs + w * 0.5f;
                            rs += w;
                            m.Right = rs;
                            if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                            if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
                        }
                    }

                    SetDrawMargin(controlSize, m);
                }

                // invalid dimensions, probably the chart is too small
                // or it is initializing in the UI and has no dimensions yet
                if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

                var totalAxes = YAxes.Concat(XAxes).ToArray();
                var toDeleteAxes = new HashSet<IAxis<TDrawingContext>>(_everMeasuredAxes);
                foreach (var axis in totalAxes)
                {
                    if (axis.DataBounds.Max == axis.DataBounds.Min)
                    {
                        var c = axis.DataBounds.Min * 0.3;
                        axis.DataBounds.Min = axis.DataBounds.Min - c;
                        axis.DataBounds.Max = axis.DataBounds.Max + c;
                    }

                    axis.Measure(this);
                    _ = _everMeasuredAxes.Add(axis);
                    _ = toDeleteAxes.Remove(axis);

                    var deleted = false;
                    foreach (var item in axis.DeletingTasks)
                    {
                        canvas.RemovePaintTask(item);
                        item.Dispose();
                        deleted = true;
                    }
                    if (deleted) axis.DeletingTasks.Clear();
                }

                var toDeleteSeries = new HashSet<ISeries>(_everMeasuredSeries);

                foreach (var series in Series)
                {
                    var secondaryAxis = XAxes[series.ScalesXAt];
                    var primaryAxis = YAxes[series.ScalesYAt];
                    series.Measure(this, secondaryAxis, primaryAxis);
                    _ = _everMeasuredSeries.Add(series);
                    _ = toDeleteSeries.Remove(series);

                    var deleted = false;
                    foreach (var item in series.DeletingTasks)
                    {
                        canvas.RemovePaintTask(item);
                        item.Dispose();
                        deleted = true;
                    }
                    if (deleted) series.DeletingTasks.Clear();
                }

                foreach (var series in toDeleteSeries)
                {
                    series.Dispose();
                    _ = _everMeasuredSeries.Remove(series);
                }
                foreach (var axis in toDeleteAxes)
                {
                    axis.Dispose();
                    _ = _everMeasuredAxes.Remove(axis);
                }
            }

            Canvas.Invalidate();
        }

        /// <summary>
        /// Called when the updated the throttler is unlocked.
        /// </summary>
        /// <returns></returns>
        protected override void UpdateThrottlerUnlocked()
        {
            // before measure every element in the chart
            // we copy the properties that might change while we are updating the chart
            // this call should be thread safe
            // ToDo: ensure it is thread safe...

            viewDrawMargin = _chartView.DrawMargin;
            controlSize = _chartView.ControlSize;
            YAxes = _chartView.YAxes.Cast<IAxis<TDrawingContext>>().Select(x => x).ToArray();
            XAxes = _chartView.XAxes.Cast<IAxis<TDrawingContext>>().Select(x => x).ToArray();

            _zoomingSpeed = _chartView.ZoomingSpeed;
            _zoomMode = _chartView.ZoomMode;

            Series = _chartView.Series
                .Where(x => x.IsVisible)
                .Cast<ICartesianSeries<TDrawingContext>>()
                .Select(series =>
                {
                    _ = series.Fetch(this);
                    return series;
                }).ToArray();

            legendPosition = _chartView.LegendPosition;
            legendOrientation = _chartView.LegendOrientation;
            legend = _chartView.Legend; // ... this is a reference type.. this has no sense

            tooltipPosition = _chartView.TooltipPosition;
            tooltipFindingStrategy = _chartView.TooltipFindingStrategy;
            tooltip = _chartView.Tooltip; //... no sense again...

            animationsSpeed = _chartView.AnimationsSpeed;
            easingFunction = _chartView.EasingFunction;

            Measure();
        }
    }
}
