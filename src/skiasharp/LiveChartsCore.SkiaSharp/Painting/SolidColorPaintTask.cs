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
    /// Defines a set of geometries that will be painted using a solid color.
    /// </summary>
    /// <seealso cref="PaintTask" />
    public class SolidColorPaintTask : PaintTask
    {
        private SkiaSharpDrawingContext? _drawingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorPaintTask"/> class.
        /// </summary>
        public SolidColorPaintTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorPaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidColorPaintTask(SKColor color)
            : base(color)
        {
            Color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorPaintTask"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="strokeWidth">Width of the stroke.</param>
        public SolidColorPaintTask(SKColor color, float strokeWidth)
            : base(color)
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), strokeWidth));
            Color = color;
        }


        /// <inheritdoc cref="IPaintTask{TDrawingContext}.CloneTask" />
        public override IPaintTask<SkiaSharpDrawingContext> CloneTask()
        {
            var clone = new SolidColorPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                IsFill = IsFill,
                Color = Color,
                IsAntialias = IsAntialias,
                StrokeThickness = StrokeThickness,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                StrokeMiter = StrokeMiter,
                PathEffect = PathEffect?.Clone(),
                ImageFilter = ImageFilter?.Clone()
            };

            return clone;
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
        {
            skiaPaint ??= new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
            skiaPaint.StrokeWidth = StrokeThickness;
            skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

            if (PathEffect is not null)
            {
                PathEffect.CreateEffect(drawingContext);
                skiaPaint.PathEffect = PathEffect.SKPathEffect;
            }

            if (ImageFilter is not null)
            {
                ImageFilter.CreateFilter(drawingContext);
                skiaPaint.ImageFilter = ImageFilter.SKImageFilter;
            }

            var clip = GetClipRectangle(drawingContext.MotionCanvas);
            if (clip != RectangleF.Empty)
            {
                _ = drawingContext.Canvas.Save();
                drawingContext.Canvas.ClipRect(new SKRect(clip.X, clip.Y, clip.X + clip.Width, clip.Y + clip.Height));
                _drawingContext = drawingContext;
            }

            drawingContext.Paint = skiaPaint;
            drawingContext.PaintTask = this;
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            if (context.PaintTask is null || context.Paint is null) return;

            var baseColor = context.PaintTask.Color;
            context.Paint.Color =
                new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, unchecked((byte)(255 * geometry.Opacity)));
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.ResetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            if (context.PaintTask is null || context.Paint is null) return;
            if (ImageFilter is not null) ImageFilter.Dispose();

            var baseColor = context.PaintTask.Color;
            context.Paint.Color = baseColor;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (PathEffect is not null) PathEffect.Dispose();

            if (_drawingContext is not null && GetClipRectangle(_drawingContext.MotionCanvas) != RectangleF.Empty)
            {
                _drawingContext.Canvas.Restore();
                _drawingContext = null;
            }

            base.Dispose();
        }
    }
}
