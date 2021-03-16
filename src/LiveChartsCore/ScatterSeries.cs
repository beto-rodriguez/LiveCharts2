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
    public class ScatterSeries<TModel, TVisual, TLabel, TDrawingContext> : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IScatterSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private double geometrySize = 24d;
        private double minGeometrySize = 6d;
        private Bounds weightBounds = new Bounds();

        public ScatterSeries()
            : base(SeriesProperties.Scatter)
        {
            HoverState = LiveCharts.ScatterSeriesHoverKey;
        }

        public double MinGeometrySize { get => minGeometrySize; set => minGeometrySize = value; }

        public double GeometrySize { get => geometrySize; set => geometrySize = value; }

        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            if (Fill != null) chart.Canvas.AddDrawableTask(Fill);
            if (Stroke != null) chart.Canvas.AddDrawableTask(Stroke);
            if (DataLabelsDrawableTask != null) chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
            var dls = unchecked((float)DataLabelsSize);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeThickness ?? 0;
            var requiresWScale = weightBounds.max - weightBounds.min > 0;
            var wm = -(geometrySize - minGeometrySize) / (weightBounds.max - weightBounds.min);

            foreach (var point in Fetch(chart))
            {
                var x = xScale.ScaleToUi(point.SecondaryValue);
                var y = yScale.ScaleToUi(point.PrimaryValue);

                if (point.IsNull)
                {
                    if (point.Context.Visual != null)
                    {
                        point.Context.Visual.X = x - hgs;
                        point.Context.Visual.Y = y - hgs;
                        point.Context.Visual.Width = 0;
                        point.Context.Visual.Height = 0;
                        point.Context.Visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (requiresWScale)
                {
                    gs = (float)(wm * (weightBounds.max - point.TertiaryValue) + geometrySize);
                    hgs = gs / 2f;
                }

                if (point.Context.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - hgs,
                        Y = y - hgs,
                        Width = 0,
                        Height = 0
                    };

                    point.Context.Visual = r;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = point.Context.Visual;

                sizedGeometry.X = x - hgs;
                sizedGeometry.Y = y - hgs;
                sizedGeometry.Width = gs;
                sizedGeometry.Height = gs;
                sizedGeometry.RemoveOnCompleted = false;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs, gs + 2 * sw, gs + 2 * sw);

                OnPointMeasured(point);
                chart.MeasuredDrawables.Add(sizedGeometry);

                if (DataLabelsDrawableTask != null)
                {
                    if (point.Context.Label == null)
                    {
                        var l = new TLabel { X = x - hgs, Y = y - hgs };

                        l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(a =>
                                a.WithDuration(chart.AnimationsSpeed)
                                .WithEasingFunction(chart.EasingFunction));

                        l.CompleteAllTransitions();
                        point.Context.Label = l;
                        DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                    }

                    point.Context.Label.Text = DataLabelFormatter(point);
                    point.Context.Label.TextSize = dls;
                    point.Context.Label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, point.Context.Label.Measure(DataLabelsDrawableTask), DataLabelsPosition, SeriesProperties, point.PrimaryValue > 0);
                    point.Context.Label.X = labelPosition.X;
                    point.Context.Label.Y = labelPosition.Y;

                    chart.MeasuredDrawables.Add(point.Context.Label);
                }
            }
        }

        public override DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = new DimensinalBounds();
            weightBounds = new Bounds();
            foreach (var point in Fetch(chart))
            {
                var primary = point.PrimaryValue;
                var secondary = point.SecondaryValue;
                var tertiary = point.TertiaryValue;

                baseBounds.PrimaryBounds.AppendValue(primary);
                baseBounds.SecondaryBounds.AppendValue(secondary);
                weightBounds.AppendValue(tertiary);
            }

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new DimensinalBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + tick.Value,
                    Min = baseBounds.SecondaryBounds.Min - tick.Value
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize
                };
                w += 2 * strokeClone.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }

        protected override void SetDefaultPointTransitions(IChartPoint<TVisual, TLabel, TDrawingContext> chartPoint)
        {
            var visual = chartPoint.Context.Visual;
            var chart = chartPoint.Context.Chart;

            if (visual == null) throw new Exception("Unable to initialize the point instance.");

            visual
               .TransitionateProperties(
                   nameof(visual.X),
                   nameof(visual.Y),
                   nameof(visual.Width),
                   nameof(visual.Height))
               .WithAnimation(animation =>
                   animation
                       .WithDuration(chart.AnimationsSpeed)
                       .WithEasingFunction(chart.EasingFunction));
        }
    }
}
