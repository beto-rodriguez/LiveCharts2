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

namespace LiveChartsCore
{
    public class RowSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        public RowSeries()
            : base(SeriesProperties.Bar | SeriesProperties.HorizontalOrientation) { }

        public override void Measure(
           CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var secondaryScale = new ScaleContext(drawLocation, drawMarginSize, primaryAxis.Orientation, primaryAxis.DataBounds);
            var primaryScale = new ScaleContext(drawLocation, drawMarginSize, secondaryAxis.Orientation, secondaryAxis.DataBounds);

            float uw = secondaryScale.ScaleToUi(1f) - secondaryScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = primaryScale.ScaleToUi(unchecked((float)Pivot));

            var pos = chart.SeriesContext.GetColumnPostion(this);
            var count = chart.SeriesContext.GetColumnSeriesCount();
            float cp = 0f;

            if (!IgnoresBarPosition && count > 1)
            {
                uw = uw / count;
                uwm = 0.5f * uw;
                cp = (pos - (count / 2f)) * uw + uwm;
            }

            if (uw > MaxBarWidth)
            {
                uw = unchecked((float)MaxBarWidth);
                uwm = uw / 2f;
            }

            if (Fill != null) chart.Canvas.AddDrawableTask(Fill);
            if (Stroke != null) chart.Canvas.AddDrawableTask(Stroke);
            if (DataLabelsBrush != null) chart.Canvas.AddDrawableTask(DataLabelsBrush);
            var dls = unchecked((float)DataLabelsSize);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = OnPointCreated ?? DefaultOnPointCreated;

            foreach (var point in Fetch(chart))
            {
                var primary = primaryScale.ScaleToUi(point.PrimaryValue);
                var secondary = secondaryScale.ScaleToUi(point.SecondaryValue);
                float b = Math.Abs(primary - p);

                if (point.IsNull)
                {
                    if (point.Context.Visual != null)
                    {
                        point.Context.Visual.X = p;
                        point.Context.Visual.Y = secondary - uwm + cp;
                        point.Context.Visual.Width = 0;
                        point.Context.Visual.Height = uw;
                        point.Context.Visual.RemoveOnCompleted = true;
                    }
                    continue;
                }

                if (point.Context.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = p,
                        Y = secondary - uwm + cp,
                        Width = 0,
                        Height = uw
                    };

                    ts(r, chart.View);
                    r.CompleteAllTransitions();

                    point.Context.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = point.Context.Visual;

                var cx = point.PrimaryValue > Pivot ? primary - b : primary;
                var y = secondary - uwm + cp;

                sizedGeometry.X = cx;
                sizedGeometry.Y = y;
                sizedGeometry.Width = b;
                sizedGeometry.Height = uw;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(primary, secondary - uwm + cp, b, uw);
                OnPointMeasured(point, sizedGeometry);
                chart.MeasuredDrawables.Add(sizedGeometry);

                if (DataLabelsBrush != null)
                {
                    if (point.Context.Label == null)
                    {
                        var l = new TLabel { X = p, Y = secondary - uwm + cp };

                        l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(a =>
                                a.WithDuration(chart.AnimationsSpeed)
                                .WithEasingFunction(chart.EasingFunction));

                        l.CompleteAllTransitions();
                        point.Context.Label = l;
                        DataLabelsBrush.AddGeometyToPaintTask(l);
                    }

                    point.Context.Label.Text = DataLabelFormatter(point);
                    point.Context.Label.TextSize = dls;
                    point.Context.Label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        cx, y, b, uw, point.Context.Label.Measure(DataLabelsBrush), DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
                    point.Context.Label.X = labelPosition.X;
                    point.Context.Label.Y = labelPosition.Y;

                    chart.MeasuredDrawables.Add(point.Context.Label);
                }
            }
        }

        public override DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            var tick = secondaryAxis.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new DimensinalBounds
            {
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + 0.5,
                    Min = baseBounds.SecondaryBounds.Min - 0.5
                },
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected virtual void DefaultOnPointCreated(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual
                .TransitionateProperties(
                    nameof(visual.X),
                    nameof(visual.Width))
                .WithAnimation(animation =>
                    animation
                        .WithDuration((long)(chart.AnimationsSpeed.TotalMilliseconds * 1.5))
                        .WithEasingFunction(EasingFunctions.BounceOut));

            visual
                .TransitionateProperties(
                    nameof(visual.Y),
                    nameof(visual.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));
        }
    }
}
