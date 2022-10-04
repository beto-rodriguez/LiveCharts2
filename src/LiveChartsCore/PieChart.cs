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
using System.Runtime.Serialization;
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
    private readonly HashSet<ISeries> _everMeasuredSeries = new();
    internal readonly HashSet<ChartElement<TDrawingContext>> _everMeasuredVisuals = new();
    private readonly IPieChartView<TDrawingContext> _chartView;
    private int _nextSeries = 0;
    private readonly bool _requiresLegendMeasureAlways = false;

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
        _requiresLegendMeasureAlways = requiresLegendMeasureAlways;
    }

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public IPieSeries<TDrawingContext>[] Series { get; private set; } = Array.Empty<IPieSeries<TDrawingContext>>();

    /// <summary>
    /// Gets the visual elements.
    /// </summary>
    /// <value>
    /// The visual elements.
    /// </value>
    public ChartElement<TDrawingContext>[] VisualElements { get; private set; } = Array.Empty<ChartElement<TDrawingContext>>();

    /// <summary>
    /// Gets the drawable series.
    /// </summary>
    /// <value>
    /// The drawable series.
    /// </value>
    public override IEnumerable<IChartSeries<TDrawingContext>> ChartSeries
        => Series.Where(x => (x is IPieSeries<TDrawingContext> pieSeries) && !pieSeries.IsFillSeries);

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
            .SelectMany(series => series.FindHoveredPoints(this, pointerPosition, TooltipFindingStrategy.CompareAll));
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
            IsFirstDraw = true;
            _preserveFirstDraw = false;
        }

        MeasureWork = new object();

        var viewDrawMargin = _chartView.DrawMargin;
        ControlSize = _chartView.ControlSize;

        var actualSeries = (_chartView.Series ?? Enumerable.Empty<ISeries>()).Where(x => x.IsVisible);

        Series = actualSeries
            .Cast<IPieSeries<TDrawingContext>>()
            .ToArray();

        VisualElements = _chartView.VisualElements?.ToArray() ?? Array.Empty<ChartElement<TDrawingContext>>();

        LegendPosition = _chartView.LegendPosition;
        LegendOrientation = _chartView.LegendOrientation;
        Legend = _chartView.Legend;

        TooltipPosition = _chartView.TooltipPosition;
        Tooltip = _chartView.Tooltip;

        AnimationsSpeed = _chartView.AnimationsSpeed;
        EasingFunction = _chartView.EasingFunction;

        SeriesContext = new SeriesContext<TDrawingContext>(Series);

        var theme = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
        if (theme.CurrentColors is null || theme.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        var forceApply = ThemeId != LiveCharts.CurrentSettings.ThemeId && !IsFirstDraw;

        ValueBounds = new Bounds();
        IndexBounds = new Bounds();
        PushoutBounds = new Bounds();
        foreach (var series in Series)
        {
            series.IsNotifyingChanges = false;

            if (series.SeriesId == -1) series.SeriesId = _nextSeries++;
            theme.ResolveSeriesDefaults(theme.CurrentColors, series, forceApply);

            var seriesBounds = series.GetBounds(this);

            ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Max);
            ValueBounds.AppendValue(seriesBounds.PrimaryBounds.Min);
            IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Max);
            IndexBounds.AppendValue(seriesBounds.SecondaryBounds.Min);
            PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Max);
            PushoutBounds.AppendValue(seriesBounds.TertiaryBounds.Min);

            series.IsNotifyingChanges = true;
        }

        var seriesInLegend = Series.Where(x => x.IsVisibleAtLegend).ToList();
        if (Legend is not null && (SeriesMiniatureChanged(seriesInLegend, LegendPosition) || SizeChanged()))
        {
            Legend.Draw(this);
            Update();
            PreviousLegendPosition = LegendPosition;
            PreviousSeriesAtLegend = Series.Where(x => x.IsVisibleAtLegend).ToList();
            _preserveFirstDraw = IsFirstDraw;
        }

        if (viewDrawMargin is null)
        {
            var m = viewDrawMargin ?? new Margin();
            SetDrawMargin(ControlSize, m);
        }

        // invalid dimensions, probably the chart is too small
        // or it is initializing in the UI and has no dimensions yet
        if (DrawMarginSize.Width <= 0 || DrawMarginSize.Height <= 0) return;

        var toDeleteVisualElements = new HashSet<ChartElement<TDrawingContext>>(_everMeasuredVisuals);
        foreach (var visual in VisualElements)
        {
            visual.Invalidate(this);
            visual.RemoveOldPaints(View);
            _ = _everMeasuredVisuals.Add(visual);
            _ = toDeleteVisualElements.Remove(visual);
        }

        var toDeleteSeries = new HashSet<ISeries>(_everMeasuredSeries);
        foreach (var series in Series)
        {
            series.Invalidate(this);
            series.RemoveOldPaints(View);
            _ = _everMeasuredSeries.Add(series);
            _ = toDeleteSeries.Remove(series);
        }

        foreach (var series in toDeleteSeries)
        {
            series.SoftDeleteOrDispose(View);
            _ = _everMeasuredSeries.Remove(series);
        }
        foreach (var visual in toDeleteVisualElements)
        {
            visual.RemoveFromUI(this);
            _ = _everMeasuredVisuals.Remove(visual);
        }

        InvokeOnUpdateStarted();
        IsFirstDraw = false;
        ThemeId = LiveCharts.CurrentSettings.ThemeId;
        PreviousSeriesAtLegend = Series.Where(x => x.IsVisibleAtLegend).ToList();
        PreviousLegendPosition = LegendPosition;

        Canvas.Invalidate();
    }

    /// <inheritdoc cref="Chart{TDrawingContext}.Unload"/>
    public override void Unload()
    {
        base.Unload();

        foreach (var item in _everMeasuredSeries) ((ChartElement<TDrawingContext>)item).RemoveFromUI(this);
        _everMeasuredSeries.Clear();
        foreach (var item in _everMeasuredVisuals) item.RemoveFromUI(this);
        _everMeasuredVisuals.Clear();
        IsFirstDraw = true;
    }
}
