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

/// <summary>
/// Defines a geometry that is built from a svg path.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseSVGPathGeometry"/> class.
/// </remarks>
public class BaseSVGPathGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSVGPathGeometry"/> class.
    /// </summary>
    public BaseSVGPathGeometry()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSVGPathGeometry"/> class.
    /// </summary>
    /// <param name="sKPath"></param>
    public BaseSVGPathGeometry(SKPath sKPath)
    {
        Path = sKPath;
    }

    /// <summary>
    /// The SVG path.
    /// </summary>
    public SKPath? Path { get; set; }

    /// <summary>
    /// Gets or sets whether the path should fit the size of the geometry.
    /// </summary>
    public bool FitToSize { get; set; } = false;

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public virtual void Draw(SkiaSharpDrawingContext context)
    {
        if (Path is null) return;

        DrawPath(context, context.ActiveSkiaPaint, Path);
    }

    /// <summary>
    /// Draws the given path to the canvas.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="paint">The paint</param>
    protected void DrawPath(SkiaSharpDrawingContext context, SKPaint paint, SKPath path)
    {
        _ = context.Canvas.Save();

        var canvas = context.Canvas;
        _ = path.GetTightBounds(out var bounds);

        if (FitToSize)
        {
            // fit to both axes
            canvas.Translate(X + Width / 2f, Y + Height / 2f);
            canvas.Scale(
                Width / (bounds.Width + paint.StrokeWidth),
                Height / (bounds.Height + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);
        }
        else
        {
            // fit to the max dimension
            // preserve the corresponding scale in the min axis.
            var maxB = bounds.Width < bounds.Height ? bounds.Height : bounds.Width;

            canvas.Translate(X + Width / 2f, Y + Height / 2f);
            canvas.Scale(
                Width / (maxB + paint.StrokeWidth),
                Height / (maxB + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);
        }

        canvas.DrawPath(path, paint);

        context.Canvas.Restore();
    }
}
