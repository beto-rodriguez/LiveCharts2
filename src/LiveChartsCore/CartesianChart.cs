﻿// The MIT License(MIT)
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
/// Defines a Cartesian chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="Chart{TDrawingContext}" />
public class CartesianChart<TDrawingContext> : Chart<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    internal readonly HashSet<ISeries> _everMeasuredSeries = new();
    internal readonly HashSet<IPlane<TDrawingContext>> _everMeasuredAxes = new();
    internal readonly HashSet<Section<TDrawingContext>> _everMeasuredSections = new();
    private readonly ICartesianChartView<TDrawingContext> _chartView;
    private int _nextSeries = 0;
    private double _zoomingSpeed = 0;
    private readonly bool _requiresLegendMeasureAlways = false;
    private ZoomAndPanMode _zoomMode;
    private DrawMarginFrame<TDrawingContext>? _previousDrawMarginFrame;
    private const double MaxAxisBound = 0.05;
    private const double MaxAxisActiveBound = 0.15;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="defaultPlatformConfig">The default platform configuration.</param>
    /// <param name="canvas">The canvas.</param>
    /// <param name="requiresLegendMeasureAlways">Forces the legends to redraw with every measure request.</param>
    public CartesianChart(
        ICartesianChartView<TDrawingContext> view,
        Action<LiveChartsSettings> defaultPlatformConfig,
        MotionCanvas<TDrawingContext> canvas,
        bool requiresLegendMeasureAlways = false)
        : base(canvas, defaultPlatformConfig, view)
    {
        _chartView = view;
        _requiresLegendMeasureAlways = requiresLegendMeasureAlways;
    }

    /// <summary>
    /// Gets the x axes.
    /// </summary>
    /// <value>
    /// The x axes.
    /// </value>
    public ICartesianAxis[] XAxes { get; private set; } = Array.Empty<ICartesianAxis>();

    /// <summary>
    /// Gets the y axes.
    /// </summary>
    /// <value>
    /// The y axes.
    /// </value>
    public ICartesianAxis[] YAxes { get; private set; } = Array.Empty<ICartesianAxis>();

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public ICartesianSeries<TDrawingContext>[] Series { get; private set; } = Array.Empty<ICartesianSeries<TDrawingContext>>();

    /// <summary>
    /// Gets the sections.
    /// </summary>
    /// <value>
    /// The sections.
    /// </value>
    public Section<TDrawingContext>[] Sections { get; private set; } = Array.Empty<Section<TDrawingContext>>();

    /// <summary>
    /// Gets the drawable series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public override IEnumerable<IChartSeries<TDrawingContext>> ChartSeries => Series;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is zooming or panning.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is zooming or panning; otherwise, <c>false</c>.
    /// </value>
    public bool IsZoomingOrPanning { get; private set; }

    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public override IChartView<TDrawingContext> View => _chartView;

    /// <summary>
    /// Finds the points near to the specified location.
    /// </summary>
    /// <param name="pointerPosition">The pointer position.</param>
    /// <returns></returns>
    public override IEnumerable<ChartPoint> FindHoveredPointsBy(LvcPoint pointerPosition)
    {
        var actualStrategy = TooltipFindingStrategy;

        if (actualStrategy == TooltipFindingStrategy.Automatic)
            actualStrategy = Series.GetTooltipFindingStrategy();

        return ChartSeries
            .Where(series => series.IsHoverable)
            .SelectMany(series => series.FindHoveredPoints(this, pointerPosition, actualStrategy));
    }

    /// <summary>
    /// Scales the specified point to the UI.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="xAxisIndex">Index of the x axis.</param>
    /// <param name="yAxisIndex">Index of the y axis.</param>
    /// <returns></returns>
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        var xAxis = XAxes[xAxisIndex];
        var yAxis = YAxes[yAxisIndex];

        var xScaler = new Scaler(DrawMarginLocation, DrawMarginSize, xAxis);
        var yScaler = new Scaler(DrawMarginLocation, DrawMarginSize, yAxis);

        return new double[] { xScaler.ToChartValues(point.X), yScaler.ToChartValues(point.Y) };
    }

    /// <summary>
    /// Zooms to the specified pivot.
    /// </summary>
    /// <param name="pivot">The pivot.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public void Zoom(LvcPoint pivot, ZoomDirection direction, double? scaleFactor = null, bool isActive = false)
    {
        if (YAxes is null || XAxes is null) return;

        var speed = _zoomingSpeed < 0.1 ? 0.1 : (_zoomingSpeed > 0.95 ? 0.95 : _zoomingSpeed);
        speed = 1 - speed;

        if (scaleFactor is not null && direction != ZoomDirection.DefinedByScaleFactor)
            throw new InvalidOperationException(
                $"When the scale factor is defined, the zoom direction must be {nameof(ZoomDirection.DefinedByScaleFactor)}... " +
                $"it just makes sense.");

        var m = direction == ZoomDirection.ZoomIn ? speed : 1 / speed;

        if ((_zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
        {
            for (var index = 0; index < XAxes.Length; index++)
            {
                var xi = XAxes[index];
                var px = new Scaler(DrawMarginLocation, DrawMarginSize, xi).ToChartValues(pivot.X);

                var max = xi.MaxLimit is null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                var min = xi.MinLimit is null ? xi.DataBounds.Min : xi.MinLimit.Value;

                double mint, maxt;
                var l = max - min;

                if (scaleFactor is null)
                {
                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;

                    mint = px - target * rMin;
                    maxt = px + target * rMax;
                }
                else
                {
                    var delta = 1 - scaleFactor.Value;
                    int dir;

                    if (delta < 0)
                    {
                        dir = -1;
                        direction = ZoomDirection.ZoomIn;
                    }
                    else
                    {
                        dir = 1;
                        direction = ZoomDirection.ZoomOut;
                    }

                    var ld = l * Math.Abs(delta);
                    mint = min - ld * 0.5 * dir;
                    maxt = max + ld * 0.5 * dir;
                }

                if (maxt - mint < xi.DataBounds.MinDelta * 5) return;

                var xm = (max - min) * (isActive ? MaxAxisActiveBound : MaxAxisBound);
                if (maxt > xi.DataBounds.Max && direction == ZoomDirection.ZoomOut) maxt = xi.DataBounds.Max + xm;
                if (mint < xi.DataBounds.Min && direction == ZoomDirection.ZoomOut) mint = xi.DataBounds.Min - xm;

                xi.MaxLimit = maxt;
                xi.MinLimit = mint;
            }
        }

        if ((_zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
        {
            for (var index = 0; index < YAxes.Length; index++)
            {
                var yi = YAxes[index];
                var px = new Scaler(DrawMarginLocation, DrawMarginSize, yi).ToChartValues(pivot.Y);

                var max = yi.MaxLimit is null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                var min = yi.MinLimit is null ? yi.DataBounds.Min : yi.MinLimit.Value;

                double mint, maxt;
                var l = max - min;

                if (scaleFactor is null)
                {
                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;
                    mint = px - target * rMin;
                    maxt = px + target * rMax;
                }
                else
                {
                    var delta = 1 - scaleFactor.Value;
                    int dir;

                    if (delta < 0)
                    {
                        dir = -1;
                        direction = ZoomDirection.ZoomIn;
                    }
                    else
                    {
                        dir = 1;
                        direction = ZoomDirection.ZoomOut;
                    }

                    var ld = l * Math.Abs(delta);
                    mint = min - ld * 0.5 * dir;
                    maxt = max + ld * 0.5 * dir;
                }

                if (maxt - mint < yi.DataBounds.MinDelta * 5) return;

                var ym = (max - min) * (isActive ? MaxAxisActiveBound : MaxAxisBound);
                if (maxt > yi.DataBounds.Max && direction == ZoomDirection.ZoomOut) maxt = yi.DataBounds.Max + ym;
                if (mint < yi.DataBounds.Min && direction == ZoomDirection.ZoomOut) mint = yi.DataBounds.Min - ym;

                yi.MaxLimit = maxt;
                yi.MinLimit = mint;
            }
        }

        IsZoomingOrPanning = true;
    }

    /// <summary>
    /// Pans with the specified delta.
    /// </summary>
    /// <param name="delta">The delta.</param>
    /// <param name="isActive">Indicates whether the pointer is down.</param>
    /// <returns></returns>
    public void Pan(LvcPoint delta, bool isActive)
    {
        if ((_zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
        {
            for (var index = 0; index < XAxes.Length; index++)
            {
                var xi = XAxes[index];
                var scale = new Scaler(DrawMarginLocation, DrawMarginSize, xi);
                var dx = scale.ToChartValues(-delta.X) - scale.ToChartValues(0);

                var max = xi.MaxLimit is null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                var min = xi.MinLimit is null ? xi.DataBounds.Min : xi.MinLimit.Value;

                var xm = max - min;
                xm = isActive ? xm * MaxAxisActiveBound : xm * MaxAxisBound;

                if (max + dx > xi.DataBounds.Max && delta.X < 0)
                {
                    xi.MaxLimit = xi.DataBounds.Max + xm;
                    xi.MinLimit = xi.DataBounds.Max - (max - xm - min);
                    continue;
                }

                if (min + dx < xi.DataBounds.Min && delta.X > 0)
                {
                    xi.MinLimit = xi.DataBounds.Min - xm;
                    xi.MaxLimit = xi.DataBounds.Min + max - min - xm;
                    continue;
                }

                xi.MaxLimit = max + dx;
                xi.MinLimit = min + dx;
            }
        }

        if ((_zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
        {
            for (var index = 0; index < YAxes.Length; index++)
            {
                var yi = YAxes[index];
                var scale = new Scaler(DrawMarginLocation, DrawMarginSize, yi);
                var dy = -(scale.ToChartValues(delta.Y) - scale.ToChartValues(0));

                var max = yi.MaxLimit is null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                var min = yi.MinLimit is null ? yi.DataBounds.Min : yi.MinLimit.Value;

                var ym = max - min;
                ym = isActive ? ym * MaxAxisActiveBound : ym * MaxAxisBound;

                if (max + dy > yi.DataBounds.Max)
                {
                    yi.MaxLimit = yi.DataBounds.Max + ym;
                    yi.MinLimit = yi.DataBounds.Max - (max - ym - min);
                    continue;
                }

                if (min + dy < yi.DataBounds.Min)
                {
                    yi.MinLimit = yi.DataBounds.Min - ym;
                    yi.MaxLimit = yi.DataBounds.Min + max - min - ym;
                    continue;
                }

                yi.MaxLimit = max + dy;
                yi.MinLimit = min + dy;
            }
        }

        IsZoomingOrPanning = true;
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

        if (preserveFirstDraw)
        {
            IsFirstDraw = true;
            preserveFirstDraw = false;
        }

        MeasureWork = new object();

        #region copy the current data in the view

        var viewDrawMargin = _chartView.DrawMargin;
        ControlSize = _chartView.ControlSize;

        YAxes = _chartView.YAxes.Cast<ICartesianAxis>().Select(x => x).ToArray();
        XAxes = _chartView.XAxes.Cast<ICartesianAxis>().Select(x => x).ToArray();

        _zoomingSpeed = _chartView.ZoomingSpeed;
        _zoomMode = _chartView.ZoomMode;

        var theme = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
        if (theme.CurrentColors is null || theme.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        var forceApply = ThemeId != LiveCharts.CurrentSettings.ThemeId && !IsFirstDraw;

        LegendPosition = _chartView.LegendPosition;
        LegendOrientation = _chartView.LegendOrientation;
        Legend = _chartView.Legend;

        TooltipPosition = _chartView.TooltipPosition;
        TooltipFindingStrategy = _chartView.TooltipFindingStrategy;
        Tooltip = _chartView.Tooltip;

        AnimationsSpeed = _chartView.AnimationsSpeed;
        EasingFunction = _chartView.EasingFunction;

        var actualSeries = (_chartView.Series ?? Enumerable.Empty<ISeries>()).Where(x => x.IsVisible);

        Series = actualSeries
            .Cast<ICartesianSeries<TDrawingContext>>()
            .ToArray();

        Sections = _chartView.Sections?.Where(x => x.IsVisible).ToArray() ?? Array.Empty<Section<TDrawingContext>>();

        #endregion

        SeriesContext = new SeriesContext<TDrawingContext>(Series);

        // restart axes bounds and meta data
        foreach (var axis in XAxes)
        {
            axis.IsNotifyingChanges = false;
            axis.Initialize(AxisOrientation.X);
            theme.ResolveAxisDefaults((IPlane<TDrawingContext>)axis, forceApply);
            axis.IsNotifyingChanges = true;
        }
        foreach (var axis in YAxes)
        {
            axis.IsNotifyingChanges = false;
            axis.Initialize(AxisOrientation.Y);
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

            var xAxis = XAxes[series.ScalesXAt];
            var yAxis = YAxes[series.ScalesYAt];

            var seriesBounds = series.GetBounds(this, xAxis, yAxis).Bounds;
            if (seriesBounds.IsEmpty) continue;

            xAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds);
            yAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds);
            xAxis.VisibleDataBounds.AppendValue(seriesBounds.VisibleSecondaryBounds);
            yAxis.VisibleDataBounds.AppendValue(seriesBounds.VisiblePrimaryBounds);

            series.IsNotifyingChanges = true;
        }

        #region empty bounds

        // prevent the bounds are not empty...

        foreach (var axis in XAxes)
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
        foreach (var axis in YAxes)
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

        var m = new Margin();
        float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
        SetDrawMargin(ControlSize, m);

        foreach (var axis in XAxes)
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
            if (axis.Position == AxisPosition.Start)
            {
                // X Bottom
                axis.NameDesiredSize = new LvcRectangle(
                    new LvcPoint(0, ControlSize.Height - bs - ns.Height), new LvcSize(ControlSize.Width, ns.Height));
                axis.LabelsDesiredSize = new LvcRectangle(
                    new LvcPoint(0, axis.NameDesiredSize.Y - s.Height), new LvcSize(ControlSize.Width, s.Height));

                axis.Yo = m.Bottom + s.Height * 0.5f + ns.Height;
                bs += s.Height + ns.Height;
                m.Bottom = bs;
                if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
            }
            else
            {
                // X Top
                axis.NameDesiredSize = new LvcRectangle(
                   new LvcPoint(0, ts), new LvcSize(ControlSize.Width, ns.Height));
                axis.LabelsDesiredSize = new LvcRectangle(
                    new LvcPoint(0, ts + ns.Height), new LvcSize(ControlSize.Width, s.Height));

                axis.Yo = ts + s.Height * 0.5f + ns.Height;
                ts += s.Height + ns.Height;
                m.Top = ts;
                if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;
            }
        }
        foreach (var axis in YAxes)
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
            var w = s.Width;

            if (axis.Position == AxisPosition.Start)
            {
                // Y Left
                axis.NameDesiredSize = new LvcRectangle(new LvcPoint(ls, 0), new LvcSize(ns.Width, ControlSize.Height));
                axis.LabelsDesiredSize = new LvcRectangle(new LvcPoint(ls + ns.Width, 0), new LvcSize(s.Width, ControlSize.Height));

                axis.Xo = ls + w * 0.5f + ns.Width;
                ls += w + ns.Width;
                m.Left = ls;
                if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
            }
            else
            {
                // Y Right
                axis.NameDesiredSize = new LvcRectangle(
                    new LvcPoint(ControlSize.Width - rs - ns.Width, 0), new LvcSize(ns.Width, ControlSize.Height));
                axis.LabelsDesiredSize = new LvcRectangle(
                    new LvcPoint(axis.NameDesiredSize.X - s.Width, 0), new LvcSize(s.Width, ControlSize.Height));

                axis.Xo = rs + w * 0.5f + ns.Width;
                rs += w + ns.Width;
                m.Right = rs;
                if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
            }
        }

        var rm = viewDrawMargin ?? new Margin(Margin.Auto);

        var actualMargin = new Margin(
            Margin.IsAuto(rm.Left) ? m.Left : rm.Left,
            Margin.IsAuto(rm.Top) ? m.Top : rm.Top,
            Margin.IsAuto(rm.Right) ? m.Right : rm.Right,
            Margin.IsAuto(rm.Bottom) ? m.Bottom : rm.Bottom);

        SetDrawMargin(ControlSize, actualMargin);

        // invalid dimensions, probably the chart is too small
        // or it is initializing in the UI and has no dimensions yet
        if (DrawMarginSize.Width <= 0 || DrawMarginSize.Height <= 0) return;

        UpdateBounds();

        var totalAxes = XAxes.Concat(YAxes).ToArray();
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
                var s = new Scaler(DrawMarginLocation, DrawMarginSize, axis);
                // correction by geometry size
                var p = Math.Abs(s.ToChartValues(axis.DataBounds.RequestedGeometrySize) - s.ToChartValues(0));
                if (axis.DataBounds.PaddingMin > p) p = axis.DataBounds.PaddingMin;
                axis.IsNotifyingChanges = false;
                axis.DataBounds.Min = axis.DataBounds.Min - p;
                axis.VisibleDataBounds.Min = axis.VisibleDataBounds.Min - p;
                axis.IsNotifyingChanges = true;
            }

            // apply padding
            if (axis.MaxLimit is null)
            {
                var s = new Scaler(DrawMarginLocation, DrawMarginSize, axis);
                // correction by geometry size
                var p = Math.Abs(s.ToChartValues(axis.DataBounds.RequestedGeometrySize) - s.ToChartValues(0));
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

        var toDeleteSections = new HashSet<Section<TDrawingContext>>(_everMeasuredSections);
        foreach (var section in Sections)
        {
            section.Measure(this);
            section.RemoveOldPaints(View);
            _ = _everMeasuredSections.Add(section);
            _ = toDeleteSections.Remove(section);
        }

        var toDeleteSeries = new HashSet<ISeries>(_everMeasuredSeries);
        foreach (var series in Series)
        {
            series.Measure(this);
            series.RemoveOldPaints(View);
            _ = _everMeasuredSeries.Add(series);
            _ = toDeleteSeries.Remove(series);
        }

        if (_previousDrawMarginFrame is not null && _chartView.DrawMarginFrame != _previousDrawMarginFrame)
        {
            _previousDrawMarginFrame.RemoveFromUI(this);
            _previousDrawMarginFrame = null;
        }
        if (_chartView.DrawMarginFrame is not null)
        {
            _chartView.DrawMarginFrame.Measure(this);
            _chartView.DrawMarginFrame.RemoveOldPaints(View);
            _previousDrawMarginFrame = _chartView.DrawMarginFrame;
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
        foreach (var section in toDeleteSections)
        {
            section.RemoveFromUI(this);
            _ = _everMeasuredSections.Remove(section);
        }

        foreach (var axis in totalAxes)
        {
            if (!axis.IsVisible) continue;

            axis.IsNotifyingChanges = false;
            axis.ActualBounds.HasPreviousState = true;
            axis.IsNotifyingChanges = true;
        }

        ActualBounds.HasPreviousState = true;

        IsZoomingOrPanning = false;
        InvokeOnUpdateStarted();

        IsFirstDraw = false;
        ThemeId = LiveCharts.CurrentSettings.ThemeId;
        PreviousSeries = Series;
        PreviousLegendPosition = LegendPosition;

        Canvas.Invalidate();
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

        Canvas.Dispose();

        IsFirstDraw = true;
    }
}
