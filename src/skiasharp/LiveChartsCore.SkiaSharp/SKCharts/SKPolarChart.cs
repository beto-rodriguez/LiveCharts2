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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// In-memory chart that is able to generate a chart images.
/// </summary>
public class SKPolarChart : InMemorySkiaSharpChart, IPolarChartView<SkiaSharpDrawingContext>
{
    private LvcColor _backColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKPolarChart"/> class.
    /// </summary>
    public SKPolarChart()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Core = new PolarChart<SkiaSharpDrawingContext>(this, config => config.UseDefaults(), CoreCanvas);
        Core.Measuring += OnCoreMeasuring;
        Core.UpdateStarted += OnCoreUpdateStarted;
        Core.UpdateFinished += OnCoreUpdateFinished;

        CoreChart = Core;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKPolarChart"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    public SKPolarChart(IPolarChartView<SkiaSharpDrawingContext> view) : this()
    {
        AngleAxes = view.AngleAxes;
        RadiusAxes = view.RadiusAxes;
        Series = view.Series;
        FitToBounds = view.FitToBounds;
        TotalAngle = view.TotalAngle;
        InnerRadius = view.InnerRadius;
        InitialRotation = view.InitialRotation;
        LegendPosition = view.LegendPosition;
        Title = view.Title;
        DrawMargin = view.DrawMargin;
        VisualElements = view.VisualElements;
    }

    /// <inheritdoc cref="IChartView.DesignerMode" />
    public bool DesignerMode => false;

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Core"/>
    public PolarChart<SkiaSharpDrawingContext> Core { get; }

    /// <inheritdoc cref="IChartView.SyncContext"/>
    public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.AngleAxes"/>
    public IEnumerable<IPolarAxis> AngleAxes { get; set; } = [new PolarAxis()];

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.RadiusAxes"/>
    public IEnumerable<IPolarAxis> RadiusAxes { get; set; } = [new PolarAxis()];

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series"/>
    public IEnumerable<ISeries> Series { get; set; } = [];

    /// <inheritdoc cref="IChartView.VisualElements"/>
    public IEnumerable<ChartElement> VisualElements { get; set; } = [];

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

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.FitToBounds"/>
    public bool FitToBounds { get; set; }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.TotalAngle"/>
    public double TotalAngle { get; set; } = 360;

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InnerRadius"/>
    public double InnerRadius { get; set; }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InitialRotation"/>
    public double InitialRotation { get; set; } = LiveCharts.DefaultSettings.PolarInitialRotation;

    /// <inheritdoc cref="IChartView.AutoUpdateEnabled"/>
    public bool AutoUpdateEnabled { get; set; }

    /// <inheritdoc cref="IChartView.LegendTextPaint"/>
    public Paint? LegendTextPaint { get; set; }

    /// <inheritdoc cref="IChartView.LegendBackgroundPaint"/>
    public Paint? LegendBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView.LegendTextSize"/>
    public double? LegendTextSize { get; set; }

    /// <inheritdoc cref="IChartView.TooltipTextPaint"/>
    public Paint? TooltipTextPaint { get; set; }

    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint"/>
    public Paint? TooltipBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView.TooltipTextSize"/>
    public double? TooltipTextSize { get; set; }

    /// <inheritdoc cref="IChartView.Title"/>
    public CoreVisualElement? Title { get; set; }

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

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        var scaler = new PolarScaler(
            Core.DrawMarginLocation, Core.DrawMarginSize, Core.AngleAxes[angleAxisIndex], Core.RadiusAxes[radiusAxisIndex],
            Core.InnerRadius, Core.InitialRotation, Core.TotalAnge);

        return scaler.ToChartValues(point.X, point.Y);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        var scaler = new PolarScaler(
            Core.DrawMarginLocation, Core.DrawMarginSize, Core.AngleAxes[angleAxisIndex], Core.RadiusAxes[radiusAxisIndex],
            Core.InnerRadius, Core.InitialRotation, Core.TotalAnge);

        var r = scaler.ToPixels(point.X, point.Y);

        return new LvcPointD { X = (float)r.X, Y = (float)r.Y };
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
        Core.VisualElements.SelectMany(visual => ((CoreVisualElement)visual).IsHitBy(Core, new(point)));

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
        IEnumerable<CoreVisualElement> visualElements, LvcPoint pointer) => VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs(Core, visualElements, pointer));

    void IChartView.Invalidate() => throw new NotImplementedException();
}
