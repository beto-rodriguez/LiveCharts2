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

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing;

/// <summary>
/// Defines a skia sharp drawing context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SkiaSharpDrawingContext"/> class.
/// </remarks>
/// <param name="motionCanvas">The motion canvas.</param>
/// <param name="info">The information.</param>
/// <param name="surface">The surface.</param>
/// <param name="canvas">The canvas.</param>
/// <param name="clearOnBeginDraw">Indicates whether the canvas is cleared on frame draw.</param>
public class SkiaSharpDrawingContext(
    CoreMotionCanvas motionCanvas,
    SKImageInfo info,
    SKSurface? surface,
    SKCanvas canvas,
    bool clearOnBeginDraw = true)
        : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkiaSharpDrawingContext"/> class.
    /// </summary>
    /// <param name="motionCanvas">The motion canvas.</param>
    /// <param name="info">The information.</param>
    /// <param name="surface">The surface.</param>
    /// <param name="canvas">The canvas.</param>
    /// <param name="background">The background.</param>
    /// <param name="clearOnBeginDraw">Indicates whether the canvas is cleared on frame draw.</param>
    public SkiaSharpDrawingContext(
        CoreMotionCanvas motionCanvas,
        SKImageInfo info,
        SKSurface? surface,
        SKCanvas canvas,
        SKColor background,
        bool clearOnBeginDraw = true)
        : this(motionCanvas, info, surface, canvas, clearOnBeginDraw)
    {
        Background = background;
    }

    /// <summary>
    /// Gets or sets the motion canvas.
    /// </summary>
    /// <value>
    /// The motion canvas.
    /// </value>
    public CoreMotionCanvas MotionCanvas { get; set; } = motionCanvas;

    /// <summary>
    /// Gets or sets the information.
    /// </summary>
    /// <value>
    /// The information.
    /// </value>
    public SKImageInfo Info { get; set; } = info;

    /// <summary>
    /// Gets or sets the surface.
    /// </summary>
    /// <value>
    /// The surface.
    /// </value>
    public SKSurface? Surface { get; set; } = surface;

    /// <summary>
    /// Gets or sets the canvas.
    /// </summary>
    /// <value>
    /// The canvas.
    /// </value>
    public SKCanvas Canvas { get; set; } = canvas;

    /// <summary>
    /// Gets or sets the paint.
    /// </summary>
    /// <value>
    /// The paint.
    /// </value>
    public SKPaint ActiveSkiaPaint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    public SKColor Background { get; set; } = SKColor.Empty;

    /// <inheritdoc cref="DrawingContext.OnBeginDraw"/>
    public override void OnBeginDraw()
    {
        if (clearOnBeginDraw) Canvas.Clear();
        if (Background != SKColor.Empty)
        {
            Canvas.DrawRect(Info.Rect, new SKPaint { Color = Background });
        }
    }

    /// <inheritdoc cref="DrawingContext.LogOnCanvas(string)"/>
    public override void LogOnCanvas(string log)
    {
        using var p = new SKPaint
        {
            Color = SKColors.Blue,
            TextSize = 14,
            IsAntialias = true,
            FakeBoldText = true
        };

        Canvas.DrawText(
            log,
            new SKPoint(50, 10 + p.TextSize),
            p);
    }

    /// <inheritdoc cref="DrawingContext.Draw(IDrawnElement)"/>
    public override void Draw(IDrawnElement drawable)
    {
        var opacity = ActiveOpacity;

        var element = (IDrawnElement<SkiaSharpDrawingContext>)drawable;

        if (element.HasTransform)
        {
            _ = Canvas.Save();

            var m = element.Measure();
            var o = element.TransformOrigin;
            var p = new SKPoint(element.X, element.Y);

            var xo = m.Width * o.X;
            var yo = m.Height * o.Y;

            if (element.HasRotation)
            {
                Canvas.Translate(p.X + xo, p.Y + yo);
                Canvas.RotateDegrees(element.RotateTransform);
                Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            if (element.HasTranslate)
            {
                var translate = element.TranslateTransform;
                Canvas.Translate(translate.X, translate.Y);
            }

            if (element.HasScale)
            {
                var scale = element.ScaleTransform;
                Canvas.Translate(p.X + xo, p.Y + yo);
                Canvas.Scale(scale.X, scale.Y);
                Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            if (element.HasSkew)
            {
                var skew = element.SkewTransform;
                Canvas.Translate(p.X + xo, p.Y + yo);
                Canvas.Skew(skew.X, skew.Y);
                Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            // DISABLED FOR NOW.
            //if (_hasTransform)
            //{
            //    var transform = Transform;
            //    context.Canvas.Concat(ref transform);
            //}
        }

        if (ActiveLvcPaint is null)
        {
            // if the active paint is null, we need to draw by the element paint

            if (element.Fill is not null)
                DrawByPaint(element.Fill, element, opacity);

            if (element.Stroke is not null)
                DrawByPaint(element.Stroke, element, opacity);

            if (element.Paint is not null)
                DrawByPaint(element.Paint, element, opacity);
        }
        else
        {
            // we will draw using the active paint while the element paint is null

            if (ActiveLvcPaint.PaintStyle.HasFlag(PaintStyle.Fill))
            {
                if (element.Fill is null)
                    DrawByActivePaint(element, opacity);
                else
                    DrawByPaint(element.Fill, element, opacity);
            }

            if (ActiveLvcPaint.PaintStyle.HasFlag(PaintStyle.Stroke))
            {
                if (element.Stroke is null)
                    DrawByActivePaint(element, opacity);
                else
                    DrawByPaint(element.Stroke, element, opacity);
            }
        }

        if (element.HasTransform) Canvas.Restore();
    }

    /// <inheritdoc cref="DrawingContext.InitializePaintTask(Paint)"/>
    public override void InitializePaintTask(Paint paint)
    {
        ActiveLvcPaint = paint;
        //ActiveSkiaPaint = paint.SKPaint; set by paint.InitializeTask

        paint.InitializeTask(this);
    }

    /// <inheritdoc cref="DrawingContext.DisposePaintTask(Paint)"/>
    public override void DisposePaintTask(Paint paint)
    {
        paint.Dispose();

        ActiveLvcPaint = null!;
        ActiveSkiaPaint = null!;
    }

    private void DrawByActivePaint(IDrawnElement<SkiaSharpDrawingContext> element, float opacity)
    {
        var hasGeometryOpacity = opacity < 1;

        if (hasGeometryOpacity) ActiveLvcPaint!.ApplyOpacityMask(this, opacity);
        element.Draw(this);
        if (hasGeometryOpacity) ActiveLvcPaint!.RestoreOpacityMask(this, opacity);
    }

    private void DrawByPaint(Paint paint, IDrawnElement<SkiaSharpDrawingContext> element, float opacity)
    {
        var hasGeometryOpacity = opacity < 1;

        var originalPaint = ActiveSkiaPaint;
        var originalTask = ActiveLvcPaint;

        paint.InitializeTask(this);

        if (hasGeometryOpacity) paint.ApplyOpacityMask(this, opacity);
        element.Draw(this);
        if (hasGeometryOpacity) paint.RestoreOpacityMask(this, opacity);

        paint.Dispose();

        ActiveSkiaPaint = originalPaint;
        ActiveLvcPaint = originalTask;
    }
}
