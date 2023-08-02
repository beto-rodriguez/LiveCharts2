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

// Ignore Spelling: svg

using System;
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a geometry that is buil from a svg path.
/// </summary>
/// <seealso cref="SizedGeometry" />
public class SVGPathGeometry : SizedGeometry, ISvgPath<SkiaSharpDrawingContext>
{
    private readonly Func<SKPath>? _pathSource;
    private string? _svgPath;

    /// <summary>
    /// Inieializes a new instance of the <see cref="SVGPathGeometry"/> class.
    /// </summary>
    public SVGPathGeometry()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SVGPathGeometry"/> class.
    /// </summary>
    /// <param name="svgPath">The SVG path.</param>
    public SVGPathGeometry(SKPath svgPath)
    {
        Path = svgPath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SVGPathGeometry"/> class,
    /// when <see cref="Path"/> is null, The path will be obtained from the <paramref name="pathSource"/> function.
    /// </summary>
    /// <param name="pathSource">The path source.</param>
    public SVGPathGeometry(Func<SKPath> pathSource)
    {
        _pathSource = pathSource;
    }

    /// <summary>
    /// Svg paths are cached in this dictionary to prevent parsing multiple times the same path,
    /// when you use the <see cref="SVGPathGeometry"/> class, keep in mind that the parsed paths are living in
    /// memory, this has no secondary effects in most of the cases, but if you are parsing a lot of paths
    /// (maybe over 500) then you must consider cleaning the cache when you no longer need a path.
    /// </summary>
    public static readonly Dictionary<string, SKPath> Cache = new();

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    public SKPath? Path { get; set; }

    /// <summary>
    /// Gets or sets whether the path should be fitted to the size of the geometry.
    /// </summary>
    public bool FitToSize { get; set; } = false;

    /// <inheritdoc cref="ISvgPath{TDrawingContext}.SVGPath"/>
    public string? SVGPath
    {
        get => _svgPath;
        set
        {
            if (value == _svgPath) return;

            _svgPath = value;
            OnPathChanged(value);
        }
    }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var path = Path ?? _pathSource?.Invoke();
        if (path is null) return;

        DrawPath(context, paint, path);
    }

    private void OnPathChanged(string? path)
    {
        if (path is null)
        {
            Path = null;
            return;
        }

        if (!Cache.TryGetValue(path, out var skPath))
        {
            skPath = SKPath.ParseSvgPathData(path);
            Cache[path] = skPath;
        }

        Path = skPath;
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
            // fit to both axis
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
