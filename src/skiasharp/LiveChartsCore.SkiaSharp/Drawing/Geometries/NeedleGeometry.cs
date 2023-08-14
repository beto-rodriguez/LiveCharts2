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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="INeedleGeometry{TDrawingContext}"/>
public class NeedleGeometry : Geometry, INeedleGeometry<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="INeedleGeometry{TDrawingContext}.Radius"/>
    public float Radius { get; set; }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)"/>
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var w = 10;

        using var path = new SKPath();

        path.MoveTo(X, Y + Radius);
        path.LineTo(X - w, Y);
        path.LineTo(X + w, Y);
        path.Close();

        context.Canvas.DrawPath(path, paint);
        context.Canvas.DrawCircle(X, Y, w, paint);
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})"/>
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paintTasks)
    {
        return new();
    }
}
