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

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <inheritdoc cref="IGeometry{TDrawingContext}" />
    public abstract class Geometry : Drawable, IGeometry<SkiaSharpDrawingContext>, IVisualChartPoint<SkiaSharpDrawingContext>
    {
        private readonly bool _hasGeometryTransform = false;
        private readonly FloatMotionProperty _opacityProperty;
        private readonly FloatMotionProperty _xProperty;
        private readonly FloatMotionProperty _yProperty;
        private readonly FloatMotionProperty _rotationProperty;
        private readonly PointMotionProperty _transformOriginProperty;
        private readonly PointMotionProperty _scaleProperty;
        private readonly PointMotionProperty _skewProperty;
        private readonly PointMotionProperty _translateProperty;
        private readonly SKMatrixMotionProperty _transformProperty;
        private bool _hasTransform = false;
        private bool _hasRotation = false;
        private bool _hasScale = false;
        private bool _hasSkew = false;
        private bool _hasTranslate = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry"/> class.
        /// </summary>
        protected Geometry(bool hasGeometryTransform = false)
        {
            _hasGeometryTransform = hasGeometryTransform;
            _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
            _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
            _opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1));
            _transformOriginProperty = RegisterMotionProperty(
                new PointMotionProperty(nameof(TransformOrigin), new LvcPoint(0.5f, 0.5f)));
            _translateProperty = RegisterMotionProperty(
                new PointMotionProperty(nameof(TranslateTransform), new LvcPoint(0, 0)));
            _rotationProperty = RegisterMotionProperty(
                new FloatMotionProperty(nameof(RotateTransform), 0));
            _scaleProperty = RegisterMotionProperty(
                new PointMotionProperty(nameof(ScaleTransform), new LvcPoint(1, 1)));
            _skewProperty = RegisterMotionProperty(
                new PointMotionProperty(nameof(SkewTransform), new LvcPoint(1, 1)));
            _transformProperty = RegisterMotionProperty(
                new SKMatrixMotionProperty(nameof(Transform), SKMatrix.Identity));
        }

        private bool HasTransform => _hasGeometryTransform || _hasTranslate || _hasRotation || _hasScale || _hasSkew || _hasTransform;

        /// <inheritdoc cref="IGeometry{TDrawingContext}.X" />
        public float X { get => _xProperty.GetMovement(this); set => _xProperty.SetMovement(value, this); }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.Y" />
        public float Y { get => _yProperty.GetMovement(this); set => _yProperty.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the transform origin.
        /// </summary>
        public LvcPoint TransformOrigin
        {
            get => _transformOriginProperty.GetMovement(this);
            set => _transformOriginProperty.SetMovement(value, this);
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.TranslateTransform" />
        public LvcPoint TranslateTransform
        {
            get => _translateProperty.GetMovement(this);
            set
            {
                _translateProperty.SetMovement(value, this);
                _hasTranslate = value.X != 0 || value.Y != 0;
            }
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.RotateTransform" />
        public float RotateTransform
        {
            get => _rotationProperty.GetMovement(this);
            set
            {
                _rotationProperty.SetMovement(value, this);
                _hasRotation = value != 0;
            }
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.ScaleTransform" />
        public LvcPoint ScaleTransform
        {
            get => _scaleProperty.GetMovement(this);
            set
            {
                _scaleProperty.SetMovement(value, this);
                _hasScale = value.X != 1 || value.Y != 1;
            }
        }

        /// <inheritdoc cref="IGeometry{TDrawingContext}.SkewTransform" />
        public LvcPoint SkewTransform
        {
            get => _skewProperty.GetMovement(this);
            set
            {
                _skewProperty.SetMovement(value, this);
                _hasSkew = value.X != 0 || value.Y != 0;
            }
        }

        /// <summary>
        /// Gets or sets the matrix transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        public SKMatrix Transform
        {
            get => _transformProperty.GetMovement(this);
            set
            {
                _transformProperty.SetMovement(value, this);
                _hasTransform = !value.IsIdentity;
            }
        }

        /// <inheritdoc cref="IPaintable{TDrawingContext}.Opacity" />
        public float Opacity { get => _opacityProperty.GetMovement(this); set => _opacityProperty.SetMovement(value, this); }

        /// <inheritdoc cref="IPaintable{TDrawingContext}.Stroke" />
        public IPaint<SkiaSharpDrawingContext>? Stroke { get; set; }

        /// <inheritdoc cref="IPaintable{TDrawingContext}.Fill" />
        public IPaint<SkiaSharpDrawingContext>? Fill { get; set; }

        /// <inheritdoc cref="IVisualChartPoint{TDrawingContext}.HighlightableGeometry" />
        public IDrawable<SkiaSharpDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        /// <summary>
        /// Draws the geometry in the user interface.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Draw(SkiaSharpDrawingContext context)
        {
            if (HasTransform)
            {
                _ = context.Canvas.Save();

                var m = OnMeasure(context.PaintTask);
                var o = TransformOrigin;
                var p = new SKPoint(X, Y);

                var xo = m.Width * o.X;
                var yo = m.Height * o.Y;

                if (_hasGeometryTransform)
                {
                    ApplyCustomGeometryTransform(context);
                }

                if (_hasRotation)
                {
                    context.Canvas.Translate(p.X + xo, p.Y + yo);
                    context.Canvas.RotateDegrees(RotateTransform);
                    context.Canvas.Translate(-p.X - xo, -p.Y - yo);
                }

                if (_hasTranslate)
                {
                    var translate = TranslateTransform;
                    context.Canvas.Translate(translate.X, translate.Y);
                }

                if (_hasScale)
                {
                    var scale = ScaleTransform;
                    context.Canvas.Translate(p.X + xo, p.Y + yo);
                    context.Canvas.Scale(scale.X, scale.Y);
                    context.Canvas.Translate(-p.X - xo, -p.Y - yo);
                }

                if (_hasSkew)
                {
                    var skew = SkewTransform;
                    context.Canvas.Translate(p.X + xo, p.Y + yo);
                    context.Canvas.Skew(skew.X, skew.Y);
                    context.Canvas.Translate(-p.X - xo, -p.Y - yo);
                }

                if (_hasTransform)
                {
                    var transform = Transform;
                    context.Canvas.Concat(ref transform);
                }
            }

            SKPaint? originalStroke = null;
            if (context.PaintTask.IsStroke && Stroke is not null)
            {
                Stroke.IsStroke = true;
                originalStroke = context.Paint;
                Stroke.InitializeTask(context);
            }
            SKPaint? originalFill = null;
            if (!context.PaintTask.IsStroke && Fill is not null)
            {
                Fill.IsStroke = false;
                originalFill = context.Paint;
                Fill.InitializeTask(context);
            }

            if (Opacity != 1) context.PaintTask.ApplyOpacityMask(context, this);
            OnDraw(context, context.Paint);
            if (Opacity != 1) context.PaintTask.RestoreOpacityMask(context, this);

            if (context.PaintTask.IsStroke && Stroke is not null)
            {
                Stroke.Dispose();
                if (originalStroke != null) context.Paint = originalStroke;
            }
            if (!context.PaintTask.IsStroke && Fill is not null)
            {
                Fill.Dispose();
                if (originalFill != null) context.Paint = originalFill;
            }

            if (HasTransform) context.Canvas.Restore();
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
        public LvcSize Measure(IPaint<SkiaSharpDrawingContext> drawableTask)
        {
            var measure = OnMeasure((Paint)drawableTask);

            var r = RotateTransform;
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

                measure = new LvcSize(w, h);
            }

            return measure;
        }

        /// <summary>
        /// Called when the geometry is measured.
        /// </summary>
        /// <param name="paintTaks">The paint task.</param>
        /// <returns>the size of the geometry</returns>
        protected abstract LvcSize OnMeasure(Paint paintTaks);

        /// <summary>
        /// Gets the highlitable geometry.
        /// </summary>
        /// <returns></returns>
        protected virtual IGeometry<SkiaSharpDrawingContext> GetHighlitableGeometry()
        {
            return this;
        }

        /// <summary>
        /// Applies the geometry transform.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ApplyCustomGeometryTransform(SkiaSharpDrawingContext context) { }
    }
}
