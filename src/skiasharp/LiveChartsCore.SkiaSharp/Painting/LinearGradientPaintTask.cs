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
        private static readonly SKPoint s_defaultStartPoint = new SKPoint(0, 0);
        private static readonly SKPoint s_defaultEndPoint = new SKPoint(1, 1);
        private readonly SKColor[] _gradientStops;
        private readonly SKPoint _startPoint;
        private readonly SKPoint _endPoint;
        private SkiaSharpDrawingContext _drawingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="gradientStops">The gradient stops.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        public LinearGradientPaintTask(SKColor[] gradientStops, SKPoint startPoint, SKPoint endPoint)
        {
            _gradientStops = gradientStops;
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="gradientStops">The gradient stops.</param>
        public LinearGradientPaintTask(SKColor[] gradientStops)
            : this(gradientStops, s_defaultStartPoint, s_defaultEndPoint) { }

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
            : this(start, end, s_defaultStartPoint, s_defaultEndPoint) { }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.CloneTask" />
        public override IDrawableTask<SkiaSharpDrawingContext> CloneTask()
        {
            return new LinearGradientPaintTask(_gradientStops, _startPoint, _endPoint)
            {
                Style = Style,
                IsStroke = IsStroke,
                IsFill = IsFill,
                Color = Color,
                IsAntialias = IsAntialias,
                StrokeThickness = StrokeThickness
            };
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.ResetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IDrawableTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Shader = SKShader.CreateLinearGradient(
                    _startPoint,
                    _endPoint,
                    _gradientStops,
                    SKShaderTileMode.Repeat);


            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = false;
            skiaPaint.StrokeWidth = 0;
            if (ClipRectangle != RectangleF.Empty)
            {
                _ = drawingContext.Canvas.Save();
                drawingContext.Canvas.ClipRect(new SKRect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                _drawingContext = drawingContext;
            }

            drawingContext.Paint = skiaPaint;
            drawingContext.PaintTask = this;
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
