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

using Avalonia;
using LiveChartsCore.AvaloniaView.Motion;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using System;
using System.Drawing;

namespace LiveChartsCore.AvaloniaView.Drawing
{
    public abstract class Geometry : Drawable, IGeometry<AvaloniaDrawingContext>, IVisualChartPoint<AvaloniaDrawingContext>
    {
        private bool hasTransform = false;
        private bool hasRotation = false;

        protected readonly MatrixMotionProperty transform;
        protected readonly FloatMotionProperty x;
        protected readonly FloatMotionProperty y;
        protected readonly FloatMotionProperty rotation;

        public Geometry()
        {
            x = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
            y = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
            transform = RegisterMotionProperty(new MatrixMotionProperty(nameof(Transform)));
            rotation = RegisterMotionProperty(new FloatMotionProperty(nameof(Rotation), 0));
        }

        public float X { get => x.GetMovement(this); set => x.SetMovement(value, this); }

        public float Y { get => y.GetMovement(this); set => y.SetMovement(value, this); }

        public Matrix Transform
        {
            get => transform.GetMovement(this);
            set
            {
                transform.SetMovement(value, this);
                hasTransform = !value.IsIdentity;
            }
        }

        public float Rotation
        {
            get => rotation.GetMovement(this);
            set
            {
                rotation.SetMovement(value, this);
                hasRotation = Math.Abs(value) > 0.01;
            }
        }

        public IDrawable<AvaloniaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        public override void Draw(AvaloniaDrawingContext context)
        {
            var hasRotation = Rotation != 0;

            var t = Matrix.Identity;
            if (hasRotation) t *= Matrix.CreateRotation(Rotation * Math.PI / 180);
            if (hasTransform) t *= Transform;

            OnDraw(context);
        }

        public abstract void OnDraw(AvaloniaDrawingContext context);

        public SizeF Measure(IDrawableTask<AvaloniaDrawingContext> drawableTask)
        {
            var measure = OnMeasure(drawableTask);

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

        protected abstract SizeF OnMeasure(IDrawableTask<AvaloniaDrawingContext> paintTaks);

        protected virtual PointF GetPosition(AvaloniaDrawingContext context, IDrawableTask<AvaloniaDrawingContext> drawableTask) => new(X, Y);

        protected virtual IGeometry<AvaloniaDrawingContext> GetHighlitableGeometry() => this;
    }
}
