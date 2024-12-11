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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a box geometry.
/// </summary>
public class BoxGeometry : BaseBoxGeometry, ISkiaGeometry
{
    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public void Draw(SkiaSharpDrawingContext ctx) =>
        OnDraw(ctx, ctx.ActiveSkiaPaint);

    /// <inheritdoc cref="ISkiaGeometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public virtual void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
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

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure() =>
        new(Width, Math.Abs(Min - Y));
}
