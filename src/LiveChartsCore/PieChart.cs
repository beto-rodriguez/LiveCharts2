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
/// Defines a pie chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="Chart{TDrawingContext}" />
public class PieChart<TDrawingContext> : Chart<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private readonly IPieChartView<TDrawingContext> _chartView;
    private int _nextSeries = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="defaultPlatformConfig">The default platform configuration.</param>
    /// <param name="canvas">The canvas.</param>
    /// <param name="requiresLegendMeasureAlways">Forces the legends to redraw with every measure request.</param>
    public PieChart(
        IPieChartView<TDrawingContext> view,
        Action<LiveChartsSettings> defaultPlatformConfig,
        MotionCanvas<TDrawingContext> canvas,
        bool requiresLegendMeasureAlways = false)
        : base(canvas, defaultPlatformConfig, view)
    {
        _chartView = view;
    }

    ///<inheritdoc cref="Chart{TDrawingContext}.Series"/>
    public override IEnumerable<IChartSeries<TDrawingContext>> Series =>
        _chartView.Series.Cast<IChartSeries<TDrawingContext>>();

    ///<inheritdoc cref="Chart{TDrawingContext}.VisibleSeries"/>
    public override IEnumerable<IChartSeries<TDrawingContext>> VisibleSeries =>
        Series.Where(x => x.IsVisible);

    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public override IChartView<TDrawingContext> View => _chartView;

    /// <summary>
    /// Gets the value bounds.
    /// </summary>
    /// <value>
    /// The value bounds.
    /// </value>
    public Bounds ValueBounds { get; private set; } = new();

    /// <summary>
    /// Gets the index bounds.
    /// </summary>
    /// <value>
    /// The index bounds.
    /// </value>
    public Bounds IndexBounds { get; private set; } = new();

    /// <summary>
    /// Gets the pushout bounds.
    /// </summary>
    /// <value>
    /// The pushout bounds.
    /// </value>
    public Bounds PushoutBounds { get; private set; } = new();

    /// <summary>
    /// Finds the points near to the specified point.
    /// </summary>
    /// <param name="pointerPosition">The pointer position.</param>
    /// <returns></returns>
    public override IEnumerable<ChartPoint> FindHoveredPointsBy(LvcPoint pointerPosition)
    {
        return _chartView.Series
            .Where(series => (series is IPieSeries<TDrawingContext> pieSeries) && !pieSeries.IsFillSeries)
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

        var viewDrawMargin = _chartView.DrawMargin;
        ControlSize = _chartView.ControlSize;

        VisualElements = _chartView.VisualElements ?? Array.Empty<ChartElement<TDrawingContext>>();

        LegendPosition = _chartView.LegendPosition;
        Legend = _chartView.Legend;

        TooltipPosition = _chartView.TooltipPosition;
        Tooltip = _chartView.Tooltip;

        AnimationsSpeed = _chartView.AnimationsSpeed;
        EasingFunction = _chartView.EasingFunction;

        SeriesContext = new SeriesContext<TDrawingContext>(VisibleSeries, this);
        var isNewTheme = LiveCharts.DefaultSettings.CurrentThemeId != ThemeId;

        var theme = LiveCharts.DefaultSettings.GetTheme<TDrawingContext>();

        ValueBounds = new Bounds();
        IndexBounds = new Bounds();
        PushoutBounds = new Bounds();

        foreach (var series in VisibleSeries.Cast<IPieSeries<TDrawingContext>>())
        {
            if (series.SeriesId == -1) series.SeriesId = _nextSeries++;

            var ce = (ChartElement<TDrawingContext>)series;
            ce._isInternalSet = true;
            if (!ce._isThemeSet || isNewTheme)
            {
                theme.ApplyStyleToSeries(series);
                ce._isThemeSet = true;
            }

            var seriesBounds = series.GetBounds(this);

            ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Max);
            ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Min);
            IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Max);
            IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Min);
            PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Max);
            PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Min);

            ce._isInternalSet = false;
        }

        InitializeVisualsCollector();

        var title = View.Title;
        var m = new Margin();
        float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
        if (title is not null)
        {
            title.ClippingMode = ClipMode.None;
            var titleSize = title.Measure(this);
            m.Top = titleSize.Height;
            ts = titleSize.Height;
            _titleHeight = titleSize.Height;
        }

        DrawLegend(ref ts, ref bs, ref ls, ref rs);

        m.Top = ts;
        m.Bottom = bs;
        m.Left = ls;
        m.Right = rs;

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

        if (title is not null)
        {
            var titleSize = title.Measure(this);
            title.AlignToTopLeftCorner();
            title.X = ControlSize.Width * 0.5f - titleSize.Width * 0.5f;
            title.Y = 0;
            AddVisual(title);
        }

        foreach (var visual in VisualElements) AddVisual(visual);
        foreach (var series in VisibleSeries)
        {
            AddVisual((ChartElement<TDrawingContext>)series);
            _drawnSeries.Add(series.SeriesId);
        }

        CollectVisuals();

        if (_isToolTipOpen) DrawToolTip();
        InvokeOnUpdateStarted();
        _isFirstDraw = false;
        ThemeId = LiveCharts.DefaultSettings.CurrentThemeId;

        Canvas.Invalidate();
        _isFirstDraw = false;
    }

    /// <inheritdoc cref="Chart{TDrawingContext}.Unload"/>
    public override void Unload()
    {
        base.Unload();
        _isFirstDraw = true;
    }
}
