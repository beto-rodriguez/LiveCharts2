using System.Drawing;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    /// <summary>
    /// Defines a set of geometries that will be painted using a linear gradient shader.,
    /// </summary>
    /// <seealso cref="PaintTask" />
    public class LinearGradientPaintTask : PaintTask
    {
        private static readonly SKPoint DefaultStartPoint = new SKPoint(0, 0);
        private static readonly SKPoint DefaultEndPoint = new SKPoint(1, 1);
        private readonly SKColor[] gradientStops;
        private readonly SKPoint startPoint;
        private readonly SKPoint endPoint;
        private SkiaSharpDrawingContext drawingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="gradientStops">The gradient stops.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        public LinearGradientPaintTask(SKColor[] gradientStops, SKPoint startPoint, SKPoint endPoint)
        {
            this.gradientStops = gradientStops;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="gradientStops">The gradient stops.</param>
        public LinearGradientPaintTask(SKColor[] gradientStops)
            : this(gradientStops, DefaultStartPoint, DefaultEndPoint) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        public LinearGradientPaintTask(SKColor startColor, SKColor endColor, SKPoint startPoint, SKPoint endPoint)
            : this(new[] { startColor, endColor }, startPoint, endPoint) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LinearGradientPaintTask(SKColor start, SKColor end)
            : this(start, end, DefaultStartPoint, DefaultEndPoint) { }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.CloneTask" />
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

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
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
                _ = drawingContext.Canvas.Save();
                drawingContext.Canvas.ClipRect(new SKRect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                this.drawingContext = drawingContext;
            }

            drawingContext.Paint = skiaPaint;
            drawingContext.PaintTask = this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
