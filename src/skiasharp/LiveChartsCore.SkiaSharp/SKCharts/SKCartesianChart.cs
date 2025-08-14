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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// In-memory chart that is able to generate a chart images.
/// </summary>
public class SKCartesianChart : InMemorySkiaSharpChart, ICartesianChartView
{
    private LvcColor _backColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKCartesianChart"/> class.
    /// </summary>
    /// <param name="chartView">The chart view source to build the image from.</param>
    public SKCartesianChart(ICartesianChartView? chartView = null)
        : base(chartView)
    {
        Core = new CartesianChartEngine(this, config => config.UseDefaults(), CoreCanvas);
        Core.Measuring += OnCoreMeasuring;
        Core.UpdateStarted += OnCoreUpdateStarted;
        Core.UpdateFinished += OnCoreUpdateFinished;

        CoreChart = Core;
    }

    bool IChartView.DesignerMode => false;
    bool IChartView.IsDarkMode => false;

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get; set; }

    /// <inheritdoc cref="ICartesianChartView.Core"/>
    public CartesianChartEngine Core { get; }

    /// <inheritdoc cref="IChartView.SyncContext"/>
    public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

    /// <inheritdoc cref="ICartesianChartView.XAxes"/>
    public ICollection<ICartesianAxis> XAxes { get; set; } = [new Axis()];

    /// <inheritdoc cref="ICartesianChartView.YAxes"/>
    public ICollection<ICartesianAxis> YAxes { get; set; } = [new Axis()];

    /// <inheritdoc cref="ICartesianChartView.Sections"/>
    public ICollection<IChartElement> Sections { get; set; } = [];

    /// <inheritdoc cref="IChartView.VisualElements"/>
    public ICollection<IChartElement> VisualElements { get; set; } = [];

    /// <inheritdoc cref="IChartView.Series"/>
    public ICollection<ISeries> Series { get; set; } = [];

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame"/>
    public IChartElement? DrawMarginFrame { get; set; }

    /// <inheritdoc cref="ICartesianChartView.ZoomMode"/>
    public ZoomAndPanMode ZoomMode { get; set; }

    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed"/>
    public double ZoomingSpeed { get; set; }

    /// <inheritdoc cref="IChartView.AutoUpdateEnabled"/>
    public bool AutoUpdateEnabled { get; set; }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy"/>
    public FindingStrategy FindingStrategy { get; set; }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy"/>
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

    /// <inheritdoc cref="IChartView.Legend"/>
    public IChartLegend? Legend { get; set; } = new SKDefaultLegend();

    /// <inheritdoc cref="IChartView.Tooltip"/>
    public IChartTooltip? Tooltip { get; set; }

    LvcColor IChartView.BackColor
    {
        get => _backColor;
        set
        {
            _backColor = value;
            Background = new SKColor(_backColor.R, _backColor.G, _backColor.B, _backColor.A);
        }
    }

    LvcSize IChartView.ControlSize => GetControlSize();

    /// <inheritdoc cref="IChartView.DrawMargin"/>
    public Margin? DrawMargin { get; set; }

    /// <inheritdoc cref="IChartView.AnimationsSpeed"/>
    public TimeSpan AnimationsSpeed { get; set; }

    /// <inheritdoc cref="IChartView.EasingFunction"/>
    public Func<float, float>? EasingFunction { get; set; } = null;

    /// <inheritdoc cref="IChartView.UpdaterThrottler"/>
    public TimeSpan UpdaterThrottler { get; set; }

    /// <inheritdoc cref="IChartView.LegendPosition"/>
    public LegendPosition LegendPosition { get; set; }

    /// <inheritdoc cref="IChartView.TooltipPosition"/>
    public TooltipPosition TooltipPosition { get; set; }

    /// <inheritdoc cref="IChartView.Title"/>
    public IChartElement? Title { get; set; }

    /// <inheritdoc cref="IChartView.LegendTextPaint"/>
    public Paint? LegendTextPaint { get => field; set { value?.PaintStyle = PaintStyle.Text; field = value; } }

    /// <inheritdoc cref="IChartView.LegendBackgroundPaint"/>
    public Paint? LegendBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView.LegendTextSize"/>
    public double LegendTextSize { get; set; } = LiveCharts.DefaultSettings.LegendTextSize;

    /// <inheritdoc cref="IChartView.TooltipTextPaint"/>
    public Paint? TooltipTextPaint { get => field; set { value?.PaintStyle = PaintStyle.Text; field = value; } }

    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint"/>
    public Paint? TooltipBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView.TooltipTextSize"/>
    public double TooltipTextSize { get; set; } = LiveCharts.DefaultSettings.TooltipTextSize;

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

    /// <inheritdoc cref="ICartesianChartView.MatchAxesScreenDataRatio" />
    public bool MatchAxesScreenDataRatio
    {
        get;
        set
        {
            field = value;

            if (value) SharedAxes.MatchAxesScreenDataRatio(this);
            else SharedAxes.DisposeMatchAxesScreenDataRatio(this);
        }
    }

    /// <inheritdoc cref="ICartesianChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        var xScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.XAxes[xAxisIndex]);
        var yScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        var xScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.XAxes[xAxisIndex]);
        var yScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (strategy == FindingStrategy.Automatic)
            strategy = Core.Series.GetFindingStrategy();

        return Core.Series.SelectMany(series => series.FindHitPoints(Core, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point) =>
        Core.VisualElements.SelectMany(visual => ((VisualElement)visual).IsHitBy(Core, new(point)));

    void IChartView.InvokeOnUIThread(Action action) => action();

    private void OnCoreUpdateFinished(IChartView chart) =>
        UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart) =>
        UpdateStarted?.Invoke(this);

    private void OnCoreMeasuring(IChartView chart) =>
        Measuring?.Invoke(this);

    private LvcSize GetControlSize() => new(Width, Height);

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints) =>
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer) =>
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs(Core, visualElements, pointer));

    void IChartView.Invalidate() => throw new NotImplementedException();
}
