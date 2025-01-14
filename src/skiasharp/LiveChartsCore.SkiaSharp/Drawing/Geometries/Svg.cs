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

using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Provides a set of methods to draw SVG paths.
/// </summary>
public static class Svg
{
    /// <summary>
    /// Draws the given path to the canvas.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    /// <param name="paint">The paint.</param>
    /// <param name="path">The path.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coorindate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="fitToSize">Indicates whether the path should fit the size of the geometry.</param>
    public static void Draw(
        SkiaSharpDrawingContext context,
        SKPaint paint,
        SKPath path,
        float x,
        float y,
        float width,
        float height,
        bool fitToSize = false)
    {
        _ = context.Canvas.Save();

        var canvas = context.Canvas;
        _ = path.GetTightBounds(out var bounds);

        if (fitToSize)
        {
            // fit to both axes
            canvas.Translate(x + width / 2f, y + height / 2f);
            canvas.Scale(
                width / (bounds.Width + paint.StrokeWidth),
                height / (bounds.Height + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);
        }
        else
        {
            // fit to the max dimension
            // preserve the corresponding scale in the min axis.
            var maxB = bounds.Width < bounds.Height ? bounds.Height : bounds.Width;

            canvas.Translate(x + width / 2f, y + height / 2f);
            canvas.Scale(
                width / (maxB + paint.StrokeWidth),
                height / (maxB + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);
        }

        canvas.DrawPath(path, paint);

        context.Canvas.Restore();
    }
}
