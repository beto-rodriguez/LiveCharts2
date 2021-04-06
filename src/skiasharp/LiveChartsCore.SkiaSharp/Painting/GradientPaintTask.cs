using System;
using System.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Motion;
using LiveChartsCore.SkiaSharpView.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    public class GradientPaintTask : PaintTask
    {
        private SkiaSharpDrawingContext? drawingContext;
        private readonly SKColor start;
        private readonly SKColor end;

        public GradientPaintTask(SKColor start, SKColor end)
        {
            this.start = start;
            this.end = end;
        }

        public override IDrawableTask<SkiaSharpDrawingContext> CloneTask()
        {
            return new GradientPaintTask(start, end)
            {
                Style = Style,
                IsStroke = IsStroke,
                IsFill = IsFill,
                Color = Color,
                IsAntialias = IsAntialias,
                StrokeThickness = StrokeThickness
            };
        }

        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            var info = drawingContext.Info;

            skiaPaint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(info.Width / 2, 0),
                    new SKPoint(info.Width / 2, info.Height),
                    new SKColor[] { start, end },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Repeat);


            //skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = false;
            skiaPaint.StrokeWidth = 0;
            //skiaPaint.Style = SKPaintStyle.Fill;
            if (ClipRectangle != RectangleF.Empty)
            {
                drawingContext.Canvas.Save();
                drawingContext.Canvas.ClipRect(new SKRect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                this.drawingContext = drawingContext;
            }

            drawingContext.Paint = skiaPaint;
            drawingContext.PaintTask = this;
        }

        public override void Dispose()
        {
            if (ClipRectangle != RectangleF.Empty && drawingContext != null)
            {
                drawingContext.Canvas.Restore();
                drawingContext = null;
            }

            base.Dispose();
        }
    }
}
