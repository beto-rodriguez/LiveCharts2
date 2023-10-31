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
    private readonly IPolarChartView<TDrawingContext> _chartView;
    private int _nextSeries = 0;

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

    ///<inheritdoc cref="Chart{TDrawingContext}.Series"/>
    public override IEnumerable<IChartSeries<TDrawingContext>> Series =>
        _chartView.Series.Cast<IChartSeries<TDrawingContext>>();

    ///<inheritdoc cref="Chart{TDrawingContext}.VisibleSeries"/>
    public override IEnumerable<IChartSeries<TDrawingContext>> VisibleSeries =>
        Series.Where(x => x.IsVisible);

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
        return VisibleSeries
            .Where(series => series.IsHoverable)
            .SelectMany(series => series.FindHitPoints(this, pointerPosition, TooltipFindingStrategy.CompareAll));
    }

    /// <summary>
    /// Measures this chart.
    /// </summary>
    /// <returns></returns>
    protected internal override void Measure()
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

        if (_preserveFirstDraw)
        {
            _isFirstDraw = true;
            _preserveFirstDraw = false;
        }

        MeasureWork = new object();

        #region copy the current data in the view

        var viewDrawMargin = _chartView.DrawMargin;
        ControlSize = _chartView.ControlSize;

        AngleAxes = _chartView.AngleAxes.Cast<IPolarAxis>().ToArray();
        RadiusAxes = _chartView.RadiusAxes.Cast<IPolarAxis>().ToArray();

        var theme = LiveCharts.DefaultSettings.GetTheme<TDrawingContext>();

        LegendPosition = _chartView.LegendPosition;
        Legend = _chartView.Legend;

        TooltipPosition = _chartView.TooltipPosition;
        Tooltip = _chartView.Tooltip;

        AnimationsSpeed = _chartView.AnimationsSpeed;
        EasingFunction = _chartView.EasingFunction;

        FitToBounds = _chartView.FitToBounds;
        TotalAnge = (float)_chartView.TotalAngle;
        InnerRadius = (float)_chartView.InnerRadius;
        InitialRotation = (float)_chartView.InitialRotation;

        VisualElements = _chartView.VisualElements ?? Array.Empty<ChartElement<TDrawingContext>>();

        #endregion

        SeriesContext = new SeriesContext<TDrawingContext>(VisibleSeries, this);
        var isNewTheme = LiveCharts.DefaultSettings.CurrentThemeId != ThemeId;

        // restart axes bounds and meta data
        foreach (var axis in AngleAxes)
        {
            var ce = (ChartElement<TDrawingContext>)axis;
            ce._isInternalSet = true;
            axis.Initialize(PolarAxisOrientation.Angle);
            if (!ce._isThemeSet || isNewTheme)
            {
                theme.ApplyStyleToAxis((IPlane<TDrawingContext>)axis);
                ce._isThemeSet = true;
            }
            ce._isInternalSet = false;
        }
        foreach (var axis in RadiusAxes)
        {
            var ce = (ChartElement<TDrawingContext>)axis;
            ce._isInternalSet = true;
            axis.Initialize(PolarAxisOrientation.Radius);
            if (!ce._isThemeSet || isNewTheme)
            {
                theme.ApplyStyleToAxis((IPlane<TDrawingContext>)axis);
                ce._isThemeSet = true;
            }
            ce._isInternalSet = false;
        }

        // get seriesBounds
        SetDrawMargin(ControlSize, new Margin());
        foreach (var series in VisibleSeries.Cast<IPolarSeries<TDrawingContext>>())
        {
            if (series.SeriesId == -1) series.SeriesId = _nextSeries++;

            var ce = (ChartElement<TDrawingContext>)series;
            ce._isInternalSet = true;
            if (!ce._isThemeSet || isNewTheme)
            {
                theme.ApplyStyleToSeries(series);
                ce._isThemeSet = true;
            }

            var secondaryAxis = AngleAxes[series.ScalesAngleAt];
            var primaryAxis = RadiusAxes[series.ScalesRadiusAt];

            var seriesBounds = series.GetBounds(this, secondaryAxis, primaryAxis).Bounds;

            if (seriesBounds.IsEmpty) continue;

            secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds);
            primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds);
            secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.SecondaryBounds);
            primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.PrimaryBounds);
        }

        #region empty bounds

        // prevent the bounds are not empty...

        foreach (var axis in AngleAxes)
        {
            var ce = (ChartElement<TDrawingContext>)axis;
            ce._isInternalSet = true;

            if (!axis.DataBounds.IsEmpty)
            {
                ce._isInternalSet = false;
                continue;
            }

            var min = 0;
            var max = 10d * axis.UnitWidth;

            axis.DataBounds.AppendValue(max);
            axis.DataBounds.AppendValue(min);
            axis.VisibleDataBounds.AppendValue(max);
            axis.VisibleDataBounds.AppendValue(min);

            if (axis.DataBounds.MinDelta < max) axis.DataBounds.MinDelta = max;

            ce._isInternalSet = false;
        }
        foreach (var axis in RadiusAxes)
        {
            var ce = (ChartElement<TDrawingContext>)axis;
            ce._isInternalSet = true;

            if (!axis.DataBounds.IsEmpty)
            {
                ce._isInternalSet = false;
                continue;
            }

            var min = 0;
            var max = 10d * axis.UnitWidth;

            axis.DataBounds.AppendValue(max);
            axis.DataBounds.AppendValue(min);
            axis.VisibleDataBounds.AppendValue(max);
            axis.VisibleDataBounds.AppendValue(min);

            if (axis.DataBounds.MinDelta < max) axis.DataBounds.MinDelta = max;

            ce._isInternalSet = false;
        }

        #endregion

        InitializeVisualsCollector();

        if (FitToBounds)
        {
            float mt = 0, mb = 0, ml = 0, mr = 0;

            foreach (var series in VisibleSeries.Cast<IPolarSeries<TDrawingContext>>())
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
        else
        {
            // calculate draw margin
            var m = new Margin();
            float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
            if (View.Title is not null)
            {
                View.Title.ClippingMode = ClipMode.None;
                var titleSize = View.Title.Measure(this);
                m.Top = titleSize.Height;
                ts = titleSize.Height;
                _titleHeight = titleSize.Height;
            }

            DrawLegend(ref ts, ref bs, ref ls, ref rs);

            m.Top = ts;
            m.Bottom = bs;
            m.Left = ls;
            m.Right = rs;

            foreach (var axis in AngleAxes)
            {
                if (!axis.IsVisible) continue;

                if (axis.DataBounds.Max == axis.DataBounds.Min)
                {
                    var c = axis.UnitWidth * 0.5;
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
                    var c = axis.UnitWidth * 0.5;
                    axis.DataBounds.Min = axis.DataBounds.Min - c;
                    axis.DataBounds.Max = axis.DataBounds.Max + c;
                    axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - c;
                    axis.VisibleDataBounds.Max = axis.VisibleDataBounds.Max + c;
                }

                // the angle axis does not require padding?? I think it does not
            }

            var rm = viewDrawMargin ?? new Margin(Margin.Auto);
            var actualMargin = new Margin(
                Margin.IsAuto(rm.Left) ? m.Left : rm.Left,
                Margin.IsAuto(rm.Top) ? m.Top : rm.Top,
                Margin.IsAuto(rm.Right) ? m.Right : rm.Right,
                Margin.IsAuto(rm.Bottom) ? m.Bottom : rm.Bottom);

            SetDrawMargin(ControlSize, m);
        }

        // invalid dimensions, probably the chart is too small
        // or it is initializing in the UI and has no dimensions yet
        if (DrawMarginSize.Width <= 0 || DrawMarginSize.Height <= 0) return;

        UpdateBounds();

        var title = View.Title;
        if (title is not null)
        {
            var titleSize = title.Measure(this);
            title.AlignToTopLeftCorner();
            title.X = ControlSize.Width * 0.5f - titleSize.Width * 0.5f;
            title.Y = 0;
            AddVisual(title);
        }

        var totalAxes = RadiusAxes.Concat(AngleAxes).ToArray();
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
                var ce = (ChartElement<TDrawingContext>)axis;
                ce._isInternalSet = true;
                axis.DataBounds.Min = axis.DataBounds.Min - p;
                axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - p;
                ce._isInternalSet = false;
            }

            // apply padding
            if (axis.MaxLimit is null)
            {
                // correction by geometry size
                var p = 0d; // Math.Abs(s.ToChartValues(axis.DataBounds.RequestedGeometrySize) - s.ToChartValues(0));
                if (axis.DataBounds.PaddingMax > p) p = axis.DataBounds.PaddingMax;
                var ce = (ChartElement<TDrawingContext>)axis;
                ce._isInternalSet = true;
                axis.DataBounds.Max = axis.DataBounds.Max + p;
                axis.VisibleDataBounds.Max = axis.VisibleDataBounds.Max + p;
                ce._isInternalSet = false;
            }

            if (axis.IsVisible) AddVisual((ChartElement<TDrawingContext>)axis);
            ((ChartElement<TDrawingContext>)axis).RemoveOldPaints(View); // <- this is probably obsolete.
            // the probable issue is the "IsVisible" property
        }

        foreach (var visual in VisualElements) AddVisual(visual);
        foreach (var series in VisibleSeries)
        {
            AddVisual((ChartElement<TDrawingContext>)series);
            _drawnSeries.Add(series.SeriesId);
        }

        CollectVisuals();

        foreach (var axis in totalAxes)
        {
            if (!axis.IsVisible) continue;

            var ce = (ChartElement<TDrawingContext>)axis;
            ce._isInternalSet = true;
            axis.ActualBounds.HasPreviousState = true;
            ce._isInternalSet = false;
        }

        InvokeOnUpdateStarted();

        if (_isToolTipOpen) DrawToolTip();
        _isFirstDraw = false;
        ThemeId = LiveCharts.DefaultSettings.CurrentThemeId;

        Canvas.Invalidate();
        _isFirstDraw = false;
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
        _isFirstDraw = true;
    }
}
