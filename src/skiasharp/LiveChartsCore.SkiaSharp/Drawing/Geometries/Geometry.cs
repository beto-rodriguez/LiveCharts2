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
using LiveChartsCore.SkiaSharpView.Motion;
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

        /// <summary>
        /// The transform
        /// </summary>
        protected readonly MatrixMotionProperty transform;

        /// <summary>
        /// The local transform
        /// </summary>
        protected readonly MatrixMotionProperty localTransform;

        /// <summary>
        /// The x
        /// </summary>
        protected readonly FloatMotionProperty x;

        /// <summary>
        /// The y
        /// </summary>
        protected readonly FloatMotionProperty y;

        /// <summary>
        /// The rotation
        /// </summary>
        protected readonly FloatMotionProperty rotation;

        /// <summary>
        /// The has custom transform
        /// </summary>
        protected bool hasCustomTransform = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry"/> class.
        /// </summary>
        public Geometry(bool hasCustomTransform = false)
        {
            x = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
            y = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
            transform = RegisterMotionProperty(new MatrixMotionProperty(nameof(Transform), SKMatrix.Identity));
            rotation = RegisterMotionProperty(new FloatMotionProperty(nameof(Rotation), 0));
            this.hasCustomTransform = hasCustomTransform;
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.X" />
        public float X { get => x.GetMovement(this); set => x.SetMovement(value, this); }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Y" />
        public float Y { get => y.GetMovement(this); set => y.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the matrix transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        public SKMatrix Transform
        {
            get => transform.GetMovement(this);
            set { transform.SetMovement(value, this); _hasTransform = !value.IsIdentity; }
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Rotation" />
        public float Rotation { get => rotation.GetMovement(this); set => rotation.SetMovement(value, this); }

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

            OnDraw(context, context.Paint);

            if (_hasTransform || hasRotation | hasCustomTransform) context.Canvas.Restore();
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
        public SizeF Measure(IDrawableTask<SkiaSharpDrawingContext> drawableTask)
        {
            var measure = OnMeasure((PaintTask)drawableTask);

            var r = Rotation;
            if (Math.Abs(r) > 0)
            {
                const double toRadias = Math.PI / 180;

                if (r < 0) r += 360;
                r %= 360;

                if (r > 180) r = 360 - r;
                if (r > 90 && r <= 180) r = 180 - r;

                var w = (float)(Math.Cos(r * toRadias) * measure.Width + Math.Sin(r * toRadias) * measure.Height);
                var h = (float)(Math.Sin(r * toRadias) * measure.Width + Math.Cos(r * toRadias) * measure.Height);

                measure = new SizeF(w, h);
            }

            return measure;
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
