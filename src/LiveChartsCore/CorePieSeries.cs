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

// Ignore Spelling: Gauge Pushout

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines a pie series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TMiniatureGeometry">The type of the miniature geometry, used in tool tips and legends.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class CorePieSeries<TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext>
    : ChartSeries<TModel, TVisual, TLabel, TDrawingContext>, IPieSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IDoughnutGeometry<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TMiniatureGeometry : ISizedGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _stroke = null;
    private IPaint<TDrawingContext>? _fill = null;
    private double _pushout = 0;
    private double _innerRadius = 0;
    private double _maxOuterRadius = 1;
    private double _outerRadiusOffset = 0;
    private double _hoverPushout = 20;
    private double _innerPadding = 0;
    private double _outerPadding = 0;
    private double _maxRadialColW = double.MaxValue;
    private double _cornerRadius = 0;
    private RadialAlignment _radialAlign = RadialAlignment.Outer;
    private bool _invertedCornerRadius = false;
    private bool _isFillSeries;
    private bool _isRelativeToMin;
    private PolarLabelsPosition _labelsPosition = PolarLabelsPosition.Middle;
    private Func<ChartPoint<TModel, TVisual, TLabel>, string>? _tooltipLabelFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorePieSeries{TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext}"/> class.
    /// </summary>
    protected CorePieSeries(bool isGauge = false, bool isGaugeFill = false)
        : base(SeriesProperties.PieSeries | SeriesProperties.Stacked |
              (isGauge ? SeriesProperties.Gauge : 0) | (isGaugeFill ? SeriesProperties.GaugeFill : 0) | SeriesProperties.Solid)
    { }

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    /// <value>
    /// The stroke.
    /// </value>
    public IPaint<TDrawingContext>? Stroke
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
    public IPaint<TDrawingContext>? Fill
    {
        get => _fill;
        set => SetPaintProperty(ref _fill, value);
    }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.Pushout"/>
    public double Pushout { get => _pushout; set => SetProperty(ref _pushout, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.InnerRadius"/>
    public double InnerRadius { get => _innerRadius; set => SetProperty(ref _innerRadius, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.OuterRadiusOffset"/>
    public double OuterRadiusOffset { get => _outerRadiusOffset; set => SetProperty(ref _outerRadiusOffset, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.MaxOuterRadius"/>
    [Obsolete($"Use {nameof(OuterRadiusOffset)} instead.")]
    public double MaxOuterRadius { get => _maxOuterRadius; set => SetProperty(ref _maxOuterRadius, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.HoverPushout"/>
    public double HoverPushout { get => _hoverPushout; set => SetProperty(ref _hoverPushout, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.RelativeInnerRadius"/>
    public double RelativeInnerRadius { get => _innerPadding; set => SetProperty(ref _innerPadding, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.RelativeOuterRadius"/>
    public double RelativeOuterRadius { get => _outerPadding; set => SetProperty(ref _outerPadding, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.MaxRadialColumnWidth"/>
    public double MaxRadialColumnWidth { get => _maxRadialColW; set => SetProperty(ref _maxRadialColW, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.RadialAlign"/>
    public RadialAlignment RadialAlign { get => _radialAlign; set => SetProperty(ref _radialAlign, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.CornerRadius"/>
    public double CornerRadius { get => _cornerRadius; set => SetProperty(ref _cornerRadius, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.InvertedCornerRadius"/>
    public bool InvertedCornerRadius { get => _invertedCornerRadius; set => SetProperty(ref _invertedCornerRadius, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.IsFillSeries"/>
    public bool IsFillSeries { get => _isFillSeries; set => SetProperty(ref _isFillSeries, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.IsRelativeToMinValue"/>
    public bool IsRelativeToMinValue { get => _isRelativeToMin; set => SetProperty(ref _isRelativeToMin, value); }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.DataLabelsPosition"/>
    public PolarLabelsPosition DataLabelsPosition { get => _labelsPosition; set => SetProperty(ref _labelsPosition, value); }

    /// <summary>
    /// Gets or sets the tool tip label formatter for the Y axis, this function will build the label when a point in this series 
    /// is shown inside a tool tip.
    /// </summary>
    /// <value>
    /// The tool tip label formatter.
    /// </value>
    public Func<ChartPoint<TModel, TVisual, TLabel>, string>? ToolTipLabelFormatter
    {
        get => _tooltipLabelFormatter;
        set { SetProperty(ref _tooltipLabelFormatter, value); _obsolete_formatter = value; }
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        var pieChart = (PieChart<TDrawingContext>)chart;

        var drawLocation = pieChart.DrawMarginLocation;
        var drawMarginSize = pieChart.DrawMarginSize;
        var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;

        var maxPushout = (float)pieChart.PushoutBounds.Max;
        var pushout = (float)Pushout;
        var innerRadius = (float)InnerRadius;

        minDimension = minDimension - (Stroke?.StrokeThickness ?? 0) * 2 - maxPushout * 2;

        var maxOuterRadius = (float)MaxOuterRadius;
        minDimension *= maxOuterRadius;

        var pieLabelsCorrection = chart.SeriesContext.GetPieOuterLabelsSpace<TLabel>();
        minDimension -= pieLabelsCorrection;

        var outerRadiusOffset = (float)OuterRadiusOffset;
        minDimension -= outerRadiusOffset;

        var view = (IPieChartView<TDrawingContext>)pieChart.View;
        var initialRotation = (float)Math.Truncate(view.InitialRotation);
        var completeAngle = (float)view.MaxAngle;

        var startValue = (float)view.MinValue;
        var chartTotal = (float?)view.MaxValue;

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        if (Fill is not null)
        {
            Fill.ZIndex = actualZIndex + 0.1;
            Fill.SetClipRectangle(pieChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            pieChart.Canvas.AddDrawableTask(Fill);
        }
        if (Stroke is not null)
        {
            Stroke.ZIndex = actualZIndex + 0.2;
            Stroke.SetClipRectangle(pieChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            pieChart.Canvas.AddDrawableTask(Stroke);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = 1000 + actualZIndex + 0.3;
            // this does not require clipping...
            //DataLabelsPaint.SetClipRectangle(pieChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            pieChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
        var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

        var dls = (float)DataLabelsSize;
        var stacker = pieChart.SeriesContext.GetStackPosition(this, GetStackGroup())
            ?? throw new NullReferenceException("Unexpected null stacker");
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

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

        var r = (float)DataLabelsRotation;
        var isTangent = false;
        var isCotangent = false;

        if (((int)r & LiveCharts.TangentAngle) != 0)
        {
            r -= LiveCharts.TangentAngle;
            isTangent = true;
        }

        if (((int)r & LiveCharts.CotangentAngle) != 0)
        {
            r -= LiveCharts.CotangentAngle;
            isCotangent = true;
        }

        var i = 1f;
        var isClockWise = view.IsClockwise;

        var isFirstDraw = !chart._drawnSeries.Contains(((ISeries)this).SeriesId);

        foreach (var point in fetched)
        {
            var coordinate = point.Coordinate;
            var visual = point.Context.Visual as TVisual;

            if (point.IsEmpty)
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

                var md2 = minDimension;
                var w2 = md2 - (md2 - 2 * innerRadius) * (fetched.Length - i) / fetched.Length - relativeOuterRadius * 2;
                stackedInnerRadius = (w2 + relativeOuterRadius * 2) * 0.5f;
                i++;
                continue;
            }

            var stack = stacker.GetStack(point);
            var stackedValue = stack.Start;
            var total = chartTotal ?? stack.Total;

            double start, sweep;

            if (total == 0)
            {
                start = 0;
                sweep = 0;
            }
            else
            {
                if (_isRelativeToMin)
                {
                    // when the series is relative to min value,
                    // the start value is always the PieChart.MinValue
                    // this is normally used on angular gauge series.
                    var h = total - startValue;
                    var h1 = stackedValue + coordinate.PrimaryValue;
                    start = stackedValue / (total - startValue) * completeAngle;
                    sweep = h1 / h * completeAngle - start;
                    if (!isClockWise) start = completeAngle - start - sweep;
                }
                else
                {
                    var h = total - startValue;
                    var h1 = stackedValue + coordinate.PrimaryValue - startValue;
                    start = stackedValue / total * completeAngle;
                    sweep = h1 / h * completeAngle - start;
                    if (!isClockWise) start = completeAngle - start - sweep;
                }
            }

            if (IsFillSeries)
            {
                start = 0;
                sweep = completeAngle - 0.1f;
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
                    StartAngle = (float)(isFirstDraw ? initialRotation : start + initialRotation),
                    SweepAngle = 0,
                    PushOut = 0,
                    InnerRadius = 0,
                    CornerRadius = 0
                };

                visual = p;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            Fill?.AddGeometryToPaintTask(pieChart.Canvas, visual);
            Stroke?.AddGeometryToPaintTask(pieChart.Canvas, visual);

            var dougnutGeometry = visual;

            stackedInnerRadius += relativeInnerRadius;

            var md = minDimension;
            var stackedOuterRadius = md - (md - 2 * innerRadius) * (fetched.Length - i) / fetched.Length - relativeOuterRadius * 2;

            dougnutGeometry.CenterX = cx;
            dougnutGeometry.CenterY = cy;
            dougnutGeometry.X = drawLocation.X + (drawMarginSize.Width - stackedOuterRadius) * 0.5f;
            dougnutGeometry.Y = drawLocation.Y + (drawMarginSize.Height - stackedOuterRadius) * 0.5f;
            dougnutGeometry.Width = stackedOuterRadius;
            dougnutGeometry.Height = stackedOuterRadius;
            dougnutGeometry.InnerRadius = stackedInnerRadius;
            dougnutGeometry.PushOut = pushout;
            dougnutGeometry.StartAngle = (float)(start + initialRotation);
            dougnutGeometry.SweepAngle = (float)sweep;
            dougnutGeometry.CornerRadius = cornerRadius;
            dougnutGeometry.InvertedCornerRadius = InvertedCornerRadius;
            dougnutGeometry.RemoveOnCompleted = false;

            if (start + initialRotation == initialRotation && sweep == 360)
                dougnutGeometry.SweepAngle = 359.99f;

            if (point.Context.HoverArea is not SemicircleHoverArea ha)
                point.Context.HoverArea = ha = new SemicircleHoverArea();

            _ = ha.SetDimensions(
                cx, cy, (float)(start + initialRotation), (float)(start + initialRotation + sweep),
                stackedInnerRadius, stackedOuterRadius);

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                var middleAngle = (float)(start + initialRotation + sweep * 0.5);

                var actualRotation = r +
                        (isTangent ? middleAngle - 90 : 0) +
                        (isCotangent ? middleAngle : 0);

                if ((isTangent || isCotangent) && ((actualRotation + 90) % 360) > 180)
                    actualRotation += 180;

                if (label is null)
                {
                    var l = new TLabel { X = cx, Y = cy, RotateTransform = actualRotation, MaxWidth = (float)DataLabelsMaxWidth };
                    l.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(pieChart.Canvas, label);

                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;
                label.RotateTransform = actualRotation;

                AlignLabel(label, (float)start, initialRotation, sweep);

                if (isFirstDraw)
                    label.CompleteTransition(
                        nameof(label.TextSize), nameof(label.X), nameof(label.Y), nameof(label.RotateTransform));

                var labelPosition = GetLabelPolarPosition(
                    cx,
                    cy,
                    stackedInnerRadius,
                    (stackedOuterRadius + relativeOuterRadius * 2) * 0.5f,
                    (float)(start + initialRotation),
                    (float)sweep,
                    label.Measure(DataLabelsPaint),
                    DataLabelsPosition);

                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);

            stackedInnerRadius = (stackedOuterRadius + relativeOuterRadius * 2) * 0.5f;
            i++;
        }

        var u = new Scaler(); // dummy scaler, this is not used in the SoftDeleteOrDisposePoint method.
        pointsCleanup.CollectPoints(everFetched, pieChart.View, u, u, SoftDeleteOrDisposePoint);
    }

    /// <inheritdoc cref="IPieSeries{TDrawingContext}.GetBounds(PieChart{TDrawingContext})"/>
    public virtual DimensionalBounds GetBounds(PieChart<TDrawingContext> chart)
    {
        return DataFactory.GetPieBounds(chart, this).Bounds;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniaturesSketch"/>
    public override Sketch<TDrawingContext> GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TMiniatureGeometry()));
        if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TMiniatureGeometry()));

        return new Sketch<TDrawingContext>(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="ISeries.GetPrimaryToolTipText(ChartPoint)"/>
    public override string? GetPrimaryToolTipText(ChartPoint point)
    {
        return ToolTipLabelFormatter is not null
            ? ToolTipLabelFormatter(new ChartPoint<TModel, TVisual, TLabel>(point))
            : point.Coordinate.PrimaryValue.ToString();
    }

    /// <inheritdoc cref="ISeries.GetSecondaryToolTipText(ChartPoint)"/>
    public override string? GetSecondaryToolTipText(ChartPoint point)
    {
        return LiveCharts.IgnoreToolTipLabel;
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
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _fill, _stroke, DataLabelsPaint };
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerEnter(ChartPoint)"/>
    protected override void OnPointerEnter(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.PushOut = (float)HoverPushout;
        visual.Opacity = 0.8f;

        base.OnPointerEnter(point);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerLeft(ChartPoint)"/>
    protected override void OnPointerLeft(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.PushOut = (float)Pushout;
        visual.Opacity = 1;

        base.OnPointerLeft(point);
    }

    /// <summary>
    /// Sets the default point transitions.
    /// </summary>
    /// <param name="chartPoint">The chart point.</param>
    /// <exception cref="Exception">Unable to initialize the point instance.</exception>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        if (IsFillSeries) return;
        var chart = chartPoint.Context.Chart;
        if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

        if ((SeriesProperties & SeriesProperties.Gauge) == 0)
            visual.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
        else
            visual.Animate(
                EasingFunction ?? chart.EasingFunction,
                AnimationsSpeed ?? chart.AnimationsSpeed,
                nameof(visual.StartAngle),
                nameof(visual.SweepAngle));
    }

    /// <summary>
    /// Softs the delete point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="primaryScale">The primary scale.</param>
    /// <param name="secondaryScale">The secondary scale.</param>
    protected virtual void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

        visual.StartAngle += visual.SweepAngle;
        visual.SweepAngle = 0;
        visual.CornerRadius = 0;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

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
    /// <param name="innerRadius">The iner radius.</param>
    /// <param name="outerRadius">The outer radius.</param>
    /// <param name="startAngle">The start angle.</param>
    /// <param name="sweepAngle">The sweep angle.</param>
    /// <param name="labelSize">Size of the label.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    protected virtual LvcPoint GetLabelPolarPosition(
        float centerX,
        float centerY,
        float innerRadius,
        float outerRadius,
        float startAngle,
        float sweepAngle,
        LvcSize labelSize,
        PolarLabelsPosition position)
    {
        float angle = 0, radius = 0;

        switch (position)
        {
            case PolarLabelsPosition.End:
                angle = startAngle + sweepAngle;
                radius = innerRadius + (outerRadius - innerRadius) * 0.5f;
                break;
            case PolarLabelsPosition.Start:
                angle = startAngle;
                radius = innerRadius + (outerRadius - innerRadius) * 0.5f;
                break;
            case PolarLabelsPosition.Outer:
                angle = startAngle + sweepAngle * 0.5f;
                radius = outerRadius + 0.45f * (float)Math.Sqrt(Math.Pow(labelSize.Width, 2) + Math.Pow(labelSize.Height, 2));
                break;
            case PolarLabelsPosition.Middle:
                var f = (SeriesProperties & SeriesProperties.Gauge) != 0 ? 0.5f : 0.65f;
                angle = startAngle + sweepAngle * 0.5f;
                radius = innerRadius + (outerRadius - innerRadius) * f;
                break;
            case PolarLabelsPosition.ChartCenter:
                return new LvcPoint(centerX, centerY);
            default:
                break;
        }

        const float toRadians = (float)(Math.PI / 180);
        angle *= toRadians;

        return new LvcPoint(
             (float)(centerX + Math.Cos(angle) * radius),
             (float)(centerY + Math.Sin(angle) * radius));
    }

    /// <summary>
    /// Softly deletes the all points from the chart.
    /// </summary>
    /// <param name="chart"></param>
    /// <inheritdoc cref="ISeries.SoftDeleteOrDispose" />
    public override void SoftDeleteOrDispose(IChartView chart)
    {
        var core = ((IPieChartView<TDrawingContext>)chart).Core;
        var u = new Scaler();

        var toDelete = new List<ChartPoint>();
        foreach (var point in everFetched)
        {
            if (point.Context.Chart != chart) continue;
            SoftDeleteOrDisposePoint(point, u, u);
            toDelete.Add(point);
        }

        foreach (var pt in GetPaintTasks())
        {
            if (pt is not null) core.Canvas.RemovePaintTask(pt);
        }

        foreach (var item in toDelete) _ = everFetched.Remove(item);

        OnVisibilityChanged();
    }

    private void AlignLabel(TLabel label, double start, double initialRotation, double sweep)
    {
        switch (DataLabelsPosition)
        {
            case PolarLabelsPosition.Middle:
            case PolarLabelsPosition.ChartCenter:
            case PolarLabelsPosition.Outer:
                label.HorizontalAlign = Align.Middle;
                label.VerticalAlign = Align.Middle;
                break;
            case PolarLabelsPosition.End:
                var a = start + initialRotation + sweep;
                a %= 360;
                if (a < 0) a += 360;
                var c = 90;
                if (a > 180) c = -90;
                label.HorizontalAlign = a > 180 ? Align.Start : Align.End;
                label.RotateTransform = (float)(a - c);
                break;
            case PolarLabelsPosition.Start:
                var a1 = start + initialRotation;
                a1 %= 360;
                if (a1 < 0) a1 += 360;
                var c1 = 90;
                if (a1 > 180) c1 = -90;
                label.HorizontalAlign = a1 > 180 ? Align.End : Align.Start;
                label.RotateTransform = (float)(a1 - c1);
                break;
            default:
                break;
        }
    }
}
