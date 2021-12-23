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
/// Creates a drop shadow image filter.
/// </summary>
/// <seealso cref="ImageFilter" />
public class DropShadow : ImageFilter
{
    private readonly float _dx;
    private readonly float _dy;
    private readonly float _sigmaX;
    private readonly float _sigmaY;
    private readonly SKColor _color;
    private readonly SKImageFilter? _filter = null;
    private readonly SKImageFilter.CropRect? _cropRect = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DropShadow"/> class.
    /// </summary>
    /// <param name="dx">The dx.</param>
    /// <param name="dy">The dy.</param>
    /// <param name="sigmaX">The sigma x.</param>
    /// <param name="sigmaY">The sigma y.</param>
    /// <param name="color">The color.</param>
    /// <param name="input">The input.</param>
    /// <param name="cropRect">The crop rect.</param>
    public DropShadow(float dx, float dy, float sigmaX, float sigmaY, SKColor color, SKImageFilter? input = null, SKImageFilter.CropRect? cropRect = null)
    {
        _dx = dx;
        _dy = dy;
        _sigmaX = sigmaX;
        _sigmaY = sigmaY;
        _color = color;
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
        return new DropShadow(_dx, _dy, _sigmaX, _sigmaY, _color, _filter, _cropRect);
    }

    /// <summary>
    /// Creates the image filter.
    /// </summary>
    /// <param name="drawingContext">The drawing context.</param>
    public override void CreateFilter(SkiaSharpDrawingContext drawingContext)
    {
        SKImageFilter = SKImageFilter.CreateDropShadow(_dx, _dy, _sigmaX, _sigmaY, _color, _filter, _cropRect);
    }
}
