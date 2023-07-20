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
/// Defines a candlestick geometry.
/// </summary>
public class CandlestickGeometry : Geometry, IFinancialGeometry<SkiaSharpDrawingContext>
{
    private readonly FloatMotionProperty _wProperty;
    private readonly FloatMotionProperty _oProperty;
    private readonly FloatMotionProperty _cProperty;
    private readonly FloatMotionProperty _lProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandlestickGeometry"/> class.
    /// </summary>
    public CandlestickGeometry()
    {
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0f));
        _oProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Open), 0f));
        _cProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Close), 0f));
        _lProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Low), 0f));
    }

    /// <inheritdoc cref="IFinancialGeometry{TDrawingContext}.Width" />
    public float Width { get => _wProperty.GetMovement(this); set => _wProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IFinancialGeometry{TDrawingContext}.Open" />
    public float Open { get => _oProperty.GetMovement(this); set => _oProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IFinancialGeometry{TDrawingContext}.Close" />
    public float Close { get => _cProperty.GetMovement(this); set => _cProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IFinancialGeometry{TDrawingContext}.Low" />
    public float Low { get => _lProperty.GetMovement(this); set => _lProperty.SetMovement(value, this); }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var w = Width;
        var cx = X + w * 0.5f;
        var h = Y;
        var o = Open;
        var c = Close;
        var l = Low;

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

        context.Canvas.DrawLine(cx, h, cx, yi, paint);
        context.Canvas.DrawRect(X, yi, w, Math.Abs(o - c), paint);
        context.Canvas.DrawLine(cx, yj, cx, l, paint);
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paintTasks)
    {
        return new LvcSize(Width, Math.Abs(Low - Y));
    }
}
