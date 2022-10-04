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
using System.IO;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// In-memory chart that is able to generate a chart images.
/// </summary>
public class SKPieChart : IPieChartView<SkiaSharpDrawingContext>, ISkiaSharpChart, IDrawnLegend
{
    private LvcColor _backColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKPieChart"/> class.
    /// </summary>
    public SKPieChart()
    {
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        Core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, CoreCanvas);
        Core.Measuring += OnCoreMeasuring;
        Core.UpdateStarted += OnCoreUpdateStarted;
        Core.UpdateFinished += OnCoreUpdateFinished;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKPieChart"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    public SKPieChart(IPieChartView<SkiaSharpDrawingContext> view) : this()
    {
        Series = view.Series;
        InitialRotation = view.InitialRotation;
        MaxAngle = view.MaxAngle;
        Total = view.Total;
        LegendPosition = view.LegendPosition;
    }

    /// <inheritdoc cref="IChartView.DesignerMode" />
    public bool DesignerMode => false;

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    public SKColor Background { get; set; } = SKColors.Empty;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public int Height { get; set; } = 600;

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public int Width { get; set; } = 900;

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Core"/>
    public PieChart<SkiaSharpDrawingContext> Core { get; }

    /// <inheritdoc cref="IChartView.SyncContext"/>
    public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series"/>
    public IEnumerable<ISeries> Series { get; set; } = Array.Empty<ISeries>();

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.VisualElements"/>
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; } = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation"/>
    public double InitialRotation { get; set; }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxAngle"/>
    public double MaxAngle { get; set; } = 360;

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total"/>
    public double? Total { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled"/>
    public bool AutoUpdateEnabled { get; set; }

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas"/>
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas { get; } = new();

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend"/>
    public IChartLegend<SkiaSharpDrawingContext>? Legend { get; } = new SKDefaultLegend();

    /// <inheritdoc cref="IDrawnLegend.LegendFontPaint" />
    public IPaint<SkiaSharpDrawingContext>? LegendFontPaint { get; set; } = new SolidColorPaint { FontFamily = "Arial", Color = new SKColor(40, 40, 40) };

    /// <inheritdoc cref="IDrawnLegend.LegendFontSize" />
    public double LegendFontSize { get; set; } = 13;

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip"/>
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => null;

    /// <inheritdoc cref="IChartView.CoreChart"/>
    public IChart CoreChart => Core;

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
    public Func<float, float>? EasingFunction { get; set; }

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

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        throw new NotImplementedException();
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

    /// <inheritdoc cref="ISkiaSharpChart.GetImage"/>
    public SKImage GetImage()
    {
        CoreCanvas.DisableAnimations = true;

        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

        var canvas = surface.Canvas;
        using var clearColor = new SKPaint { Color = Background };
        canvas.DrawRect(0, 0, Width, Height, clearColor);

        Core.IsLoaded = true;
        Core.IsFirstDraw = true;
        Core.Measure();

        CoreCanvas.DrawFrame(
            new SkiaSharpDrawingContext(
                CoreCanvas,
                new SKImageInfo(Height, Width),
                surface,
                canvas,
                Background));

        Core.Unload();

        return surface.Snapshot();
    }

    /// <inheritdoc cref="ISkiaSharpChart.SaveImage(string, SKEncodedImageFormat, int)"/>
    public void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
    {
        using var image = GetImage();
        using var data = image.Encode(format, quality);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
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

    void IChartView.Invalidate()
    {
        throw new NotImplementedException();
    }
}
