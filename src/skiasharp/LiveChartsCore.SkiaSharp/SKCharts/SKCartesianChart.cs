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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// In-memory chart that is able to generate a chart images.
/// </summary>
public class SKCartesianChart : InMemorySkiaSharpChart, ICartesianChartView<SkiaSharpDrawingContext>, IDrawnLegend
{
    private LvcColor _backColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKCartesianChart"/> class.
    /// </summary>
    public SKCartesianChart()
    {
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        Core = new CartesianChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, CoreCanvas, new RectangleGeometry());
        Core.Measuring += OnCoreMeasuring;
        Core.UpdateStarted += OnCoreUpdateStarted;
        Core.UpdateFinished += OnCoreUpdateFinished;

        CoreChart = Core;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKCartesianChart"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    public SKCartesianChart(ICartesianChartView<SkiaSharpDrawingContext> view) : this()
    {
        XAxes = view.XAxes;
        YAxes = view.YAxes;
        Series = view.Series;
        Sections = view.Sections;
        DrawMarginFrame = view.DrawMarginFrame;
        LegendPosition = view.LegendPosition;
    }

    /// <inheritdoc cref="IChartView.DesignerMode" />
    public bool DesignerMode => false;

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Core"/>
    public CartesianChart<SkiaSharpDrawingContext> Core { get; }

    /// <inheritdoc cref="IChartView.SyncContext"/>
    public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes"/>
    public IEnumerable<ICartesianAxis> XAxes { get; set; } = new Axis[] { new() };

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes"/>
    public IEnumerable<ICartesianAxis> YAxes { get; set; } = new Axis[] { new() };

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections"/>
    public IEnumerable<Section<SkiaSharpDrawingContext>> Sections { get; set; } = Array.Empty<RectangularSection>();

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements"/>
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; } = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series"/>
    public IEnumerable<ISeries> Series { get; set; } = Array.Empty<ISeries>();

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame"/>
    public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame { get; set; }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode"/>
    public ZoomAndPanMode ZoomMode { get; set; }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed"/>
    public double ZoomingSpeed { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled"/>
    public bool AutoUpdateEnabled { get; set; }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy"/>
    public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend"/>
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; } = new SKDefaultLegend();

    /// <inheritdoc cref="IDrawnLegend.LegendFontPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendFontPaint { get; set; } = new SolidColorPaint { FontFamily = "Arial", Color = new SKColor(40, 40, 40) };

    /// <inheritdoc cref="IDrawnLegend.LegendFontSize" />
    public double LegendFontSize { get; set; } = 13;

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip"/>
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => null;

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

    /// <inheritdoc cref="IChartView.LegendOrientation"/>
    public LegendOrientation LegendOrientation { get; set; }

    /// <inheritdoc cref="IChartView.TooltipPosition"/>
    public TooltipPosition TooltipPosition { get; set; }

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
    public event VisualElementHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)"/>
    [Obsolete($"Use {nameof(ScalePixelsToData)} instead.")]
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        var xScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.XAxes[xAxisIndex]);
        var yScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        var xScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.XAxes[xAxisIndex]);
        var yScaler = new Scaler(Core.DrawMarginLocation, Core.DrawMarginSize, Core.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
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

    /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
    public void SetTooltipStyle(LvcColor background, LvcColor textColor) { }

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        throw new NotImplementedException();
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
        if (LegendPosition == LegendPosition.Hidden || Legend is null) return new(Width, Height);

        if (LegendPosition is LegendPosition.Left or LegendPosition.Right)
        {
            var imageControl = (IImageControl)Legend;
            return new(Width - imageControl.Size.Width, Height);
        }

        if (LegendPosition is LegendPosition.Top or LegendPosition.Bottom)
        {
            var imageControl = (IImageControl)Legend;
            return new(Width, Height - imageControl.Size.Height);
        }

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
        VisualElementsPointerDown?.Invoke(this, new VisualElementsEventArgs<SkiaSharpDrawingContext>(visualElements, pointer));
    }

    void IChartView.Invalidate()
    {
        throw new NotImplementedException();
    }
}
