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
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting.ImageFilters;

/// <summary>
/// A wrapper object for skia sharp image filters.
/// </summary>
/// <seealso cref="IDisposable" />
public abstract class ImageFilter : IDisposable
{
    /// <summary>
    /// Gets or sets the sk image filter.
    /// </summary>
    /// <value>
    /// The sk image filter.
    /// </value>
    public SKImageFilter? SKImageFilter { get; set; }

    /// <summary>
    /// Creates the image filter.
    /// </summary>
    /// <param name="drawingContext">The drawing context.</param>
    public abstract void CreateFilter(SkiaSharpDrawingContext drawingContext);

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    public abstract ImageFilter Clone();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
        if (SKImageFilter is null) return;
        SKImageFilter.Dispose();
        SKImageFilter = null;
    }
}
