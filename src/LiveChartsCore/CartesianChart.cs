﻿// The MIT License(MIT)

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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public class CartesianChart<TDrawingContext> : Chart<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly ICartesianChartView<TDrawingContext> chartView;
        private int nextSeries = 0;
        private IAxis<TDrawingContext>[] secondaryAxes = new IAxis<TDrawingContext>[0];
        private IAxis<TDrawingContext>[] primaryAxes = new IAxis<TDrawingContext>[0];
        private ICartesianSeries<TDrawingContext>[] series = new ICartesianSeries<TDrawingContext>[0];

        public CartesianChart(
            ICartesianChartView<TDrawingContext> view,
            Action<LiveChartsSettings> defaultPlatformConfig,
            Canvas<TDrawingContext> canvas)
            : base(canvas, defaultPlatformConfig)
        {
            chartView = view;

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

        public IAxis<TDrawingContext>[] XAxes => secondaryAxes;
        public IAxis<TDrawingContext>[] YAxes => primaryAxes;
        public ICartesianSeries<TDrawingContext>[] Series => series;
        public override IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries => series;
        public override IChartView<TDrawingContext> View => chartView;

        public override void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        public override IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition)
        {
            if (measureWorker == null) return Enumerable.Empty<TooltipPoint>();

            return chartView.Series.SelectMany(series => series.FindPointsNearTo(this, pointerPosition));
        }

        protected override void Measure()
        {
            measuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
            seriesContext = new SeriesContext<TDrawingContext>(series);

            if (legend != null) legend.Draw(this);

            // restart axes bounds and meta data
            foreach (var axis in secondaryAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in primaryAxes) axis.Initialize(AxisOrientation.Y);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<TDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            // get seriesBounds
            foreach (var series in series)
            {
                if (series.SeriesId == -1) series.SeriesId = nextSeries++;
                initializer.ResolveSeriesDefaults(stylesBuilder.CurrentColors, series);

                var secondaryAxis = secondaryAxes[series.ScalesXAt];
                var primaryAxis = primaryAxes[series.ScalesYAt];

                var seriesBounds = series.GetBounds(this, secondaryAxis, primaryAxis);

                secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.max);
                secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.min);
                primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.max);
                primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.min);
            }

            if (viewDrawMargin == null)
            {
                var m = viewDrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                foreach (var axis in secondaryAxes)
                {
                    var s = axis.GetPossibleSize(this);
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // X Bottom
                        axis.Yo = m.Bottom + s.Height * 0.5f;
                        bs = bs + s.Height;
                        m.Bottom = bs;
                        //if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                        //if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                    }
                    else
                    {
                        // X Top
                        axis.Yo = ts + s.Height * 0.5f;
                        ts += s.Height;
                        m.Top = ts;
                        //if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                        //if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;
                    }
                }
                foreach (var axis in primaryAxes)
                {
                    var s = axis.GetPossibleSize(this);
                    var w = s.Width > m.Left ? s.Width : m.Left;
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // Y Left
                        axis.Xo = ls + w * 0.5f;
                        ls += w;
                        m.Left = ls;
                        //if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                        //if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                    }
                    else
                    {
                        // Y Right
                        axis.Xo = rs + w * 0.5f;
                        rs += w;
                        m.Right = rs;
                        //if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                        //if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
                    }
                }

                SetDrawMargin(controlSize, m);
            }

            // invalid dimensions, probably the chart is too small
            // or it is initializing in the UI and has no dimensions yet
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            foreach (var axis in secondaryAxes)
            {
                axis.Measure(this);
            }
            foreach (var axis in primaryAxes)
            {
                axis.Measure(this);
            }
            foreach (var series in series)
            {
                var secondaryAxis = secondaryAxes[series.ScalesXAt];
                var primaryAxis = primaryAxes[series.ScalesYAt];
                series.Measure(this, secondaryAxis, primaryAxis);
            }

            chartView.CoreCanvas.ForEachGeometry((geometry, drawable) =>
            {
                if (measuredDrawables.Contains(geometry)) return; // then the geometry was measured

                // at this point,the geometry is not required in the UI
                geometry.RemoveOnCompleted = true;
            });

            Canvas.Invalidate();
        }

        protected override void UpdateThrottlerUnlocked()
        {
            // before measure every element in the chart
            // we copy the properties that might change while we are updating the chart
            // this call should be thread safe
            // ToDo: ensure it is thread safe...

            viewDrawMargin = chartView.DrawMargin;
            controlSize = chartView.ControlSize;
            primaryAxes = chartView.YAxes.Select(x => x.Copy()).ToArray();
            secondaryAxes = chartView.XAxes.Select(x => x.Copy()).ToArray();

            measureWorker = new object();
            series = chartView.Series.Select(series =>
            {
                // a good implementation of ISeries<T>
                // must use the measureWorker to identify
                // if the points are already fetched.

                // this way no matter if the Series.Values collection changes
                // the fetch method will always return the same collection for the
                // current measureWorker instance

                series.Fetch(this);
                return series;
            }).ToArray();

            legendPosition = chartView.LegendPosition;
            legendOrientation = chartView.LegendOrientation;
            legend = chartView.Legend; // ... this is a reference type.. this has no sense

            tooltipPosition = chartView.TooltipPosition;
            tooltipFindingStrategy = chartView.TooltipFindingStrategy;
            tooltip = chartView.Tooltip; //... no sense again...

            animationsSpeed = chartView.AnimationsSpeed;
            easingFunction = chartView.EasingFunction;

            Measure();
        }
    }
}
