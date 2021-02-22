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
using LiveChartsCore.Rx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public class CartesianChartCore<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private object measureWorker = null;
        private HashSet<IDrawable<TDrawingContext>> measuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
        private SeriesContext<TDrawingContext> seriesContext = new SeriesContext<TDrawingContext>(
            Enumerable.Empty<ISeries<TDrawingContext>>());

        private readonly ICartesianChartView<TDrawingContext> chartView;
        private readonly Canvas<TDrawingContext> canvas;
        private readonly ActionThrottler updateThrottler;

        // view copied properties
        private SizeF controlSize = new SizeF();
        private Margin viewDrawMargin = null;
        private IAxis<TDrawingContext>[] xAxes = new IAxis<TDrawingContext>[0];
        private IAxis<TDrawingContext>[] yAxes = new IAxis<TDrawingContext>[0];
        private ISeries<TDrawingContext>[] series = new ISeries<TDrawingContext>[0];
        private LegendPosition legendPosition;
        private LegendOrientation legendOrientation;
        private IChartLegend<TDrawingContext> legend;
        private TooltipPosition tooltipPosition;
        private TooltipFindingStrategy tooltipFindingStrategy;
        private IChartTooltip<TDrawingContext> tooltip;
        private TimeSpan animationsSpeed;
        private Func<float, float> easingFunction;

        private SizeF drawMarginSize;
        private PointF drawMaringLocation;

        public CartesianChartCore(ICartesianChartView<TDrawingContext> view, Canvas<TDrawingContext> canvas)
        {
            this.canvas = canvas;
            chartView = view;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
        }

        public object MeasureWorker => measureWorker;
        public HashSet<IDrawable<TDrawingContext>> MeasuredDrawables => measuredDrawables;
        public SeriesContext<TDrawingContext> SeriesContext => seriesContext;

        public Canvas<TDrawingContext> Canvas => canvas;

        public SizeF ControlSize => controlSize;
        public PointF DrawMaringLocation => drawMaringLocation;
        public SizeF DrawMarginSize => drawMarginSize;
        public IAxis<TDrawingContext>[] XAxes => xAxes;
        public IAxis<TDrawingContext>[] YAxes => yAxes;
        public ISeries<TDrawingContext>[] Series => series;
        public LegendPosition LegendPosition => LegendPosition;
        public LegendOrientation LegendOrientation => legendOrientation;
        public IChartLegend<TDrawingContext> Legend => legend;
        public TooltipPosition TooltipPosition => tooltipPosition;
        public TooltipFindingStrategy TooltipFindingStrategy => tooltipFindingStrategy;
        public IChartTooltip<TDrawingContext> Tooltip => tooltip;
        public TimeSpan AnimationsSpeed => animationsSpeed;
        public Func<float, float> EasingFunction => easingFunction;

        public void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        public IEnumerable<FoundPoint<TDrawingContext>> FindPointsNearTo(PointF point)
        {
            if (measureWorker == null) return Enumerable.Empty<FoundPoint<TDrawingContext>>();

            return chartView.Series
                .SelectMany(series => series
                        .Fetch(this)
                        .Where(p => p.HoverArea.IsTriggerBy(point, chartView.TooltipFindingStrategy))
                        .Select(p => new FoundPoint<TDrawingContext> { Coordinate = p, Series = series }));
        }

        private void Measure()
        {
            measuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
            seriesContext = new SeriesContext<TDrawingContext>(series);

            if (legend != null) legend.Draw(chartView);

            // restart axes bounds and meta data
            foreach (var axis in xAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in yAxes) axis.Initialize(AxisOrientation.Y);

            // get seriesBounds
            foreach (var series in series)
            {
                var xAxis = xAxes[series.ScalesXAt];
                var yAxis = yAxes[series.ScalesYAt];

                var seriesBounds = series.GetBounds(this, xAxis, yAxis);

                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.max);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.min);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.max);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.min);
            }

            if (viewDrawMargin == null)
            {
                var m = viewDrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                foreach (var axis in xAxes)
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
                foreach (var axis in yAxes)
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

            foreach (var axis in xAxes)
            {
                axis.Measure(this);
            }
            foreach (var axis in yAxes)
            {
                axis.Measure(this);
            }
            foreach (var series in series)
            {
                var x = xAxes[series.ScalesXAt];
                var y = yAxes[series.ScalesYAt];
                series.Measure(this, x, y);
            }

            chartView.CoreCanvas.ForEachGeometry((geometry, paint) =>
            {
                if (measuredDrawables.Contains(geometry)) return; // then the geometry was measured

                // at this point,the geometry is not required in the UI
                geometry.RemoveOnCompleted = true;
            });

            Canvas.Invalidate();
        }

        private void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMaringLocation = new PointF(margin.Left, margin.Top);
        }

        private void UpdateThrottlerUnlocked()
        {
            // before measure every element in the chart
            // we copy the properties that might change while we are updating the chart
            // this call should be thread safe
            // ToDo: ensure it is thread safe...

            viewDrawMargin = chartView.DrawMargin;
            controlSize = chartView.ControlSize;
            yAxes = chartView.YAxes.Select(x => x.Copy()).ToArray();
            xAxes = chartView.XAxes.Select(x => x.Copy()).ToArray();

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
