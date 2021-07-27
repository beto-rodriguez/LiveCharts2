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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Drawing;

namespace LiveChartsCore
{
    /// <inheritdoc cref="IPieSeries{TDrawingContext}" />
    public abstract class PieSeries<TModel, TVisual, TLabel, TDrawingContext>
        : ChartSeries<TModel, TVisual, TLabel, TDrawingContext>, IPieSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IDoughnutVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private IPaintTask<TDrawingContext>? _stroke = null;
        private IPaintTask<TDrawingContext>? _fill = null;
        private double _pushout = 0;
        private double _innerRadius = 0;
        private double _maxOuterRadius = 1;
        private double _hoverPushout = 20;
        private double _innerPadding = 0;
        private double _outerPadding = 0;
        private double _maxRadialColW = double.MaxValue;
        private double _cornerRadius = 0;
        private RadialAlignment _radialAlign = RadialAlignment.Outer;
        private bool _invertedCornerRadius = false;
        private bool _isFillSeries;
        private PolarLabelsPosition _labelsPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        protected PieSeries(bool isGauge = false, bool isGaugeFill = false)
            : base(SeriesProperties.PieSeries | SeriesProperties.Stacked |
                  (isGauge ? SeriesProperties.Gauge : 0) | (isGaugeFill ? SeriesProperties.GaugeFill : 0) | SeriesProperties.Solid)
        {
            HoverState = LiveCharts.PieSeriesHoverKey;
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public IPaintTask<TDrawingContext>? Stroke
        {
            get => _stroke;
            set => SetPaintProperty(ref _stroke, value, true);
        }

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public IPaintTask<TDrawingContext>? Fill
        {
            get => _fill;
            set => SetPaintProperty(ref _fill, value);
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

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.MaxRadialColumnWidth"/>
        public double MaxRadialColumnWidth { get => _maxRadialColW; set { _maxRadialColW = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.RadialAlign"/>
        public RadialAlignment RadialAlign { get => _radialAlign; set { _radialAlign = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.CornerRadius"/>
        public double CornerRadius { get => _cornerRadius; set { _cornerRadius = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.InvertedCornerRadius"/>
        public bool InvertedCornerRadius { get => _invertedCornerRadius; set { _invertedCornerRadius = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.IsFillSeries"/>
        public bool IsFillSeries { get => _isFillSeries; set { _isFillSeries = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data labels position.
        /// </summary>
        /// <value>
        /// The data labels position.
        /// </value>
        public PolarLabelsPosition DataLabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var pieChart = (PieChart<TDrawingContext>)chart;

            var drawLocation = pieChart.DrawMarginLocation;
            var drawMarginSize = pieChart.DrawMarginSize;
            var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;

            var maxPushout = (float)pieChart.PushoutBounds.Max;
            var pushout = (float)Pushout;
            var innerRadius = (float)InnerRadius;
            var maxOuterRadius = (float)MaxOuterRadius;

            minDimension = minDimension - (Stroke?.StrokeThickness ?? 0) * 2 - maxPushout * 2;
            minDimension *= maxOuterRadius;

            var view = (IPieChartView<TDrawingContext>)pieChart.View;
            var initialRotation = (float)Math.Truncate(view.InitialRotation);
            var completeAngle = (float)view.MaxAngle;
            var chartTotal = (float?)view.Total;

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill is not null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.SetClipRectangle(pieChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                pieChart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke is not null)
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.SetClipRectangle(pieChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                pieChart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsPaint is not null)
            {
                DataLabelsPaint.ZIndex = 1000 + actualZIndex + 0.3;
                DataLabelsPaint.SetClipRectangle(pieChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                pieChart.Canvas.AddDrawableTask(DataLabelsPaint);
            }

            var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
            var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

            var dls = (float)DataLabelsSize;
            var stacker = pieChart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker is null) throw new NullReferenceException("Unexpected null stacker");

            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var fetched = Fetch(pieChart).ToArray();

            var stackedInnerRadius = innerRadius;
            var relativeInnerRadius = (float)RelativeInnerRadius;
            var relativeOuterRadius = (float)RelativeOuterRadius;
            var maxRadialWidth = (float)MaxRadialColumnWidth;
            var cornerRadius = (float)CornerRadius;

            var mdc = minDimension;
            var wc = mdc - (mdc - 2 * innerRadius) * (fetched.Length - 1) / fetched.Length - relativeOuterRadius * 2;

            if (wc * 0.5f - stackedInnerRadius > maxRadialWidth)
            {
                var dw = wc * 0.5f - stackedInnerRadius - maxRadialWidth;

                switch (RadialAlign)
                {
                    case RadialAlignment.Outer:
                        relativeOuterRadius = 0;
                        relativeInnerRadius = dw;
                        break;
                    case RadialAlignment.Center:
                        relativeOuterRadius = dw * 0.5f;
                        relativeInnerRadius = dw * 0.5f;
                        break;
                    case RadialAlignment.Inner:
                        relativeOuterRadius = dw;
                        relativeInnerRadius = 0;
                        break;
                    default:
                        throw new NotImplementedException($"The alignment {RadialAlign} is not supported.");
                }
            }

            var i = 1f;

            foreach (var point in fetched)
            {
                var visual = point.Context.Visual as TVisual;

                if (point.IsNull)
                {
                    if (visual is not null)
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
                        visual.CornerRadius = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                var stack = stacker.GetStack(point);
                var stackedValue = stack.Start;
                var total = chartTotal ?? stack.Total;

                double start, end;
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
                    end = completeAngle - 0.1f;
                }

                if (visual is null)
                {
                    var p = new TVisual
                    {
                        CenterX = cx,
                        CenterY = cy,
                        X = cx,
                        Y = cy,
                        Width = 0,
                        Height = 0,
                        StartAngle = (float)(pieChart.IsFirstDraw ? initialRotation : start + initialRotation),
                        SweepAngle = 0,
                        PushOut = 0,
                        InnerRadius = 0,
                        CornerRadius = 0
                    };

                    visual = p;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    p.CompleteAllTransitions();

                    _ = everFetched.Add(point);
                }

                if (Fill is not null) Fill.AddGeometryToPaintTask(pieChart.Canvas, visual);
                if (Stroke is not null) Stroke.AddGeometryToPaintTask(pieChart.Canvas, visual);

                var dougnutGeometry = visual;

                stackedInnerRadius += relativeInnerRadius;

                var md = minDimension;
                var w = md - (md - 2 * innerRadius) * (fetched.Length - i) / fetched.Length - relativeOuterRadius * 2;

                var x = (drawMarginSize.Width - w) * 0.5f;

                dougnutGeometry.CenterX = cx;
                dougnutGeometry.CenterY = cy;
                dougnutGeometry.X = x;
                dougnutGeometry.Y = (drawMarginSize.Height - w) * 0.5f;
                dougnutGeometry.Width = w;
                dougnutGeometry.Height = w;
                dougnutGeometry.InnerRadius = stackedInnerRadius;
                dougnutGeometry.PushOut = pushout;
                dougnutGeometry.StartAngle = (float)(start + initialRotation);
                dougnutGeometry.SweepAngle = (float)end;
                dougnutGeometry.CornerRadius = cornerRadius;
                dougnutGeometry.InvertedCornerRadius = InvertedCornerRadius;
                dougnutGeometry.RemoveOnCompleted = false;
                if (start == initialRotation && end == completeAngle) dougnutGeometry.SweepAngle = completeAngle - 0.1f;

                point.Context.HoverArea = new SemicircleHoverArea()
                    .SetDimensions(cx, cy, (float)(start + initialRotation), (float)(start + initialRotation + end), md * 0.5f);

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null && point.PrimaryValue > 0)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = cx, Y = cy };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? pieChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? pieChart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(pieChart.Canvas, label);

                    label.Text = DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
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
                        label.Rotation = (float)(a - c);
                    }

                    if (DataLabelsPosition == PolarLabelsPosition.End)
                    {
                        var a = start + initialRotation + end;
                        a %= 360;
                        if (a < 0) a += 360;
                        var c = 90;

                        if (a > 180) c = -90;

                        label.HorizontalAlign = a > 180 ? Align.Start : Align.End;
                        label.Rotation = (float)(a - c);
                    }

                    var labelPosition = GetLabelPolarPosition(
                        cx, cy, ((w + relativeOuterRadius * 2) * 0.5f + stackedInnerRadius) * 0.5f,
                        (float)(start + initialRotation), (float)end,
                        label.Measure(DataLabelsPaint), DataLabelsPosition);

                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }

                stackedInnerRadius = (w + relativeOuterRadius * 2) * 0.5f;
                i++;
            }

            var u = new Scaler();
            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != pieChart.View) continue;
                SoftDeletePoint(point, u, u);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.GetBounds(PieChart{TDrawingContext})"/>
        public DimensionalBounds GetBounds(PieChart<TDrawingContext> chart)
        {
            return dataProvider is null
                ? throw new Exception("Data provider not found")
                : dataProvider.GetPieBounds(chart, this).Bounds;
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
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
            }

            if (Fill is not null)
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
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
            }

            context.Width = w;
            context.Height = w;

            canvaSchedule = context;
            OnPropertyChanged(nameof(CanvasSchedule));
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

        /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
        protected override IPaintTask<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { _fill, _stroke, DataLabelsPaint };
        }

        /// <summary>
        /// Called when [paint changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected override void OnPaintChanged(string? propertyName)
        {
            OnSeriesMiniatureChanged();
            OnPropertyChanged();
        }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
        public override bool MiniatureEquals(IChartSeries<TDrawingContext> instance)
        {
            return instance is PieSeries<TModel, TVisual, TLabel, TDrawingContext> pieSeries &&
               Name == pieSeries.Name && Fill == pieSeries.Fill && Stroke == pieSeries.Stroke;
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
            if (visual is null) return;

            visual.StartAngle += visual.SweepAngle;
            visual.SweepAngle = 0;
            visual.CornerRadius = 0;
            visual.RemoveOnCompleted = true;

            if (dataProvider is null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
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
                    angle = startAngle + sweepAngle * 0.5f;
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
        /// Softly deletes the all points from the chart.
        /// </summary>
        /// <param name="chart"></param>
        /// <inheritdoc cref="M:LiveChartsCore.ISeries.Delete(LiveChartsCore.Kernel.IChartView)" />
        public override void SoftDelete(IChartView chart)
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
