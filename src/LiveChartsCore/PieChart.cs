// The MIT License(MIT)

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
    public class PieChart<TDrawingContext> : Chart<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IPieChartView<TDrawingContext> chartView;
        private int nextSeries = 0;
        private IPieSeries<TDrawingContext>[] series = new IPieSeries<TDrawingContext>[0];

        public PieChart(
            IPieChartView<TDrawingContext> view,
            Action<LiveChartsSettings> defaultPlatformConfig,
            Canvas<TDrawingContext> canvas)
            : base(canvas, defaultPlatformConfig)
        {
            chartView = view;
        }

        public IPieSeries<TDrawingContext>[] Series => series;
        public override IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries => series;
        public override IChartView<TDrawingContext> ChartView => chartView;
        public Bounds ValueBounds { get; private set; } = new Bounds();
        public Bounds IndexBounds { get; private set; } = new Bounds();
        public Bounds PushoutBounds { get; private set; } = new Bounds();

        public override IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition)
        {
            if (measureWorker == null) return Enumerable.Empty<TooltipPoint>();

            return chartView.Series.SelectMany(series => series.FindPointsNearTo(this, pointerPosition));
        }

        public override void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        protected override void Measure()
        {
            if (series == null)
            {
                chartView.CoreCanvas.ForEachGeometry((geometry, drawable) =>
                {
                    if (measuredDrawables.Contains(geometry)) return; // then the geometry was measured

                    // at this point,the geometry is not required in the UI
                    geometry.RemoveOnCompleted = true;
                });
                return;
            }

            measuredDrawables =  new HashSet<IDrawable<TDrawingContext>>();
            seriesContext = new SeriesContext<TDrawingContext>(series);

            if (legend != null) legend.Draw(this);

            ValueBounds = new Bounds();
            IndexBounds = new Bounds();
            PushoutBounds = new Bounds();
            foreach (var series in series)
            {
                if (series.SeriesId == -1)
                {
                    series.SeriesId = nextSeries++;
                    LiveCharts.CurrentSettings.ApplySeriesStyle(series);
                }

                var seriesBounds = series.GetBounds(this);

                ValueBounds.AppendValue(seriesBounds.PrimaryBounds.max);
                ValueBounds.AppendValue(seriesBounds.PrimaryBounds.min);
                IndexBounds.AppendValue(seriesBounds.SecondaryBounds.max);
                IndexBounds.AppendValue(seriesBounds.SecondaryBounds.min);
                PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.max);
                PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.min);
            }

            if (viewDrawMargin == null)
            {
                var m = viewDrawMargin ?? new Margin();
                SetDrawMargin(controlSize, m);
                SetDrawMargin(controlSize, m);
            }

            // invalid dimensions, probably the chart is too small
            // or it is initializing in the UI and has no dimensions yet
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            foreach (var series in series)
            {
                series.Measure(this);
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
