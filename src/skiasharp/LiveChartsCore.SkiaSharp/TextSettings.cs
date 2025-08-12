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
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the text settings for SkiaSharp.
/// </summary>
public class TextSettings
{
    /// <summary>
    /// Defines a global typeface to use for text paints, this typeface will be used
    /// when the <see cref="SkiaPaint.SKTypeface"/> property is null (default is null).
    /// </summary>
    public SKTypeface? Typeface { get; set; }

    /// <summary>
    /// Defines a global action that configures a font before drawing with it, this action will be used
    /// to configure the font to draw with, by default edging is set to <see cref="SKFontEdging.SubpixelAntialias"/>
    /// and hinting is set to <see cref="SKFontHinting.Normal"/>.
    /// </summary>
    public Func<SKTypeface, float, SKFont> FontBuilder { get; set; }
        = (typeFace, size) =>
            // could this be improved?
            // like creating font settings based on the screen dpi, etc.
            new SKFont(typeFace, size)
            {
                Edging = SKFontEdging.SubpixelAntialias,
                Hinting = SKFontHinting.Normal
            };

    /// <summary>
    /// Determines whether the text is rendered in right-to-left mode.
    /// </summary>
    public bool IsRTL => LiveCharts.DefaultSettings.IsRightToLeft;
}
