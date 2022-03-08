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

using LiveChartsCore.Drawing.Segments;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines an area drawn using cubic beziers.
/// </summary>
public class CubicBezierAreaGeometry : VectorGeometry<CubicBezierSegment>
{
    /// <inheritdoc cref="VectorGeometry{TSegment}.OnDrawSegment(SkiaSharpDrawingContext, SKPath, TSegment)"/>
    protected override void OnDrawSegment(SkiaSharpDrawingContext context, SKPath path, CubicBezierSegment segment)
    {
        path.CubicTo(segment.Xi, segment.Yi, segment.Xm, segment.Ym, segment.Xj, segment.Yj);
    }

    /// <inheritdoc cref="VectorGeometry{TSegment}.OnOpen(SkiaSharpDrawingContext, SKPath, TSegment)"/>
    protected override void OnOpen(SkiaSharpDrawingContext context, SKPath path, CubicBezierSegment segment)
    {
        if (ClosingMethod == LiveChartsCore.Drawing.VectorClosingMethod.NotClosed)
        {
            path.MoveTo(segment.Xi, segment.Yi);
            return;
        }

        if (ClosingMethod == LiveChartsCore.Drawing.VectorClosingMethod.CloseToPivot)
        {
            path.MoveTo(segment.Xi, Pivot);
            path.LineTo(segment.Xi, segment.Yi);
            return;
        }
    }

    /// <inheritdoc cref="VectorGeometry{TSegment}.OnClose(SkiaSharpDrawingContext, SKPath, TSegment)"/>
    protected override void OnClose(SkiaSharpDrawingContext context, SKPath path, CubicBezierSegment segment)
    {
        if (ClosingMethod == LiveChartsCore.Drawing.VectorClosingMethod.NotClosed) return;

        if (ClosingMethod == LiveChartsCore.Drawing.VectorClosingMethod.CloseToPivot)
        {
            path.LineTo(segment.Xj, Pivot);
            path.Close();
            return;
        }
    }
}
