// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharp.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{

    public abstract class Geometry : Animatable, IGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>
    {
        private bool hasTransform = false;

        protected readonly MatrixMotionProperty transform;
        protected readonly FloatMotionProperty x;
        protected readonly FloatMotionProperty y;
        protected readonly FloatMotionProperty rotation;

        public Geometry()
        {
            x = RegisterTransitionProperty(new FloatMotionProperty(nameof(X), 0));
            y = RegisterTransitionProperty(new FloatMotionProperty(nameof(Y), 0));
            transform = RegisterTransitionProperty(new MatrixMotionProperty(nameof(Transform)));
            rotation = RegisterTransitionProperty(new FloatMotionProperty(nameof(Rotation), 0));
        }

        public float X { get => x.GetMovement(this); set => x.SetMovement(value, this); }

        public float Y { get => y.GetMovement(this); set => y.SetMovement(value, this); }
        
        public SKMatrix Transform
        {
            get => transform.GetMovement(this);
            set
            {
                transform.SetMovement(value, this);
                if (value != SKMatrix.Identity) hasTransform = true;
            }
        }

        public float Rotation { get => rotation.GetMovement(this); set => rotation.SetMovement(value, this); }

        public IGeometry<SkiaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        public void Draw(SkiaDrawingContext context)
        {
            var hasRotation = Rotation != 0;

            if (hasTransform || hasRotation)
            {
                context.Canvas.Save();

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

        public abstract void OnDraw(SkiaDrawingContext context, SKPaint paint);

        public abstract SKSize Measure(SkiaDrawingContext context, SKPaint paint);

        public virtual SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint) => new SKPoint(X, Y);

        protected virtual IGeometry<SkiaDrawingContext> GetHighlitableGeometry() => this;
    }
}
