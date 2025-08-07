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
/// <param name="canvas">The canvas.</param>
/// <param name="clearOnBeginDraw">Indicates whether the canvas is cleared on frame draw.</param>
public class SkiaSharpDrawingContext(
    CoreMotionCanvas motionCanvas,
    SKImageInfo info,
    SKCanvas canvas,
    bool clearOnBeginDraw = true)
        : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkiaSharpDrawingContext"/> class.
    /// </summary>
    /// <param name="motionCanvas">The motion canvas.</param>
    /// <param name="info">The information.</param>
    /// <param name="canvas">The canvas.</param>
    /// <param name="background">The background.</param>
    /// <param name="clearOnBeginDraw">Indicates whether the canvas is cleared on frame draw.</param>
    public SkiaSharpDrawingContext(
        CoreMotionCanvas motionCanvas,
        SKImageInfo info,
        SKCanvas canvas,
        SKColor background,
        bool clearOnBeginDraw = true)
        : this(motionCanvas, info, canvas, clearOnBeginDraw)
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
    /// Gets or sets the canvas.
    /// </summary>
    /// <value>
    /// The canvas.
    /// </value>
    public SKCanvas Canvas { get; } = canvas;

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
            Color = SKColors.White,
            TextSize = 14,
            IsAntialias = true
        };

        using var backgroundPaint = new SKPaint
        {
            Color = SKColors.Black.WithAlpha(180),
            Style = SKPaintStyle.Fill
        };

        var lines = log.Split('`');

        Canvas.DrawRect(new(10, 0, 400, (p.TextSize + 4f) * lines.Length), backgroundPaint);

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;
            Canvas.DrawText(
                line,
                new SKPoint(10, 10 + 2 + (p.TextSize + 4f) * i),
                p);
        }
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
                    DrawElement(element, opacity);
                else
                    DrawByPaint(element.Fill, element, opacity);
            }

            if (ActiveLvcPaint.PaintStyle.HasFlag(PaintStyle.Stroke))
            {
                if (element.Stroke is null)
                    DrawElement(element, opacity);
                else
                    DrawByPaint(element.Stroke, element, opacity);
            }
        }

        if (element.HasTransform) Canvas.Restore();
    }

    /// <inheritdoc cref="DrawingContext.SelectPaint(Paint)"/>
    public override void SelectPaint(Paint paint)
    {
        ActiveLvcPaint = paint;
        //ActiveSkiaPaint = paint.SKPaint; set by paint.InitializeTask
        PaintMotionProperty.s_activePaint = paint;

        paint.OnPaintStarted(this);
    }

    /// <inheritdoc cref="DrawingContext.ClearPaintSelection(Paint)"/>
    public override void ClearPaintSelection(Paint paint)
    {
        paint.OnPaintFinished(this);

        ActiveLvcPaint = null!;
        ActiveSkiaPaint = null!;
        PaintMotionProperty.s_activePaint = null!;
    }

    private void DrawByPaint(Paint paint, IDrawnElement<SkiaSharpDrawingContext> element, float opacity)
    {
        var originalPaint = ActiveSkiaPaint;
        var originalTask = ActiveLvcPaint;

        // hack for now...
        // ActiveLvcPaint must be null for this kind of draw method...
        // normally used to draw tooltips and legends
        // Improve this? maybe a cleaner way?
        if (paint != MeasureTask.Instance)
        {
            ActiveLvcPaint = paint;
            paint.OnPaintStarted(this);
        }

        DrawElement(element, opacity);

        paint.OnPaintFinished(this);

        ActiveSkiaPaint = originalPaint;
        ActiveLvcPaint = originalTask;
    }

    private void DrawElement(IDrawnElement<SkiaSharpDrawingContext> element, float opacity)
    {
        var hasGeometryOpacity =
            ActiveLvcPaint is not null &&
            opacity < 1;

        var hasShadow =
            ActiveLvcPaint is not null &&
            element.DropShadow is not null &&
            element.DropShadow != LvcDropShadow.Empty;

        SKImageFilter? originalFilter = null;

        if (hasGeometryOpacity)
        {
            ActiveLvcPaint!.ApplyOpacityMask(this, opacity);
        }

        if (hasShadow)
        {
            var shadow = element.DropShadow!;
            originalFilter = ActiveSkiaPaint.ImageFilter;

            ActiveSkiaPaint.ImageFilter = SKImageFilter.CreateDropShadow(
                shadow.Dx, shadow.Dy,
                shadow.SigmaX, shadow.SigmaY,
                new(shadow.Color.R, shadow.Color.G, shadow.Color.B, shadow.Color.A));
        }

        element.Draw(this);

        if (hasShadow)
        {
            ActiveSkiaPaint.ImageFilter!.Dispose();
            ActiveSkiaPaint.ImageFilter = originalFilter;
        }

        if (hasGeometryOpacity)
        {
            ActiveLvcPaint!.RestoreOpacityMask(this, opacity);
        }
    }
}
