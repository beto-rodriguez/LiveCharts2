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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a scatter series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="LiveChartsCore.CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="LiveChartsCore.Kernel.IScatterSeries{TDrawingContext}" />
    public class ScatterSeries<TModel, TVisual, TLabel, TDrawingContext> : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IScatterSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly Func<float, float> easing = EasingFunctions.BuildCustomElasticOut(1.2f, 0.40f);
        private double geometrySize = 24d;
        private double minGeometrySize = 6d;
        private Bounds weightBounds = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        public ScatterSeries()
            : base(SeriesProperties.Scatter)
        {
            HoverState = LiveCharts.ScatterSeriesHoverKey;

            DataLabelsFormatter = (point) => $"{point.SecondaryValue}, {point.PrimaryValue}";
            TooltipLabelFormatter = (point) => $"{point.Context.Series.Name} {point.SecondaryValue}, {point.PrimaryValue}";
        }

        /// <summary>
        /// Gets or sets the minimum size of the geometry.
        /// </summary>
        /// <value>
        /// The minimum size of the geometry.
        /// </value>
        public double MinGeometrySize { get => minGeometrySize; set => minGeometrySize = value; }

        /// <summary>
        /// Gets or sets the size of the geometry.
        /// </summary>
        /// <value>
        /// The size of the geometry.
        /// </value>
        public double GeometrySize { get => geometrySize; set => geometrySize = value; }

        /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.Measure(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new Scaler(drawLocation, drawMarginSize, xAxis);
            var yScale = new Scaler(drawLocation, drawMarginSize, yAxis);

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill != null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke != null)
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsDrawableTask != null)
            {
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.3;
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
            }

            var dls = (float)DataLabelsSize;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var gs = (float)geometrySize;
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeThickness ?? 0;
            var requiresWScale = weightBounds.max - weightBounds.min > 0;
            var wm = -(geometrySize - minGeometrySize) / (weightBounds.max - weightBounds.min);

            foreach (var point in Fetch(chart))
            {
                var visual = (TVisual?)point.Context.Visual;

                var x = xScale.ToPixels(point.SecondaryValue);
                var y = yScale.ToPixels(point.PrimaryValue);

                if (point.IsNull)
                {
                    if (visual != null)
                    {
                        visual.X = x - hgs;
                        visual.Y = y - hgs;
                        visual.Width = 0;
                        visual.Height = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (requiresWScale)
                {
                    gs = (float)(wm * (weightBounds.max - point.TertiaryValue) + geometrySize);
                    hgs = gs / 2f;
                }

                if (visual == null)
                {
                    var r = new TVisual
                    {
                        X = x,
                        Y = y,
                        Width = 0,
                        Height = 0
                    };

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    everFetched.Add(point);
                }

                if (Fill != null) Fill.AddGeometyToPaintTask(visual);
                if (Stroke != null) Stroke.AddGeometyToPaintTask(visual);

                var sizedGeometry = visual;

                sizedGeometry.X = x - hgs;
                sizedGeometry.Y = y - hgs;
                sizedGeometry.Width = gs;
                sizedGeometry.Height = gs;
                sizedGeometry.RemoveOnCompleted = false;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs, gs + 2 * sw, gs + 2 * sw);

                OnPointMeasured(point);
                toDeletePoints.Remove(point);

                if (DataLabelsDrawableTask != null)
                {
                    var label = point.Context.Label as TLabel;

                    if (label == null)
                    {
                        var l = new TLabel { X = x - hgs, Y = y - hgs };

                        l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(a =>
                                a.WithDuration(chart.AnimationsSpeed)
                                .WithEasingFunction(chart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                        DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                    }

                    label.Text = DataLabelsFormatter(point);
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsDrawableTask), DataLabelsPosition, SeriesProperties, point.PrimaryValue > 0);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, yScale, xScale);
                everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override DimensionalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);
            weightBounds = baseBounds.TertiaryBounds;

            var tick = y.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);

            return new DimensionalBounds
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
                },
                VisibleSecondaryBounds = new Bounds
                {
                    Max = baseBounds.VisibleSecondaryBounds.Max + tick.Value,
                    Min = baseBounds.VisibleSecondaryBounds.Min - tick.Value
                },
                VisiblePrimaryBounds = new Bounds
                {
                    Max = baseBounds.VisiblePrimaryBounds.Max + tick.Value,
                    min = baseBounds.VisiblePrimaryBounds.min - tick.Value
                },
            };
        }

        /// <inheritdoc cref="OnPaintContextChanged"/>
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

        /// <inheritdoc cref="SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var visual = (TVisual?)chartPoint.Context.Visual;
            var chart = chartPoint.Context.Chart;

            if (visual == null) throw new Exception("Unable to initialize the point instance.");

            visual
               .TransitionateProperties(
                   nameof(visual.X),
                   nameof(visual.Y))
               .WithAnimation(animation =>
                   animation
                       .WithDuration(chart.AnimationsSpeed)
                       .WithEasingFunction(chart.EasingFunction));

            visual
               .TransitionateProperties(
                   nameof(visual.Width),
                   nameof(visual.Height))
               .WithAnimation(animation =>
                   animation
                       .WithDuration(chart.AnimationsSpeed)
                       .WithEasingFunction(easing));
        }

        /// <inheritdoc cref="SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual == null) return;

            visual.Height = 0;
            visual.Width = 0;
            visual.RemoveOnCompleted = true;
        }
    }
}
