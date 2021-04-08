using System;
using System.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Motion;
using LiveChartsCore.SkiaSharpView.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    public class LinearGradientPaintTask : PaintTask
    {
        private static readonly SKPoint DefaultStartPoint = new SKPoint(0, 0);
        private static readonly SKPoint DefaultEndPoint = new SKPoint(1, 1);

        private readonly SKColor[] gradientStops;

        private readonly SKPoint startPoint;
        private readonly SKPoint endPoint;

        private SkiaSharpDrawingContext drawingContext;

        public LinearGradientPaintTask(SKColor[] gradientStops, SKPoint startPoint, SKPoint endPoint)
        {
            this.gradientStops = gradientStops;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public LinearGradientPaintTask(SKColor[] gradientStops)
            : this(gradientStops, DefaultStartPoint, DefaultEndPoint) { }

        public LinearGradientPaintTask(SKColor startColor, SKColor endColor, SKPoint startPoint, SKPoint endPoint)
            : this(new[] { startColor, endColor }, startPoint, endPoint) { }

        public LinearGradientPaintTask(SKColor start, SKColor end)
            : this(start, end, DefaultStartPoint, DefaultEndPoint) { }

        public override IDrawableTask<SkiaSharpDrawingContext> CloneTask()
        {
            return new LinearGradientPaintTask(gradientStops, startPoint, endPoint)
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
                    startPoint,
                    endPoint,
                    gradientStops,
                    SKShaderTileMode.Repeat);


            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = false;
            skiaPaint.StrokeWidth = 0;
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
