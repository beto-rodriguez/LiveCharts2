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

/// <inheritdoc cref="ILineGeometry{TDrawingContext}" />
public class LineGeometry : Geometry, ILineGeometry<SkiaSharpDrawingContext>
{
    private readonly FloatMotionProperty _x1;
    private readonly FloatMotionProperty _y1;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometry"/> class.
    /// </summary>
    public LineGeometry()
    {
        _x1 = RegisterMotionProperty(new FloatMotionProperty(nameof(X1), 0f));
        _y1 = RegisterMotionProperty(new FloatMotionProperty(nameof(Y1), 0f));
    }

    /// <inheritdoc cref="ILineGeometry{TDrawingContext}.X1" />
    public float X1 { get => _x1.GetMovement(this); set => _x1.SetMovement(value, this); }

    /// <inheritdoc cref="ILineGeometry{TDrawingContext}.Y1" />
    public float Y1 { get => _y1.GetMovement(this); set => _y1.SetMovement(value, this); }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        context.Canvas.DrawLine(X, Y, X1, Y1, paint);
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> drawable)
    {
        return new LvcSize(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
    }
}
