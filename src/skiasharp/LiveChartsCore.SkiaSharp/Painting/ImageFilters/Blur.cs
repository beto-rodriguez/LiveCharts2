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

using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting.ImageFilters;

/// <summary>
/// Creates a blur image filter.
/// </summary>
/// <seealso cref="ImageFilter" />
public class Blur : ImageFilter
{
    private readonly float _sigmaX;
    private readonly float _sigmaY;
    private readonly SKImageFilter? _filter = null;
    private readonly SKImageFilter.CropRect? _cropRect = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Blur"/> class.
    /// </summary>
    /// <param name="sigmaX">The sigma x.</param>
    /// <param name="sigmaY">The sigma y.</param>
    /// <param name="input">The input.</param>
    /// <param name="cropRect">The crop rect.</param>
    public Blur(float sigmaX, float sigmaY, SKImageFilter? input = null, SKImageFilter.CropRect? cropRect = null)
    {
        _sigmaX = sigmaX;
        _sigmaY = sigmaY;
        _filter = input;
        _cropRect = cropRect;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override ImageFilter Clone()
    {
        return new Blur(_sigmaX, _sigmaY, _filter, _cropRect);
    }

    /// <summary>
    /// Creates the filter.
    /// </summary>
    /// <param name="drawingContext">The drawing context.</param>
    /// <returns></returns>
    public override void CreateFilter(SkiaSharpDrawingContext drawingContext)
    {
        SKImageFilter = SKImageFilter.CreateBlur(_sigmaX, _sigmaY, _filter, _cropRect);
    }
}
