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
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// Initializes a new instance of the <see cref="SkiaPaint"/> class.
/// </summary>
/// <param name="strokeThickness">The stroke thickness.</param>
/// <param name="strokeMiter">The stroke miter.</param>
public abstract class SkiaPaint(float strokeThickness = 1f, float strokeMiter = 0f)
    : Paint(strokeThickness, strokeMiter), IDisposable
{
    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    internal SKPaintStyle Style { get; set; }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    public SKFontStyle? SKFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the SKTypeface, if null, LiveCharts will build one based on the
    /// <see cref="Paint.FontFamily"/> and <see cref="SKFontStyle"/> properties. Default is null.
    /// </summary>
    public SKTypeface? SKTypeface { get; set; }

    /// <summary>
    /// Gets a value indication whether the paint has a custom font.
    /// </summary>
    public bool HasCustomFont =>
        LiveChartsSkiaSharp.DefaultSKTypeface is not null ||
        FontFamily is not null || SKTypeface is not null || SKFontStyle is not null;

    /// <summary>
    /// Gets or sets the stroke cap.
    /// </summary>
    /// <value>
    /// The stroke cap.
    /// </value>
    public SKStrokeCap StrokeCap { get; set; }

    /// <summary>
    /// Gets or sets the stroke join.
    /// </summary>
    /// <value>
    /// The stroke join.
    /// </value>
    public SKStrokeJoin StrokeJoin { get; set; }

    /// <summary>
    /// Gets or sets the path effect.
    /// </summary>
    /// <value>
    /// The path effect.
    /// </value>
    public PathEffect? PathEffect { get; set; }

    /// <summary>
    /// Gets or sets the image filer.
    /// </summary>
    /// <value>
    /// The image filer.
    /// </value>
    public ImageFilter? ImageFilter { get; set; }

    /// <summary>
    /// Gets a <see cref="SKTypeface"/> from the <see cref="Paint.FontFamily"/> property.
    /// </summary>
    /// <returns></returns>
    protected internal SKTypeface GetSKTypeface()
    {
        // return the defined typeface.
        if (SKTypeface is not null) return SKTypeface;

        // create one from the font family.
        if (FontFamily is not null) return SKTypeface.FromFamilyName(FontFamily, SKFontStyle ?? new SKFontStyle());

        // other wise ose the globally defined typeface.
        return LiveChartsSkiaSharp.DefaultSKTypeface ?? SKTypeface.Default;
    }
}
