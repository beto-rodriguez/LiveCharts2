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
using System.IO;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// A chart that is able to generate images or draw to a given canvas.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InMemorySkiaSharpChart"/> class.
/// </remarks>
public abstract class InMemorySkiaSharpChart(IChartView? chartView = null)
{
    private readonly IChartView? _chartView = chartView;

    static InMemorySkiaSharpChart()
    {
        LiveCharts.Configure(config => config.UseDefaults());
    }

    /// <inheritdoc cref="IChartView.CoreCanvas"/>
    public CoreMotionCanvas CoreCanvas { get; } = new();

    /// <inheritdoc cref="IChartView.CoreChart"/>
    public Chart CoreChart { get; protected set; } = null!;

    internal bool ExplicitDisposing { get; set; }

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    public SKColor Background { get; set; } = SKColors.White;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public int Height
    {
        get => _chartView is null ? field : (int)_chartView.ControlSize.Height;
        set { field = value; WarnSize(); }
    } = 600;

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public int Width
    {
        get => _chartView is null ? field : (int)_chartView.ControlSize.Width;
        set { field = value; WarnSize(); }
    } = 900;

    /// <summary>
    /// Gets the current <see cref="SKSurface"/>.
    /// </summary>
    /// <returns></returns>
    public virtual SKImage GetImage()
    {
        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
        using var canvas = surface.Canvas;

        DrawOnCanvas(canvas);

        return surface.Snapshot();
    }

    /// <summary>
    /// Saves the image to the specified path.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="format">The format.</param>
    /// <param name="quality">The quality, an integer from 0 to 100.</param>
    /// <returns></returns>
    public virtual void SaveImage(Stream stream, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
    {
        using var image = GetImage();
        using var data = image.Encode(format, quality);
        data.SaveTo(stream);
    }

    /// <summary>
    /// Saves the image to the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="format">The format.</param>
    /// <param name="quality">The quality, an integer from 0 to 100.</param>
    /// <returns></returns>
    public virtual void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
    {
        using var stream = File.OpenWrite(path);
        SaveImage(stream, format, quality);
    }

    /// <summary>
    /// Draws the image to the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="clearCanvasOnBeginDraw">Indicates whether the canvas should be cleared when the draw starts, default is false.</param>
    public virtual void SaveImage(SKCanvas canvas, bool clearCanvasOnBeginDraw = false) =>
        DrawOnCanvas(canvas, clearCanvasOnBeginDraw);

    /// <summary>
    /// Draws the chart to the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="clearCanvasOnBeginDraw">[probably an obsolete param] Indicates whether the canvas should be cleared when the draw starts, default is false.</param>
    /// <exception cref="Exception"></exception>
    public virtual void DrawOnCanvas(SKCanvas canvas, bool clearCanvasOnBeginDraw = false)
    {
        if (CoreChart is null || CoreChart is not Chart skiaChart)
            throw new Exception("Something is missing :(");

        if (_chartView is not null)
        {
            _chartView.CoreCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    CoreCanvas,
                    new SKImageInfo(Width, Height),
                    canvas,
                    Background,
                    clearCanvasOnBeginDraw));

            return;
        }

        skiaChart.Canvas.DisableAnimations = true;

        skiaChart.IsLoaded = true;
        skiaChart._isFirstDraw = true;
        if (ExplicitDisposing) skiaChart.DisableTooltipCache = true;

        skiaChart.Measure();

        skiaChart.Canvas.DrawFrame(
            new SkiaSharpDrawingContext(
                CoreCanvas,
                new SKImageInfo(Width, Height),
                canvas,
                Background,
                clearCanvasOnBeginDraw));

        if (!ExplicitDisposing) skiaChart.Unload();
    }

    private void WarnSize()
    {
#if DEBUG
        if (_chartView is null) return;
        throw new InvalidOperationException(
           $"The chart image dimensions are ignored when built from an {nameof(IChartView)} instance. " +
           $"If you need the chart in a specific size, please use the parameterless contructor and build " +
           $"the chart from there, or resize the chart in UI first to the desired size.");
#endif
    }
}
