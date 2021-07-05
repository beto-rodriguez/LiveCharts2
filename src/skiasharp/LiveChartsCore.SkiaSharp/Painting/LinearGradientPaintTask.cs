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
    /// Defines a set of geometries that will be painted using a linear gradient shader.
    /// </summary>
    /// <seealso cref="PaintTask" />
    public class LinearGradientPaintTask : PaintTask
    {
        private static readonly SKPoint s_defaultStartPoint = new(0, 0.5f);
        private static readonly SKPoint s_defaultEndPoint = new(1, 0.5f);
        private readonly SKColor[] _gradientStops;
        private readonly SKPoint _startPoint;
        private readonly SKPoint _endPoint;
        private readonly float[]? _colorPos;
        private readonly SKShaderTileMode _tileMode;
        private SkiaSharpDrawingContext? _drawingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="gradientStops">The gradient stops.</param>
        /// <param name="startPoint">
        /// The start point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
        /// </param>
        /// <param name="endPoint">
        /// The end point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
        /// </param>
        /// <param name="colorPos">
        /// An array of integers in the range of 0 to 1.
        /// These integers indicate the relative positions of the colors, You can set that argument to null to equally
        /// space the colors, default is null.
        /// </param>
        /// <param name="tileMode">
        /// The shader tile mode, default is <see cref="SKShaderTileMode.Repeat"/>.
        /// </param>
        public LinearGradientPaintTask(
            SKColor[] gradientStops,
            SKPoint startPoint,
            SKPoint endPoint,
            float[]? colorPos = null,
            SKShaderTileMode tileMode = SKShaderTileMode.Repeat)
        {
            _gradientStops = gradientStops;
            _startPoint = startPoint;
            _endPoint = endPoint;
            _colorPos = colorPos;
            _tileMode = tileMode;
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
        /// <param name="startPoint">
        /// The start point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
        /// </param>
        /// <param name="endPoint">
        /// The end point, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end.
        /// </param>
        public LinearGradientPaintTask(SKColor startColor, SKColor endColor, SKPoint startPoint, SKPoint endPoint)
            : this(new[] { startColor, endColor }, startPoint, endPoint) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientPaintTask"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public LinearGradientPaintTask(SKColor start, SKColor end)
            : this(start, end, s_defaultStartPoint, s_defaultEndPoint) { }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.CloneTask" />
        public override IPaintTask<SkiaSharpDrawingContext> CloneTask()
        {
            return new LinearGradientPaintTask(_gradientStops, _startPoint, _endPoint, _colorPos, _tileMode)
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
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.SetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void SetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.ResetOpacity(TDrawingContext, IGeometry{TDrawingContext})" />
        public override void ResetOpacity(SkiaSharpDrawingContext context, IGeometry<SkiaSharpDrawingContext> geometry)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IPaintTask{TDrawingContext}.InitializeTask(TDrawingContext)" />
        public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
        {
            skiaPaint ??= new SKPaint();

            var size = GetDrawRectangleSize(drawingContext);

            var xf = size.Location.X;
            var xt = xf + size.Width;

            var yf = size.Location.Y;
            var yt = yf + size.Height;

            var start = new SKPoint(xf + (xt - xf) * _startPoint.X, yf + (yt - yf) * _startPoint.Y);
            var end = new SKPoint(xf + (xt - xf) * _endPoint.X, yf + (yt - yf) * _endPoint.Y);

            skiaPaint.Shader = SKShader.CreateLinearGradient(
                    start,
                    end,
                    _gradientStops,
                    _colorPos,
                    _tileMode);

            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = true;
            skiaPaint.StrokeWidth = StrokeThickness;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (PathEffect is not null) PathEffect.Dispose();
            if (ImageFilter is not null) ImageFilter.Dispose();

            if (_drawingContext is not null && GetClipRectangle(_drawingContext.MotionCanvas) != RectangleF.Empty)
            {
                _drawingContext.Canvas.Restore();
                _drawingContext = null;
            }

            base.Dispose();
        }

        private SKRect GetDrawRectangleSize(SkiaSharpDrawingContext drawingContext)
        {
            var clip = GetClipRectangle(drawingContext.MotionCanvas);

            return clip == RectangleF.Empty
                ? new SKRect(0, 0, drawingContext.Info.Width, drawingContext.Info.Width)
                : new SKRect(clip.X, clip.Y, clip.X + clip.Width, clip.Y + clip.Height);
        }
    }
}
