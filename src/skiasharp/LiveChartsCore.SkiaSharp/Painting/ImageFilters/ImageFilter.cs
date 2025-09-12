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

using System.Collections.Generic;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting.ImageFilters;

/// <summary>
/// A wrapper object for skia sharp image filters.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ImageFilter"/> class.
/// </remarks>
/// <param name="key">The fiter type key.</param>
public abstract class ImageFilter(object key)
{
    private readonly object _key = key;
    private static readonly Dictionary<object, ImageFilter> s_defaultFilters = new()
    {
        { DropShadow.s_key, new DropShadow(0,0,0,0, SKColors.Transparent) },
        { Blur.s_key, new Blur(0,0) }
    };
    internal SKImageFilter? _sKImageFilter;

    /// <summary>
    /// Creates the image filter.
    /// </summary>
    public abstract void CreateFilter();

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    public abstract ImageFilter Clone();

    /// <summary>
    /// Adds a default filter.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="filter">The filter.</param>
    public static void AddDefaultFilter(byte key, ImageFilter filter) =>
        s_defaultFilters[key] = filter;

    /// <summary>
    /// Transitions the image filter to a new one.
    /// </summary>
    /// <param name="progress">The transition progress.</param>
    /// <param name="target">The end target.</param>
    /// <returns>The image filter.</returns>
    protected abstract ImageFilter Transitionate(float progress, ImageFilter target);

    internal static ImageFilter? Transitionate(ImageFilter? from, ImageFilter? to, float progress)
    {
        if (from is null && to is null) return null;

        var key = (from ?? to)!._key;

        // use the default filter when the transition is to a null reference
        // for example in the case of a shadow, the default filter is a transparent shadow
        from ??= s_defaultFilters[key];
        to ??= s_defaultFilters[key];

        return from.Transitionate(progress, to);
    }

    internal virtual void Dispose()
    {
        _sKImageFilter?.Dispose();
        _sKImageFilter = null;
    }
}
