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
using Vortice;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice.Drawing;

public class VorticeDrawingContext(
    CoreMotionCanvas motionCanvas, ID2D1HwndRenderTarget renderTarget, Color4 background)
        : DrawingContext
{
    public CoreMotionCanvas MotionCanvas { get; } = motionCanvas;
    public ID2D1HwndRenderTarget RenderTarget { get; } = renderTarget;
    public Color4 Background { get; } = background;
    public ID2D1Brush ActiveBrush { get; set; } = null!;

    private ID2D1HwndRenderTarget _previousRenderTarget = null!;
    private static ID2D1SolidColorBrush? s_logTextBrush;
    private static ID2D1SolidColorBrush? s_logBackgroundBrush;
    private static IDWriteTextFormat? s_logTextFormat;

    public override void LogOnCanvas(string log)
    {
        var app = Application.Current!;
        var writeFactory = app.WriteFactory;
        var textRenderer = app.TextRenderer;

        if (_previousRenderTarget != RenderTarget)
        {
            s_logTextBrush?.Dispose();
            s_logTextBrush = null;
            s_logBackgroundBrush?.Dispose();
            s_logBackgroundBrush = null;
            _previousRenderTarget = RenderTarget;
        }

        s_logTextBrush ??= RenderTarget.CreateSolidColorBrush(new Color4(255, 255, 255, 255));
        s_logBackgroundBrush ??= RenderTarget.CreateSolidColorBrush(new Color4(0, 0, 0, 0.4f));
        s_logTextFormat ??= writeFactory.CreateTextFormat("Consolas", FontWeight.Regular, FontStyle.Normal, 16);

        textRenderer.ActiveBrush = s_logTextBrush;

        var lines = log.Replace("`", Environment.NewLine);
        RenderTarget.FillRectangle(new(10, 0, 400, 150), s_logBackgroundBrush);

        using var textLayout = writeFactory.CreateTextLayout(lines, s_logTextFormat, 400, float.MaxValue);

        textLayout.Draw(textRenderer, 10, 10 + 2 + (16 + 4f));
    }

    internal override void OnBeginDraw() =>
        RenderTarget.Clear(Background);

    internal override void OnEndDraw()
    { }

    internal override void OnBeginZone(CanvasZone zone)
    {
        if (zone.Clip == LvcRectangle.Empty) return;

        //zone.StateId = Canvas.Save();
        //Canvas.ClipRect(new(zone.Clip.X, zone.Clip.Y, zone.Clip.X + zone.Clip.Width, zone.Clip.Y + zone.Clip.Height));
    }

    internal override void OnEndZone(CanvasZone zone)
    {
        if (zone.Clip == LvcRectangle.Empty) return;

        //Canvas.RestoreToCount(zone.StateId);
    }

    internal override void Draw(IDrawnElement drawable)
    {
        var opacity = ActiveOpacity;

        var element = (IDrawnElement<VorticeDrawingContext>)drawable;

        if (element.HasTransform)
        {
            // todo.. transform
        }

        if (ActiveLvcPaint is null)
        {
            // if the active paint is null, we need to draw by the element paint

            var elementFill = element.Fill;
            var elementStroke = element.Stroke;
            var elementPaint = element.Paint;

            if (elementFill is not null)
                DrawByPaint(elementFill, element, opacity);

            if (elementStroke is not null)
                DrawByPaint(elementStroke, element, opacity);

            if (elementPaint is not null)
                DrawByPaint(elementPaint, element, opacity);
        }
        else
        {
            // we will draw using the active paint while the element paint is null

            if (ActiveLvcPaint.PaintStyle.HasFlag(PaintStyle.Fill))
            {
                var elementFill = element.Fill;

                if (elementFill is null)
                    DrawElement(element, opacity);
                else
                    DrawByPaint(elementFill, element, opacity);
            }

            if (ActiveLvcPaint.PaintStyle.HasFlag(PaintStyle.Stroke))
            {
                var elementStroke = element.Stroke;

                if (elementStroke is null)
                    DrawElement(element, opacity);
                else
                    DrawByPaint(elementStroke, element, opacity);
            }
        }

        if (element.HasTransform)
        {
            // todo.. restore transform
        }
    }

    internal override void SelectPaint(Paint paint)
    {
        ActiveLvcPaint = paint;
        PaintMotionProperty.s_activePaint = paint;

        paint.OnPaintStarted(this, null);
    }

    internal override void ClearPaintSelection(Paint paint)
    {
        paint.OnPaintFinished(this, null);

        ActiveLvcPaint = null!;
        ActiveBrush = null!;
        PaintMotionProperty.s_activePaint = null!;
    }

    private void DrawByPaint(Paint paint, IDrawnElement<VorticeDrawingContext> element, float opacity)
    {
        var originalPaint = ActiveBrush;
        var originalTask = ActiveLvcPaint;

        // hack for now...
        // ActiveLvcPaint must be null for this kind of draw method...
        // normally used to draw tooltips and legends
        // Improve this? maybe a cleaner way?
        if (paint != MeasureTask.Instance)
        {
            ActiveLvcPaint = paint;
            paint.OnPaintStarted(this, element);
        }

        DrawElement(element, opacity);

        paint.OnPaintFinished(this, element);

        ActiveBrush = originalPaint;
        ActiveLvcPaint = originalTask;
    }

    private void DrawElement(IDrawnElement<VorticeDrawingContext> element, float opacity)
    {
        var hasGeometryOpacity =
            ActiveLvcPaint is not null &&
            opacity < 1;

        var hasShadow =
            ActiveLvcPaint is not null &&
            element.DropShadow is not null &&
            element.DropShadow != LvcDropShadow.Empty;

        if (hasGeometryOpacity)
        {
            ActiveLvcPaint!.ApplyOpacityMask(this, opacity, element);
        }

        if (hasShadow)
        {
            // todo shadows...
            //var shadow = element.DropShadow!;
            //originalFilter = ActiveSkiaPaint.ImageFilter;

            //ActiveSkiaPaint.ImageFilter = SKImageFilter.CreateDropShadow(
            //    shadow.Dx, shadow.Dy,
            //    shadow.SigmaX, shadow.SigmaY,
            //    new(shadow.Color.R, shadow.Color.G, shadow.Color.B, shadow.Color.A));
        }

        element.Draw(this);

        if (hasShadow)
        {
            // todo restore shadow...
            //ActiveSkiaPaint.ImageFilter!.Dispose();
            //ActiveSkiaPaint.ImageFilter = originalFilter;
        }

        if (hasGeometryOpacity)
        {
            ActiveLvcPaint!.RestoreOpacityMask(this, opacity, element);
        }
    }

    //private static SKMatrix BuildTransform(IDrawnElement<SkiaSharpDrawingContext> element)
    //{
    //    return ... transform...
    //}
}
