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
using LiveChartsCore.Measure;
using System.Collections.Generic;
using System.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Data;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a scatter series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="IScatterSeries{TDrawingContext}" />
    public class ScatterSeries<TModel, TVisual, TLabel, TDrawingContext> : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IScatterSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private Bounds _weightBounds = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        public ScatterSeries()
            : base(SeriesProperties.Scatter | SeriesProperties.Solid | SeriesProperties.PrefersXYStrategyTooltips)
        {
            DataPadding = new PointF(1, 1);

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
        public double MinGeometrySize { get; set; } = 6d;

        /// <summary>
        /// Gets or sets the size of the geometry.
        /// </summary>
        /// <value>
        /// The size of the geometry.
        /// </value>
        public double GeometrySize { get; set; } = 24d;

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var drawLocation = cartesianChart.DrawMarginLocation;
            var drawMarginSize = cartesianChart.DrawMarginSize;
            var xScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
            var yScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill is not null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke is not null)
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsPaint is not null)
            {
                DataLabelsPaint.ZIndex = actualZIndex + 0.3;
                DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
            }

            var dls = (float)DataLabelsSize;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var gs = (float)GeometrySize;
            var hgs = gs / 2f;
            var sw = Stroke?.StrokeThickness ?? 0;
            var requiresWScale = _weightBounds.Max - _weightBounds.Min > 0;
            var wm = -(GeometrySize - MinGeometrySize) / (_weightBounds.Max - _weightBounds.Min);

            foreach (var point in Fetch(cartesianChart))
            {
                var visual = (TVisual?)point.Context.Visual;

                var x = xScale.ToPixels(point.SecondaryValue);
                var y = yScale.ToPixels(point.PrimaryValue);

                if (point.IsNull)
                {
                    if (visual is not null)
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
                    gs = (float)(wm * (_weightBounds.Max - point.TertiaryValue) + GeometrySize);
                    hgs = gs / 2f;
                }

                if (visual is null)
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

                    _ = everFetched.Add(point);
                }

                if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

                var sizedGeometry = visual;

                sizedGeometry.X = x - hgs;
                sizedGeometry.Y = y - hgs;
                sizedGeometry.Width = gs;
                sizedGeometry.Height = gs;
                sizedGeometry.RemoveOnCompleted = false;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs, gs + 2 * sw, gs + 2 * sw);

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null)
                {
                    if (point.Context.Label is not TLabel label)
                    {
                        var l = new TLabel { X = x - hgs, Y = y - hgs };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                    label.Text = DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsPaint), DataLabelsPosition, SeriesProperties, point.PrimaryValue > 0);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != cartesianChart.View) continue;
                SoftDeletePoint(point, yScale, xScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseSeriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            if (baseSeriesBounds.HasData) return baseSeriesBounds;
            var baseBounds = baseSeriesBounds.Bounds;

            _weightBounds = baseBounds.TertiaryBounds;

            var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
            var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

            var ts = tickSecondary.Value * DataPadding.X;
            var tp = tickPrimary.Value * DataPadding.Y;

            return
                new SeriesBounds(
                    new DimensionalBounds
                    {
                        SecondaryBounds = new Bounds
                        {
                            Max = baseBounds.SecondaryBounds.Max + ts,
                            Min = baseBounds.SecondaryBounds.Min - ts
                        },
                        PrimaryBounds = new Bounds
                        {
                            Max = baseBounds.PrimaryBounds.Max + tp,
                            Min = baseBounds.PrimaryBounds.Min - tp
                        },
                        VisibleSecondaryBounds = new Bounds
                        {
                            Max = baseBounds.VisibleSecondaryBounds.Max + ts,
                            Min = baseBounds.VisibleSecondaryBounds.Min - ts
                        },
                        VisiblePrimaryBounds = new Bounds
                        {
                            Max = baseBounds.VisiblePrimaryBounds.Max + tp,
                            Min = baseBounds.VisiblePrimaryBounds.Min - tp
                        },
                        MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                        MinDeltaSecondary = baseBounds.MinDeltaSecondary
                    },
                    false);
        }

        /// <inheritdoc cref="OnSeriesMiniatureChanged"/>
        protected override void OnSeriesMiniatureChanged()
        {
            var context = new CanvasSchedule<TDrawingContext>();

            var w = LegendShapeSize;
            var sh = 0f;
            if (Stroke is not null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize
                };
                sh = strokeClone.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * strokeClone.StrokeThickness;
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
            }

            if (Fill is not null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
            }

            context.Width = w;
            context.Height = w;

            canvaSchedule = context;
            OnPropertyChanged(nameof(CanvasSchedule));
        }

        /// <inheritdoc cref="SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var visual = (TVisual?)chartPoint.Context.Visual;
            var chart = chartPoint.Context.Chart;

            if (visual is null) throw new Exception("Unable to initialize the point instance.");

            _ = visual
               .TransitionateProperties(
                   nameof(visual.X),
                   nameof(visual.Y),
                   nameof(visual.Width),
                   nameof(visual.Height))
               .WithAnimation(animation =>
                   animation
                       .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                       .WithEasingFunction(EasingFunction ?? chart.EasingFunction));
        }

        /// <inheritdoc cref="SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual is null) return;

            visual.Height = 0;
            visual.Width = 0;
            visual.RemoveOnCompleted = true;

            if (dataProvider is null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
        }
    }
}
