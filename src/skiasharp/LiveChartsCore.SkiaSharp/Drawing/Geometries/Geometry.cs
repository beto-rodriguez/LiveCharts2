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
using LiveChartsCore.SkiaSharpView.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="CoreGeometry" />
public abstract class Geometry : CoreGeometry, ISkiaGeometry
{
    private readonly SKMatrixMotionProperty _skMatrixProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Geometry"/> class.
    /// </summary>
    protected Geometry(bool hasGeometryTransform = false)
    {
        //_skMatrixProperty = RegisterMotionProperty(
        //    new SKMatrixMotionProperty(nameof(Transform), SKMatrix.Identity));
    }

    /// <summary>
    /// Gets or sets the matrix transform.
    /// </summary>
    /// <value>
    /// The transform.
    /// </value>
    public SKMatrix SKMatrix
    {
        get => _skMatrixProperty.GetMovement(this);
        set
        {
            _skMatrixProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)"/>
    public virtual void Draw(SkiaSharpDrawingContext context)
    {
        if (HasTransform)
        {
            _ = context.Canvas.Save();

            var m = OnMeasure(context.PaintTask);
            var o = TransformOrigin;
            var p = new SKPoint(X, Y);

            var xo = m.Width * o.X;
            var yo = m.Height * o.Y;

            if (HasRotation)
            {
                context.Canvas.Translate(p.X + xo, p.Y + yo);
                context.Canvas.RotateDegrees(RotateTransform);
                context.Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            if (HasTranslate)
            {
                var translate = TranslateTransform;
                context.Canvas.Translate(translate.X, translate.Y);
            }

            if (HasScale)
            {
                var scale = ScaleTransform;
                context.Canvas.Translate(p.X + xo, p.Y + yo);
                context.Canvas.Scale(scale.X, scale.Y);
                context.Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            if (HasSkew)
            {
                var skew = SkewTransform;
                context.Canvas.Translate(p.X + xo, p.Y + yo);
                context.Canvas.Skew(skew.X, skew.Y);
                context.Canvas.Translate(-p.X - xo, -p.Y - yo);
            }

            //if (_hasTransform)
            //{
            //    var transform = Transform;
            //    context.Canvas.Concat(ref transform);
            //}
        }

        var hasGeometryOpacity = Opacity < 1;

        if (hasGeometryOpacity) context.PaintTask.ApplyOpacityMask(context, this);
        OnDraw(context, context.Paint);
        if (hasGeometryOpacity) context.PaintTask.RestoreOpacityMask(context, this);

        // Fill and Stroke paints are defined by each geometry.
        // normally, LiveCharts initializes a paint, then draws all the geometries in the series
        // and diposes the paint.
        // but there are cases where each geometry needs to have its own paint.

        if (Fill is not null)
        {
            var originalPaint = context.Paint;
            var originalTask = context.PaintTask;

            Fill.IsStroke = false;
            Fill.InitializeTask(context);

            if (hasGeometryOpacity) Fill.ApplyOpacityMask(context, this);
            OnDraw(context, context.Paint);
            if (hasGeometryOpacity) Fill.RestoreOpacityMask(context, this);

            Fill.Dispose();

            context.Paint = originalPaint;
            context.PaintTask = originalTask;
        }

        if (Stroke is not null)
        {
            var originalPaint = context.Paint;
            var originalTask = context.PaintTask;

            Stroke.IsStroke = true;
            Stroke.InitializeTask(context);

            if (hasGeometryOpacity) Stroke.ApplyOpacityMask(context, this);
            OnDraw(context, context.Paint);
            if (hasGeometryOpacity) Stroke.RestoreOpacityMask(context, this);

            Stroke.Dispose();

            context.Paint = originalPaint;
            context.PaintTask = originalTask;
        }

        if (HasTransform) context.Canvas.Restore();
    }

    /// <summary>
    /// Called when the geometry is drawn.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="paint">The paint.</param>
    public abstract void OnDraw(SkiaSharpDrawingContext context, SKPaint paint);
}
