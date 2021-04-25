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
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    /// <summary>
    /// Defines a set of geometries that will be painted using a solid color.,
    /// </summary>
    /// <seealso cref="PaintTask" />
    public class SkiaSharpPaintTask : PaintTask
    {
        private readonly FloatMotionProperty _strokeMiterTransition;
        private SkiaSharpDrawingContext _drawingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpPaintTask"/> class.
        /// </summary>
        public SkiaSharpPaintTask()
        {
            _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpPaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SkiaSharpPaintTask(SKColor color)
            : base(color)
        {
            _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
            Color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpPaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="strokeWidth">Width of the stroke.</param>
        public SkiaSharpPaintTask(SKColor color, float strokeWidth)
            : base(color)
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), strokeWidth));
            _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
            Color = color;
        }

        /// <summary>
        /// Gets or sets the stroke cap.
        /// </summary>
        /// <value>
        /// The stroke cap.
        /// </value>
        public SKStrokeCap StrokeCap { get; set; }

        /// <summary>
        /// Gets or sets the stroke join.
        /// </summary>
        /// <value>
        /// The stroke join.
        /// </value>
        public SKStrokeJoin StrokeJoin { get; set; }

        /// <summary>
        /// Gets or sets the stroke miter.
        /// </summary>
        /// <value>
        /// The stroke miter.
        /// </value>
        public float StrokeMiter
        {
            get => _strokeMiterTransition.GetMovement(this);
            set => _strokeMiterTransition.SetMovement(value, this);
        }

        /// <summary>
        /// Gets or sets the skia sharp path effect.
        /// </summary>
        /// <value>
        /// The path effect.
        /// </value>
        public SKPathEffect PathEffect { get; set; }

        /// <summary>
        /// Gets or sets the skia sharp shader.
        /// </summary>
        /// <value>
        /// The shader.
        /// </value>
        public SKShader Shader { get; set; }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.CloneTask" />
        public override IDrawableTask<SkiaSharpDrawingContext> CloneTask()
        {
            var clone = new SkiaSharpPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                IsFill = IsFill,
                Color = Color,
                IsAntialias = IsAntialias,
                PathEffect = PathEffect,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                StrokeMiter = StrokeMiter,
                StrokeThickness = StrokeThickness
            };

            return clone;
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
            skiaPaint.StrokeWidth = StrokeThickness;
            skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

            if (PathEffect != null) skiaPaint.PathEffect = PathEffect;
            if (Shader != null) skiaPaint.Shader = Shader;

            if (ClipRectangle != RectangleF.Empty)
            {
                _ = drawingContext.Canvas.Save();
                drawingContext.Canvas.ClipRect(
                    new SKRect(
                        ClipRectangle.X, ClipRectangle.Y, ClipRectangle.X + ClipRectangle.Width, ClipRectangle.Y + ClipRectangle.Height));
                _drawingContext = drawingContext;
            }

            drawingContext.Paint = skiaPaint;
            drawingContext.PaintTask = this;
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            var baseColor = context.PaintTask.Color;
            context.Paint.Color =
                new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, unchecked((byte)(255 * geometry.Opacity)));
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.ResetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            var baseColor = context.PaintTask.Color;
            context.Paint.Color = baseColor;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (ClipRectangle != RectangleF.Empty && _drawingContext != null)
            {
                _drawingContext.Canvas.Restore();
                _drawingContext = null;
            }

            base.Dispose();
        }
    }
}
