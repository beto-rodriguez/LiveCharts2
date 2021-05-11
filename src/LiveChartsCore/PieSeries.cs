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
using LiveChartsCore.Measure;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <inheritdoc cref="IPieSeries{TDrawingContext}" />
    public class PieSeries<TModel, TVisual, TLabel, TDrawingContext>
        : DrawableSeries<TModel, TVisual, TLabel, TDrawingContext>, IDisposable, IPieSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IDoughnutVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private double _pushout = 5;
        private double _innerRadius = 0;
        private double _maxOuterRadius = 1;
        private double _hoverPushout = 20;
        private double _innerPadding = 0;
        private double _outerPadding = 0;
        private bool _isFillSeries;
        private PolarLabelsPosition _labelsPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        public PieSeries() : base(SeriesProperties.PieSeries | SeriesProperties.Stacked)
        {
            HoverState = LiveCharts.PieSeriesHoverKey;
        }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.Pushout"/>
        public double Pushout { get => _pushout; set { _pushout = value; OnPropertyChanged(); } }
        /// <inheritdoc cref="IPieSeries{TDrawingContext}.InnerRadius"/>
        public double InnerRadius { get => _innerRadius; set { _innerRadius = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.MaxOuterRadius"/>
        public double MaxOuterRadius { get => _maxOuterRadius; set { _maxOuterRadius = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.HoverPushout"/>
        public double HoverPushout { get => _hoverPushout; set { _hoverPushout = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.RelativeInnerRadius"/>
        public double RelativeInnerRadius { get => _innerPadding; set { _innerPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.RelativeOuterRadius"/>
        public double RelativeOuterRadius { get => _outerPadding; set { _outerPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.IsFillSeries"/>
        public bool IsFillSeries { get => _isFillSeries; set { _isFillSeries = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data labels position.
        /// </summary>
        /// <value>
        /// The data labels position.
        /// </value>
        public PolarLabelsPosition DataLabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.Measure(PieChart{TDrawingContext})"/>
        public void Measure(PieChart<TDrawingContext> chart)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;

            var maxPushout = (float)chart.PushoutBounds.Max;
            var pushout = (float)Pushout;
            var innerRadius = (float)InnerRadius;
            var maxOuterRadius = (float)MaxOuterRadius;

            minDimension = minDimension - (Stroke?.StrokeThickness ?? 0) * 2 - maxPushout * 2;
            minDimension *= maxOuterRadius;

            var view = (IPieChartView<TDrawingContext>)chart.View;
            var initialRotation = (float)Math.Truncate(view.InitialRotation);
            var chartTotal = (float?)view.Total;

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
                DataLabelsDrawableTask.ZIndex = 1000 + actualZIndex + 0.3;
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
            }

            var completeAngle = 360f;

            var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
            var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

            var dls = (float)DataLabelsSize;
            var stacker = chart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker == null) throw new NullReferenceException("Unexpected null stacker");

            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var fetched = Fetch(chart).ToArray();

            var stackedInnerRadius = innerRadius;
            var relativeInnerRadius = (float)RelativeInnerRadius;
            var relativeOuterRadius = (float)RelativeOuterRadius;
            var i = 1f;

            foreach (var point in fetched)
            {
                var visual = point.Context.Visual as TVisual;

                if (point.IsNull)
                {
                    if (visual != null)
                    {
                        visual.CenterX = cx;
                        visual.CenterY = cy;
                        visual.X = cx;
                        visual.Y = cy;
                        visual.Width = 0;
                        visual.Height = 0;
                        visual.SweepAngle = 0;
                        visual.StartAngle = initialRotation;
                        visual.PushOut = 0;
                        visual.InnerRadius = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                var stack = stacker.GetStack(point);
                var stackedValue = stack.Start;
                var total = chartTotal ?? stack.Total;

                float start, end;
                if (total == 0)
                {
                    start = 0;
                    end = 0;
                }
                else
                {
                    start = stackedValue / total * completeAngle;
                    end = (stackedValue + point.PrimaryValue) / total * completeAngle - start;
                }

                if (IsFillSeries)
                {
                    start = 0;
                    end = 359.9f;
                }

                if (visual == null)
                {
                    var p = new TVisual
                    {
                        CenterX = cx,
                        CenterY = cy,
                        X = cx,
                        Y = cy,
                        Width = 0,
                        Height = 0,
                        StartAngle = chart.IsFirstDraw ? initialRotation : start + initialRotation,
                        SweepAngle = 0,
                        PushOut = 0,
                        InnerRadius = 0
                    };

                    visual = p;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    p.CompleteAllTransitions();

                    _ = everFetched.Add(point);
                }

                if (Fill != null) Fill.AddGeometyToPaintTask(visual);
                if (Stroke != null) Stroke.AddGeometyToPaintTask(visual);

                var dougnutGeometry = visual;

                stackedInnerRadius += relativeInnerRadius;

                var md = minDimension;
                var w = md - (md - 2 * innerRadius) * (fetched.Length - i) / fetched.Length - relativeOuterRadius * 2;

                dougnutGeometry.CenterX = cx;
                dougnutGeometry.CenterY = cy;
                dougnutGeometry.X = (drawMarginSize.Width - w) * 0.5f;
                dougnutGeometry.Y = (drawMarginSize.Height - w) * 0.5f;
                dougnutGeometry.Width = w;
                dougnutGeometry.Height = w;
                dougnutGeometry.InnerRadius = stackedInnerRadius;
                dougnutGeometry.PushOut = pushout;
                dougnutGeometry.RemoveOnCompleted = false;
                dougnutGeometry.StartAngle = start + initialRotation;
                dougnutGeometry.SweepAngle = end;
                if (start == initialRotation && end == completeAngle) dougnutGeometry.SweepAngle = completeAngle - 0.0001f;

                point.Context.HoverArea = new SemicircleHoverArea()
                    .SetDimensions(cx, cy, start + initialRotation, start + initialRotation + end, md * 0.5f);

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsDrawableTask != null && point.PrimaryValue > 0)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label == null)
                    {
                        var l = new TLabel { X = cx, Y = cy };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                        DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                    }

                    label.Text = DataLabelsFormatter(point);
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;

                    if (DataLabelsPosition == PolarLabelsPosition.Start)
                    {
                        var a = start + initialRotation;
                        a %= 360;
                        if (a < 0) a += 360;
                        var c = 90;

                        if (a > 180) c = -90;

                        label.HorizontalAlign = a > 180 ? Align.End : Align.Start;
                        label.Rotation = a - c;
                    }

                    if (DataLabelsPosition == PolarLabelsPosition.End)
                    {
                        var a = start + initialRotation + end;
                        a %= 360;
                        if (a < 0) a += 360;
                        var c = 90;

                        if (a > 180) c = -90;

                        label.HorizontalAlign = a > 180 ? Align.Start : Align.End;
                        label.Rotation = a - c;
                    }

                    var labelPosition = GetLabelPolarPosition(
                        cx, cy, ((w + relativeOuterRadius * 2) * 0.5f + stackedInnerRadius) * 0.5f, start + initialRotation, end,
                        label.Measure(DataLabelsDrawableTask), DataLabelsPosition);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }

                stackedInnerRadius = (w + relativeOuterRadius * 2) * 0.5f;

                i++;
            }

            var u = new Scaler();
            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, u, u);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.GetBounds(PieChart{TDrawingContext})"/>
        public DimensionalBounds GetBounds(PieChart<TDrawingContext> chart)
        {
            return dataProvider == null ? throw new Exception("Data provider not found") : dataProvider.GetPieBounds(chart, this);
        }

        /// <summary>
        /// Defines the default behavior when a point is added to a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected override void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual.PushOut = (float)HoverPushout;
        }

        /// <summary>
        /// Defines the default behavior when a point is removed from a state.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="chart">The chart.</param>
        protected override void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual.PushOut = (float)Pushout;
        }

        /// <summary>
        /// Called when the paint context changed.
        /// </summary>
        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            var w = LegendShapeSize;
            var sh = 0f;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = 0,
                    Y = 0,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize,
                    CenterX = (float)LegendShapeSize * 0.5f,
                    CenterY = (float)LegendShapeSize * 0.5f,
                    StartAngle = 0,
                    SweepAngle = 359.9999f
                };
                sh = strokeClone.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * strokeClone.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(strokeClone);
            }

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual
                {
                    X = sh,
                    Y = sh,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize,
                    CenterX = (float)LegendShapeSize * 0.5f,
                    CenterY = (float)LegendShapeSize * 0.5f,
                    StartAngle = 0,
                    SweepAngle = 359.9999f
                };
                fillClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(fillClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
            OnPropertyChanged(nameof(DefaultPaintContext));
        }

        /// <summary>
        /// GEts the stack group
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public override int GetStackGroup()
        {
            return 0;
        }

        /// <summary>
        /// Sets the default point transitions.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <exception cref="Exception">Unable to initialize the point instance.</exception>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

            _ = visual
                .TransitionateProperties(
                    nameof(visual.CenterX),
                    nameof(visual.CenterY),
                    nameof(visual.X),
                    nameof(visual.Y),
                    nameof(visual.Width),
                    nameof(visual.Height),
                    nameof(visual.StartAngle),
                    nameof(visual.SweepAngle),
                    nameof(visual.PushOut),
                    nameof(visual.InnerRadius))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));
        }

        /// <summary>
        /// Softs the delete point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="primaryScale">The primary scale.</param>
        /// <param name="secondaryScale">The secondary scale.</param>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual == null) return;

            visual.StartAngle += visual.SweepAngle;
            visual.SweepAngle = 0;
            visual.RemoveOnCompleted = true;

            if (dataProvider == null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);
        }

        /// <summary>
        /// Gets the label polar position.
        /// </summary>
        /// <param name="centerX">The center x.</param>
        /// <param name="centerY">The center y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        /// <param name="labelSize">Size of the label.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        protected virtual PointF GetLabelPolarPosition(
            float centerX,
            float centerY,
            float radius,
            float startAngle,
            float sweepAngle,
            SizeF labelSize,
            PolarLabelsPosition position)
        {
            const float toRadians = (float)(Math.PI / 180);
            float angle = 0;

            switch (position)
            {
                case PolarLabelsPosition.End:
                    angle = startAngle + sweepAngle;
                    break;
                case PolarLabelsPosition.Start:
                    angle = startAngle;
                    break;
                case PolarLabelsPosition.Middle:
                    angle = (startAngle + sweepAngle) * 0.5f;
                    break;
                case PolarLabelsPosition.ChartCenter:
                    return new PointF(centerX, centerY);
                default:
                    break;
            }

            angle %= 360;
            if (angle < 0) angle += 360;
            angle *= toRadians;

            return new PointF(
                 (float)(centerX + Math.Cos(angle) * radius),
                 (float)(centerY + Math.Sin(angle) * radius));
        }

        /// <summary>
        /// Deletes the point from the chart.
        /// </summary>
        /// <param name="chart"></param>
        /// <inheritdoc cref="M:LiveChartsCore.ISeries.Delete(LiveChartsCore.Kernel.IChartView)" />
        public override void Delete(IChartView chart)
        {
            var u = new Scaler();

            var toDelete = new List<ChartPoint>();
            foreach (var point in everFetched)
            {
                if (point.Context.Chart != chart) continue;
                SoftDeletePoint(point, u, u);
                toDelete.Add(point);
            }

            foreach (var item in toDelete) _ = everFetched.Remove(item);
        }
    }
}
