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

/// <inheritdoc cref="IArcGeometry{TDrawingContext}" />
public class ArcGeometry : Geometry, IArcGeometry<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.CenterX"/>
    public float CenterX { get; set; }

    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.CenterY"/>
    public float CenterY { get; set; }

    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.Width"/>
    public float Width { get; set; }

    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.Height"/>
    public float Height { get; set; }

    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.StartAngle"/>
    public float StartAngle { get; set; }

    /// <inheritdoc cref="IArcGeometry{TDrawingContext}.SweepAngle"/>
    public float SweepAngle { get; set; }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)"/>
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        using var path = new SKPath();
        var cx = CenterX;
        var cy = CenterY;
        var wedge = 20;
        var r = Width * 0.5f;
        var startAngle = StartAngle;
        var sweepAngle = SweepAngle;
        const float toRadians = (float)(Math.PI / 180);

        path.MoveTo(
           (float)(cx + Math.Cos(startAngle * toRadians) * r),
           (float)(cy + Math.Sin(startAngle * toRadians) * r));
        path.ArcTo(
            new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } },
            startAngle,
            sweepAngle,
            false);

        context.Canvas.DrawPath(path, context.Paint);
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})"/>
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paintTasks)
    {
        return new();
    }
}
