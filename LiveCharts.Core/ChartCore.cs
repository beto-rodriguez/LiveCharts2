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
    public class ChartCore<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        private readonly IChartView<TDrawingContext> chartView;
        private readonly Canvas<TDrawingContext> naturalGeometriesCanvas;
        private readonly ActionThrottler updateThrottler;
        private SizeF drawMarginSize;
        private PointF drawMaringLocation;

        public ChartCore(IChartView<TDrawingContext> view, Canvas<TDrawingContext> canvas)
        {
            naturalGeometriesCanvas = canvas;
            chartView = view;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
        }

        public Canvas<TDrawingContext> NaturalGeometriesCanvas => naturalGeometriesCanvas;

        public IChartView<TDrawingContext> ChartView => chartView;

        internal PointF DrawMaringLocation => drawMaringLocation;
        internal SizeF DrawMarginSize => drawMarginSize;

        public void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        public IEnumerable<FoundPoint<TDrawingContext>> FindPointsNearTo(PointF point)
        {
            return chartView.Series
                .SelectMany(series => series
                        .Fetch(this)
                        .Where(p => p.HoverArea.IsTriggerBy(point, chartView.TooltipFindingStrategy))
                        .Select(p => new FoundPoint<TDrawingContext> { Coordinate = p, Series = series }));
        }

        private void Measure()
        {
            var drawBucket = new HashSet<IGeometry<TDrawingContext>>();

            if(chartView.Legend != null) chartView.Legend.Draw(chartView);
            var controlSize = chartView.ControlSize;

            // restart axes bounds and meta data
            foreach (var axis in chartView.XAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in chartView.YAxes) axis.Initialize(AxisOrientation.Y);

            // get series bounds
            foreach (var series in chartView.Series)
            {
                var xAxis = chartView.XAxes[series.ScalesXAt];
                var yAxis = chartView.YAxes[series.ScalesYAt];

                var seriesBounds = series.GetBounds(controlSize, xAxis, yAxis);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.max);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.min);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.max);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.min);
            }

            if (chartView.DrawMargin == null)
            {
                var m = chartView.DrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                foreach (var axis in chartView.XAxes)
                {
                    var s = axis.GetPossibleSize(chartView );
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
                foreach (var axis in chartView.YAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
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

            foreach (var axis in chartView.XAxes)
            {
                //axis.Measure(ChartView, drawBucket);
            }
            foreach (var axis in chartView.YAxes)
            {
                //axis.Measure(ChartView, drawBucket);
            }
            foreach (var series in chartView.Series)
            {
                var x = ChartView.XAxes[series.ScalesXAt];
                var y = ChartView.YAxes[series.ScalesYAt];
                series.Measure(chartView, x, y, drawBucket);
            }

            chartView.CoreCanvas.ForEachGeometry((geometry, paint) => 
            {
                if (drawBucket.Contains(geometry)) return; // then the geometry was updated by the measure method

                // at this point, no one used this geometry, we need to remove if from our canvas
                geometry.RemoveOnCompleted = true;
            });

            NaturalGeometriesCanvas.Invalidate();
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
            Measure();
        }
    }
}
