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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
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

    /// <inheritdoc cref="IChartView.ResetSeries">
    public bool ResetSeries { get; set; }

    /// <inheritdoc cref="IChartView.DesignerMode" />
    public bool DesignerMode => false;

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Core"/>
    public PolarChart<SkiaSharpDrawingContext> Core { get; }

    /// <inheritdoc cref="IChartView.SyncContext"/>
    public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.AngleAxes"/>
    public IEnumerable<IPolarAxis> AngleAxes { get; set; } = new[] { new PolarAxis() };

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.RadiusAxes"/>
    public IEnumerable<IPolarAxis> RadiusAxes { get; set; } = new[] { new PolarAxis() };

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series"/>
    public IEnumerable<ISeries> Series { get; set; } = Array.Empty<ISeries>();

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements"/>
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; } = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend"/>
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; set; } = new SKDefaultLegend();

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip"/>
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get; set; }

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

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled"/>
    public bool AutoUpdateEnabled { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextPaint"/>
    public IPaint<SkiaSharpDrawingContext>? LegendTextPaint { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendBackgroundPaint"/>
    public IPaint<SkiaSharpDrawingContext>? LegendBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.LegendTextSize"/>
    public double? LegendTextSize { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextPaint"/>
    public IPaint<SkiaSharpDrawingContext>? TooltipTextPaint { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipBackgroundPaint"/>
    public IPaint<SkiaSharpDrawingContext>? TooltipBackgroundPaint { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.TooltipTextSize"/>
    public double? TooltipTextSize { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Title"/>
    public VisualElement<SkiaSharpDrawingContext>? Title { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElementsPointerDown"/>
    public event VisualElementsHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

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

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = Core.Series.GetTooltipFindingStrategy();

        return Core.Series.SelectMany(series => series.FindHitPoints(Core, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return Core.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(Core, point));
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        action();
    }

    private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateFinished?.Invoke(this);
    }

    private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
    {
        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
    {
        Measuring?.Invoke(this);
    }

    private LvcSize GetControlSize()
    {
        return new(Width, Height);
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
    }

    void IChartView<SkiaSharpDrawingContext>.OnVisualElementPointerDown(
        IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs<SkiaSharpDrawingContext>(Core, visualElements, pointer));
    }

    void IChartView.Invalidate()
    {
        throw new NotImplementedException();
    }
}
