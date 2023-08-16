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

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a box geometry.
/// </summary>
public class BoxGeometry : Geometry, IBoxGeometry<SkiaSharpDrawingContext>
{
    private readonly FloatMotionProperty _wProperty;
    private readonly FloatMotionProperty _tProperty;
    private readonly FloatMotionProperty _fProperty;
    private readonly FloatMotionProperty _minProperty;
    private readonly FloatMotionProperty _medProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoxGeometry"/> class.
    /// </summary>
    public BoxGeometry()
    {
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0f));
        _tProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Third), 0f));
        _fProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(First), 0f));
        _minProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Min), 0f));
        _medProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Median), 0f));
    }

    /// <inheritdoc cref="IBoxGeometry{TDrawingContext}.Width" />
    public float Width { get => _wProperty.GetMovement(this); set => _wProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IBoxGeometry{TDrawingContext}.Third" />
    public float Third { get => _tProperty.GetMovement(this); set => _tProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IBoxGeometry{TDrawingContext}.First" />
    public float First { get => _fProperty.GetMovement(this); set => _fProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IBoxGeometry{TDrawingContext}.Min" />
    public float Min { get => _minProperty.GetMovement(this); set => _minProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IBoxGeometry{TDrawingContext}.Median" />
    public float Median { get => _medProperty.GetMovement(this); set => _medProperty.SetMovement(value, this); }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var w = Width;
        var cx = X + w * 0.5f;
        var h = Y;
        var o = Third;
        var c = First;
        var l = Min;
        var m = Median;
        var x = X;

        float yi, yj;

        if (o > c)
        {
            yi = c;
            yj = o;
        }
        else
        {
            yi = o;
            yj = c;
        }

        if (paint.IsStroke)
        {
            context.Canvas.DrawLine(cx, h, cx, yi, paint);
            context.Canvas.DrawLine(x, m, x + w, m, paint);
            context.Canvas.DrawLine(cx, yj, cx, l, paint);
        }
        else
        {
            context.Canvas.DrawRect(x, yi, w, Math.Abs(o - c), paint);
        }

        context.Canvas.DrawRect(x, yi, w, Math.Abs(o - c), paint);
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paintTasks)
    {
        return new LvcSize(Width, Math.Abs(Min - Y));
    }
}
