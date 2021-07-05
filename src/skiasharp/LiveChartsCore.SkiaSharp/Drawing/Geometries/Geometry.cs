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
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <inheritdoc cref="IGeometry{TDrawingContext}" />
    public abstract class Geometry : Drawable, IGeometry<SkiaSharpDrawingContext>, IVisualChartPoint<SkiaSharpDrawingContext>
    {
        private bool _hasTransform = false;
        private SKMatrix _transform = SKMatrix.Identity;

        /// <summary>
        /// The opacity property
        /// </summary>
        protected FloatMotionProperty opacityProperty;

        /// <summary>
        /// The x
        /// </summary>
        protected FloatMotionProperty xProperty;

        /// <summary>
        /// The y
        /// </summary>
        protected FloatMotionProperty yProperty;

        /// <summary>
        /// The rotation
        /// </summary>
        protected FloatMotionProperty rotationProperty;

        /// <summary>
        /// The has custom transform
        /// </summary>
        protected bool hasCustomTransform = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry"/> class.
        /// </summary>
        protected Geometry(bool hasCustomTransform = false)
        {
            xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
            yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
            rotationProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Rotation), 0));
            opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1));
            this.hasCustomTransform = hasCustomTransform;
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.X" />
        public float X { get => xProperty.GetMovement(this); set => xProperty.SetMovement(value, this); }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Y" />
        public float Y { get => yProperty.GetMovement(this); set => yProperty.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the matrix transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        public SKMatrix Transform { get => _transform; set { _transform = value; _hasTransform = !value.IsIdentity; } }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Opacity" />
        public float Opacity { get => opacityProperty.GetMovement(this); set => opacityProperty.SetMovement(value, this); }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Rotation" />
        public float Rotation { get => rotationProperty.GetMovement(this); set => rotationProperty.SetMovement(value, this); }

        /// <inheritdoc cref="IVisualChartPoint{TDrawingContext}.HighlightableGeometry" />
        public IDrawable<SkiaSharpDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        /// <summary>
        /// Draws the geometry in the user interface.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Draw(SkiaSharpDrawingContext context)
        {
            var hasRotation = Rotation != 0;

            if (_hasTransform || hasRotation || hasCustomTransform)
            {
                _ = context.Canvas.Save();

                if (hasRotation)
                {
                    var p = GetPosition(context, context.Paint);
                    context.Canvas.Translate(p.X, p.Y);
                    context.Canvas.RotateDegrees(Rotation);
                    context.Canvas.Translate(-p.X, -p.Y);
                }

                if (_hasTransform || hasCustomTransform)
                {
                    var transform = GetTransform(context);
                    context.Canvas.Concat(ref transform);
                }
            }

            BeforeDraw(context);
            OnDraw(context, context.Paint);
            AfterDraw(context);

            if (_hasTransform || hasRotation || hasCustomTransform) context.Canvas.Restore();
        }

        /// <summary>
        /// Called when the geometry is drawn.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="paint">The paint.</param>
        public abstract void OnDraw(SkiaSharpDrawingContext context, SKPaint paint);

        /// <summary>
        /// Measures the geometry.
        /// </summary>
        /// <param name="drawableTask">The drawable task.</param>
        /// <returns>the size of the geometry.</returns>
        public SizeF Measure(IPaintTask<SkiaSharpDrawingContext> drawableTask)
        {
            var measure = OnMeasure((PaintTask)drawableTask);

            var r = Rotation;
            if (Math.Abs(r) > 0)
            {
                const double toRadias = Math.PI / 180;

                r %= 360;
                if (r < 0) r += 360;

                if (r > 180) r = 360 - r;
                if (r is > 90 and <= 180) r = 180 - r;

                var rRadians = r * toRadias;

                var w = (float)(Math.Cos(rRadians) * measure.Width + Math.Sin(rRadians) * measure.Height);
                var h = (float)(Math.Sin(rRadians) * measure.Width + Math.Cos(rRadians) * measure.Height);

                measure = new SizeF(w, h);
            }

            return measure;
        }

        /// <summary>
        /// Called before the draw.
        /// </summary>
        protected virtual void BeforeDraw(SkiaSharpDrawingContext context)
        {
            if (Opacity == 1) return;

            context.PaintTask.SetOpacity(context, this);
        }

        /// <summary>
        /// Called after the draw.
        /// </summary>
        protected virtual void AfterDraw(SkiaSharpDrawingContext context)
        {
            if (Opacity == 1) return;

            context.PaintTask.ResetOpacity(context, this);
        }

        /// <summary>
        /// Called when the geometry is measured.
        /// </summary>
        /// <param name="paintTaks">The paint task.</param>
        /// <returns>the size of the geometry</returns>
        protected abstract SizeF OnMeasure(PaintTask paintTaks);

        /// <summary>
        /// Gets the actual transform.
        /// </summary>
        /// <returns></returns>
        protected virtual SKMatrix GetTransform(SkiaSharpDrawingContext context)
        {
            return Transform;
        }

        /// <summary>
        /// Gets the position of the geometry from the top left corner of the view.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="paint">The paint.</param>
        /// <returns>the position.</returns>
        protected virtual SKPoint GetPosition(SkiaSharpDrawingContext context, SKPaint paint)
        {
            return new SKPoint(X, Y);
        }

        /// <summary>
        /// Gets the highlitable geometry.
        /// </summary>
        /// <returns></returns>
        protected virtual IGeometry<SkiaSharpDrawingContext> GetHighlitableGeometry()
        {
            return this;
        }
    }
}
