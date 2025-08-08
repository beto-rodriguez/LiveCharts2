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

using System.Linq;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting.ImageFilters;

/// <summary>
/// Merges multiple image filters.
/// </summary>
/// <seealso cref="ImageFilter" />
/// <remarks>
/// Initializes a new instance of the <see cref="ImageFiltersMergeOperation"/> class.
/// </remarks>
/// <param name="imageFilters">The image filters.</param>
public class ImageFiltersMergeOperation(ImageFilter[] imageFilters) : ImageFilter(s_key)
{
    internal static object s_key = new();

    private ImageFilter[] Filters { get; set; } = imageFilters;

    /// <inheritdoc cref="ImageFilter.Clone"/>
    public override ImageFilter Clone() => new ImageFiltersMergeOperation(Filters);

    /// <inheritdoc cref="ImageFilter.CreateFilter()"/>
    public override void CreateFilter()
    {
        var imageFilters = new SKImageFilter[Filters.Length];
        var i = 0;

        foreach (var item in Filters)
        {
            item.CreateFilter();
            if (item._sKImageFilter is null) throw new System.Exception("Image filter is not valid");
            imageFilters[i++] = item._sKImageFilter;
        }

        _sKImageFilter = SKImageFilter.CreateMerge(imageFilters);
    }

    /// <inheritdoc cref="ImageFilter.Transitionate(float, ImageFilter)"/>
    protected override ImageFilter Transitionate(float progress, ImageFilter target)
    {
        if (target is not ImageFiltersMergeOperation merge) return target;

        if (merge.Filters.Length != Filters.Length)
            throw new System.Exception("The image filters must have the same length");

        var clone = (ImageFiltersMergeOperation)Clone();
        var filters = new ImageFilter[Filters.Length];

        var hasNull = false;
        for (var i = 0; i < Filters.Length; i++)
        {
            var transitionated = Transitionate(Filters[i], merge.Filters[i], progress);
            filters[i] = transitionated!; // ! ignored, will be filtered out later
            if (transitionated is null) hasNull = true;
        }

        clone.Filters = hasNull
            ? [.. filters.Where(x => x is not null)]
            : filters;

        return clone;
    }

    internal override void Dispose()
    {
        foreach (var item in Filters)
            item.Dispose();
    }
}
