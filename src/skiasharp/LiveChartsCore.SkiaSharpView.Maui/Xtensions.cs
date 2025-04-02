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
using System.Collections.Generic;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Maui.Controls.Xaml;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// The solid color paint extension.
/// </summary>
public class SolidColorPaintExtension : IMarkupExtension<SolidColorPaint>
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    public string? Color { get; set; } = null;

    /// <summary>
    /// Gets or sets the stroke width.
    /// </summary>
    public float StrokeWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    public SKFontStyleWeight FontWeight { get; set; } = SKFontStyleWeight.Normal;

    /// <summary>
    /// Gets or sets the font width.
    /// </summary>
    public SKFontStyleWidth FontWidth { get; set; } = SKFontStyleWidth.Normal;

    /// <summary>
    /// Gets or sets the font slant.
    /// </summary>
    public SKFontStyleSlant FontSlant { get; set; } = SKFontStyleSlant.Upright;

    /// <summary>
    /// Gets or sets a character to match the typeface to use.
    /// </summary>
    public string? TypefaceMatchesChar { get; set; } = null;

    /// <summary>
    /// ...
    /// </summary>
    public SolidColorPaint ProvideValue(IServiceProvider serviceProvider)
    {
        if (!SKColor.TryParse(Color, out var color))
            return new SolidColorPaint(SKColors.Transparent);

        var paint = new SolidColorPaint(color, StrokeWidth);

        if (FontWeight != SKFontStyleWeight.Normal || FontWidth != SKFontStyleWidth.Normal || FontSlant != SKFontStyleSlant.Upright)
            paint.SKFontStyle = new SKFontStyle(FontWeight, FontWidth, FontSlant);

        if (FontFamily is not null)
        {
            paint.SKTypeface = SKTypeface.FromFamilyName(FontFamily);
        }
        else if (TypefaceMatchesChar is not null && TypefaceMatchesChar.Length > 0)
        {
            paint.SKTypeface = SKFontManager.Default.MatchCharacter(TypefaceMatchesChar[0]);
        }

        return paint;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

/// <summary>
/// The solid color paint extension.
/// </summary>
public class FromSharedAxesExtension : IMarkupExtension<ICollection<ICartesianAxis>>
{
    /// <summary>
    /// Gets or sets the pair instance.
    /// </summary>
    public SharedAxesPair? AxesPair { get; set; }

    /// <summary>
    /// Gets or sets the element.
    /// </summary>
    public PairElement Element { get; set; }

    /// <summary>
    /// ...
    /// </summary>
    public ICollection<ICartesianAxis> ProvideValue(IServiceProvider serviceProvider)
    {
        if (AxesPair is null || AxesPair.First is null || AxesPair.Second is null)
            return [];

        return Element switch
        {
            PairElement.First => [AxesPair.First],
            PairElement.Second => [AxesPair.Second],
            _ => Array.Empty<ICartesianAxis>()
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    /// <summary>
    /// The shared axes pair elements.
    /// </summary>
    public enum PairElement
    {
        /// <summary>
        /// The first element.
        /// </summary>
        First,

        /// <summary>
        /// The second element.
        /// </summary>
        Second
    }
}
