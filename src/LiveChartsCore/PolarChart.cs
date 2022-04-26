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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

namespace LiveChartsCore;

/// <summary>
/// Defines a Polar chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="Chart{TDrawingContext}" />
public class PolarChart<TDrawingContext> : Chart<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    internal readonly HashSet<ISeries> _everMeasuredSeries = new();
    internal readonly HashSet<IPlane<TDrawingContext>> _everMeasuredAxes = new();
    internal readonly HashSet<Section<TDrawingContext>> _everMeasuredSections = new();
    private readonly IPolarChartView<TDrawingContext> _chartView;
    private int _nextSeries = 0;
    private readonly bool _requiresLegendMeasureAlways = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="defaultPlatformConfig">The default platform configuration.</param>
    /// <param name="canvas">The canvas.</param>
    /// <param name="requiresLegendMeasureAlways">Forces the legends to redraw with every measure request.</param>
    public PolarChart(
        IPolarChartView<TDrawingContext> view,
        Action<LiveChartsSettings> defaultPlatformConfig,
        MotionCanvas<TDrawingContext> canvas,
        bool requiresLegendMeasureAlways = false)
        : base(canvas, defaultPlatformConfig, view)
    {
        _chartView = view;
        _requiresLegendMeasureAlways = requiresLegendMeasureAlways;
    }

    /// <summary>
    /// Gets the angle axes.
    /// </summary>
    /// <value>
    /// The x axes.
    /// </value>
    public IPolarAxis[] AngleAxes { get; private set; } = Array.Empty<IPolarAxis>();

    /// <summary>
    /// Gets the radius axes.
    /// </summary>
    /// <value>
    /// The y axes.
    /// </value>
    public IPolarAxis[] RadiusAxes { get; private set; } = Array.Empty<IPolarAxis>();

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public IPolarSeries<TDrawingContext>[] Series { get; private set; } = Array.Empty<IPolarSeries<TDrawingContext>>();

    /// <summary>
    /// Gets whether the series fit to bounds or not.
    /// </summary>
    public bool FitToBounds { get; private set; }

    /// <summary>
    /// Gets the total circumference angle.
    /// </summary>
    /// <value>
    /// The total angle.
    /// </value>
    public float TotalAnge { get; private set; }

    /// <summary>
    /// Gets the Inner radius.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    public float InnerRadius { get; private set; }

    /// <summary>
    /// Gets the Initial rotation.
    /// </summary>
    /// /// <value>
    /// The inner radius.
    /// </value>
    public float InitialRotation { get; private set; }

    /// <summary>
    /// Gets the drawable series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public override IEnumerable<IChartSeries<TDrawingContext>> ChartSeries => Series;

    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public override IChartView<TDrawingContext> View => _chartView;

    /// <summary>
    /// Finds the points near to the specified point.
    /// </summary>
    /// <param name="pointerPosition">The pointer position.</param>
    /// <returns></returns>
    public override IEnumerable<ChartPoint> FindHoveredPointsBy(LvcPoint pointerPosition)
    {
        return ChartSeries
            .Where(series => series.IsHoverable)
            .SelectMany(series => series.FindHoveredPoints(this, pointerPosition, TooltipFindingStrategy.CompareAll));
    }

    /// <summary>
    /// Measures this chart.
    /// </summary>
    /// <returns></returns>
    protected override void Measure()
    {
#if DEBUG
        if (LiveCharts.EnableLogging)
        {
            Trace.WriteLine(
                $"[Cartesian chart measured]".PadRight(60) +
                $"tread: {Environment.CurrentManagedThreadId}");
        }
#endif
        if (!IsLoaded) return; // <- prevents a visual glitch where the visual call the measure method
                               // while they are not visible, the problem is when the control is visible again
                               // the animations are not as expected because previously it ran in an invalid case.

        InvokeOnMeasuring();

        if (preserveFirstDraw)
        {
            IsFirstDraw = true;
            preserveFirstDraw = false;
        }

        MeasureWork = new object();

        #region copy the current data in the view

        var viewDrawMargin = _chartView.DrawMargin;
        ControlSize = _chartView.ControlSize;

        AngleAxes = _chartView.AngleAxes.Cast<IPolarAxis>().Select(x => x).ToArray();
        RadiusAxes = _chartView.RadiusAxes.Cast<IPolarAxis>().Select(x => x).ToArray();

        var theme = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
        if (theme.CurrentColors is null || theme.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        var forceApply = ThemeId != LiveCharts.CurrentSettings.ThemeId && !IsFirstDraw;

        LegendPosition = _chartView.LegendPosition;
        LegendOrientation = _chartView.LegendOrientation;
        Legend = _chartView.Legend;

        TooltipPosition = _chartView.TooltipPosition;
        Tooltip = _chartView.Tooltip;

        AnimationsSpeed = _chartView.AnimationsSpeed;
        EasingFunction = _chartView.EasingFunction;

        FitToBounds = _chartView.FitToBounds;
        TotalAnge = (float)_chartView.TotalAngle;
        InnerRadius = (float)_chartView.InnerRadius;
        InitialRotation = (float)_chartView.InitialRotation;

        var actualSeries = (_chartView.Series ?? Enumerable.Empty<ISeries>()).Where(x => x.IsVisible);

        Series = actualSeries
            .Cast<IPolarSeries<TDrawingContext>>()
            .ToArray();

        #endregion

        SeriesContext = new SeriesContext<TDrawingContext>(Series);

        // restart axes bounds and meta data
        foreach (var axis in AngleAxes)
        {
            axis.IsNotifyingChanges = false;
            axis.Initialize(PolarAxisOrientation.Angle);
            theme.ResolveAxisDefaults((IPlane<TDrawingContext>)axis, forceApply);
            axis.IsNotifyingChanges = true;
        }
        foreach (var axis in RadiusAxes)
        {
            axis.IsNotifyingChanges = false;
            axis.Initialize(PolarAxisOrientation.Radius);
            theme.ResolveAxisDefaults((IPlane<TDrawingContext>)axis, forceApply);
            axis.IsNotifyingChanges = true;
        }

        // get seriesBounds
        SetDrawMargin(ControlSize, new Margin());
        foreach (var series in Series)
        {
            series.IsNotifyingChanges = false;
            if (series.SeriesId == -1) series.SeriesId = _nextSeries++;
            theme.ResolveSeriesDefaults(theme.CurrentColors, series, forceApply);

            var secondaryAxis = AngleAxes[series.ScalesAngleAt];
            var primaryAxis = RadiusAxes[series.ScalesRadiusAt];

            var seriesBounds = series.GetBounds(this, secondaryAxis, primaryAxis).Bounds;

            if (seriesBounds.IsEmpty) continue;

            secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds);
            primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds);
            secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.SecondaryBounds);
            primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.PrimaryBounds);

            series.IsNotifyingChanges = true;
        }

        #region empty bounds

        // prevent the bounds are not empty...

        foreach (var axis in AngleAxes)
        {
            axis.IsNotifyingChanges = false;
            if (!axis.DataBounds.IsEmpty)
            {
                axis.IsNotifyingChanges = true;
                continue;
            }

            var min = 0;
            var max = 10d * axis.UnitWidth;

            axis.DataBounds.AppendValue(max);
            axis.DataBounds.AppendValue(min);
            axis.VisibleDataBounds.AppendValue(max);
            axis.VisibleDataBounds.AppendValue(min);

            if (axis.DataBounds.MinDelta < max) axis.DataBounds.MinDelta = max;

            axis.IsNotifyingChanges = true;
        }
        foreach (var axis in RadiusAxes)
        {
            axis.IsNotifyingChanges = false;
            if (!axis.DataBounds.IsEmpty)
            {
                axis.IsNotifyingChanges = true;
                continue;
            }

            var min = 0;
            var max = 10d * axis.UnitWidth;

            axis.DataBounds.AppendValue(max);
            axis.DataBounds.AppendValue(min);
            axis.VisibleDataBounds.AppendValue(max);
            axis.VisibleDataBounds.AppendValue(min);

            if (axis.DataBounds.MinDelta < max) axis.DataBounds.MinDelta = max;

            axis.IsNotifyingChanges = true;
        }

        #endregion

        if (Legend is not null && (SeriesMiniatureChanged(Series, LegendPosition) || (_requiresLegendMeasureAlways && SizeChanged())))
        {
            Legend.Draw(this);
            Update();
            PreviousLegendPosition = LegendPosition;
            PreviousSeries = Series;
            preserveFirstDraw = IsFirstDraw;
        }

        // calculate draw margin

        if (FitToBounds)
        {
            float mt = 0, mb = 0, ml = 0, mr = 0;

            foreach (var series in Series)
            {
                var scaler = new PolarScaler(
                    DrawMarginLocation, DrawMarginSize, AngleAxes[series.ScalesAngleAt], RadiusAxes[series.ScalesRadiusAt],
                    InnerRadius, InitialRotation, TotalAnge);

                foreach (var point in series.Fetch(this))
                {
                    var p = scaler.ToPixels(point);

                    var dx = p.X - scaler.CenterX;
                    var dy = p.Y - scaler.CenterY;

                    if (dx > 0)
                    {
                        if (dx > mr)
                            mr = dx;
                    }
                    else
                    {
                        dx *= -1;
                        if (dx > ml)
                            ml = dx;
                    }

                    if (dy > 0)
                    {
                        if (dy > mb)
                            mb = dy;
                    }
                    else
                    {
                        dy *= -1;
                        if (dy > mt)
                            mt = dy;
                    }
                }
            }

            var cs = ControlSize;
            var cx = cs.Width * 0.5f;
            var cy = cs.Height * 0.5f;

            var dl = cx - ml;
            var dr = cx - mr;
            var dt = cy - mt;
            var db = cy - mb;

            // so the idea is...

            // we know the distance of the most left point to the left border (dl)
            // the most right point to the right border (dr)
            // the most bottom point to the bottom border (db)
            // the most top point to the top border (dt)

            // then to "easily" fit the plot to the data bounds, we create a negative margin for our draw margin
            // then the scaler will luckily handle it.

            var fitMargin = new Margin(-dl, -dt, -dr, -db);
            SetDrawMargin(ControlSize, fitMargin);
        }
        else if (viewDrawMargin is null)
        {
            var m = viewDrawMargin ?? new Margin();
            SetDrawMargin(ControlSize, m);

            foreach (var axis in AngleAxes)
            {
                if (!axis.IsVisible) continue;

                if (axis.DataBounds.Max == axis.DataBounds.Min)
                {
                    var c = axis.DataBounds.Min * 0.3;
                    axis.DataBounds.Min = axis.DataBounds.Min - c;
                    axis.DataBounds.Max = axis.DataBounds.Max + c;
                    axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - c;
                    axis.VisibleDataBounds.Max = axis.VisibleDataBounds.Max + c;
                }

                var drawablePlane = (IPlane<TDrawingContext>)axis;
                var ns = drawablePlane.GetNameLabelSize(this);
                var s = drawablePlane.GetPossibleSize(this);

                var radius = s.Height; // <- this type needs to be changed... it is not the height it is the radius.

                axis.Ro = m.Top + radius;

                m.Top += radius;
                m.Bottom += radius;
                m.Left += radius;
                m.Right += radius;
            }
            foreach (var axis in RadiusAxes)
            {
                if (!axis.IsVisible) continue;

                if (axis.DataBounds.Max == axis.DataBounds.Min)
                {
                    var c = axis.DataBounds.Min * 0.3;
                    axis.DataBounds.Min = axis.DataBounds.Min - c;
                    axis.DataBounds.Max = axis.DataBounds.Max + c;
                    axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - c;
                    axis.VisibleDataBounds.Max = axis.VisibleDataBounds.Max + c;
                }

                // the angle axis does not require padding?? I think it does not
            }

            SetDrawMargin(ControlSize, m);
        }

        // invalid dimensions, probably the chart is too small
        // or it is initializing in the UI and has no dimensions yet
        if (DrawMarginSize.Width <= 0 || DrawMarginSize.Height <= 0) return;

        var totalAxes = RadiusAxes.Concat(AngleAxes).ToArray();
        var toDeleteAxes = new HashSet<IPlane<TDrawingContext>>(_everMeasuredAxes);
        foreach (var axis in totalAxes)
        {
            if (axis.DataBounds.Max == axis.DataBounds.Min)
            {
                var c = axis.DataBounds.Min * 0.3;
                axis.DataBounds.Min = axis.DataBounds.Min - c;
                axis.DataBounds.Max = axis.DataBounds.Max + c;
            }

            // apply padding
            if (axis.MinLimit is null)
            {
                // correction by geometry size
                var p = 0d;
                if (axis.DataBounds.PaddingMin > p) p = axis.DataBounds.PaddingMin;
                axis.IsNotifyingChanges = false;
                axis.DataBounds.Min = axis.DataBounds.Min - p;
                axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - p;
                axis.IsNotifyingChanges = true;
            }

            // apply padding
            if (axis.MaxLimit is null)
            {
                // correction by geometry size
                var p = 0d; // Math.Abs(s.ToChartValues(axis.DataBounds.RequestedGeometrySize) - s.ToChartValues(0));
                if (axis.DataBounds.PaddingMax > p) p = axis.DataBounds.PaddingMax;
                axis.IsNotifyingChanges = false;
                axis.DataBounds.Max = axis.DataBounds.Max + p;
                axis.VisibleDataBounds.Max = axis.VisibleDataBounds.Max + p;
                axis.IsNotifyingChanges = true;
            }

            var drawablePlane = (IPlane<TDrawingContext>)axis;
            _ = _everMeasuredAxes.Add(drawablePlane);
            if (drawablePlane.IsVisible)
            {
                drawablePlane.Measure(this);
                _ = toDeleteAxes.Remove(drawablePlane);
            }

            drawablePlane.RemoveOldPaints(View);
        }

        var toDeleteSeries = new HashSet<ISeries>(_everMeasuredSeries);
        foreach (var series in Series)
        {
            series.Measure(this);
            series.RemoveOldPaints(View);
            _ = _everMeasuredSeries.Add(series);
            _ = toDeleteSeries.Remove(series);
        }

        foreach (var series in toDeleteSeries)
        {
            series.SoftDeleteOrDispose(View);
            _ = _everMeasuredSeries.Remove(series);
        }
        foreach (var axis in toDeleteAxes)
        {
            axis.RemoveFromUI(this);
            _ = _everMeasuredAxes.Remove(axis);
        }

        foreach (var axis in totalAxes)
        {
            axis.IsNotifyingChanges = false;
            axis.ActualBounds.HasPreviousState = true;
            axis.IsNotifyingChanges = true;
        }

        InvokeOnUpdateStarted();

        IsFirstDraw = false;
        ThemeId = LiveCharts.CurrentSettings.ThemeId;
        PreviousSeries = Series;
        PreviousLegendPosition = LegendPosition;

        Canvas.Invalidate();
    }

    /// <summary>
    /// Scales the specified point to the UI.
    /// </summary>
    /// <param name="point">The point, where X = angle, Y = radius.</param>
    /// <param name="angleAxisIndex">Index of the angle axis.</param>
    /// <param name="radiusAxisIndex">Index of the radius axis.</param>
    /// <returns></returns>
    public double[] ScaleUIPoint(LvcPoint point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        var angleAxis = AngleAxes[angleAxisIndex];
        var radiusAxis = RadiusAxes[radiusAxisIndex];

        var scaler = new PolarScaler(
            DrawMarginLocation, DrawMarginSize, angleAxis, radiusAxis,
            InnerRadius, InitialRotation, TotalAnge);

        var r = scaler.ToChartValues(point.X, point.Y);

        return new double[] { r.X, r.Y };
    }

    /// <inheritdoc cref="Chart{TDrawingContext}.Unload"/>
    public override void Unload()
    {
        base.Unload();

        foreach (var item in _everMeasuredAxes) item.RemoveFromUI(this);
        _everMeasuredAxes.Clear();
        foreach (var item in _everMeasuredSections) item.RemoveFromUI(this);
        _everMeasuredSections.Clear();
        foreach (var item in _everMeasuredSeries) ((ChartElement<TDrawingContext>)item).RemoveFromUI(this);
        _everMeasuredSeries.Clear();
        IsFirstDraw = true;
    }
}
