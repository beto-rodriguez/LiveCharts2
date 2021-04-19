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
        private bool hasTransform = false;
        private bool hasRotation = false;

        /// <summary>
        /// The transform
        /// </summary>
        protected readonly MatrixMotionProperty transform;

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
        /// Initializes a new instance of the <see cref="Geometry"/> class.
        /// </summary>
        public Geometry()
        {
            x = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
            y = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
            transform = RegisterMotionProperty(new MatrixMotionProperty(nameof(Transform)));
            rotation = RegisterMotionProperty(new FloatMotionProperty(nameof(Rotation), 0));
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.X" />
        public float X
        {
            get => x.GetMovement(this);
            set => x.SetMovement(value, this);
        }

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
            set
            {
                transform.SetMovement(value, this);
                hasTransform = !value.IsIdentity;
            }
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Rotation" />
        public float Rotation
        {
            get => rotation.GetMovement(this);
            set
            {
                rotation.SetMovement(value, this);
                hasRotation = Math.Abs(value) > 0.01;
            }
        }

        /// <inheritdoc cref="IVisualChartPoint{TDrawingContext}.HighlightableGeometry" />
        public IDrawable<SkiaSharpDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        /// <summary>
        /// Draws the geometry in the user interface.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Draw(SkiaSharpDrawingContext context)
        {
            var hasRotation = Rotation != 0;

            if (hasTransform || hasRotation)
            {
                _ = context.Canvas.Save();

                if (hasRotation)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = SKMatrix.CreateRotationDegrees(Rotation);
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }

                if (hasTransform)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = Transform;
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }
            }

            OnDraw(context, context.Paint);

            if (hasTransform || hasRotation) context.Canvas.Restore();
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
                r %= 90;

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
