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

namespace LiveChartsCore.SkiaSharpView.Painting.ImageFilters;

/// <summary>
/// Creates a blur image filter.
/// </summary>
/// <seealso cref="ImageFilter" />
/// <remarks>
/// Initializes a new instance of the <see cref="Blur"/> class.
/// </remarks>
/// <param name="sigmaX">The sigma x.</param>
/// <param name="sigmaY">The sigma y.</param>
public class Blur(
    float sigmaX,
    float sigmaY)
        : ImageFilter(s_key)
{
    internal static object s_key = new();

    private float SigmaX { get; set; } = sigmaX;
    private float SigmaY { get; set; } = sigmaY;

    /// <inheritdoc cref="ImageFilter.Clone"/>
    public override ImageFilter Clone() => new Blur(SigmaX, SigmaY);

    /// <inheritdoc cref="ImageFilter.CreateFilter()"/>
    public override void CreateFilter() =>
        SKImageFilter = SKImageFilter.CreateBlur(SigmaX, SigmaY);

    /// <inheritdoc cref="ImageFilter.Transitionate(float, ImageFilter)"/>
    protected override ImageFilter Transitionate(float progress, ImageFilter target)
    {
        var blur = (Blur)target;

        var clone = (Blur)Clone();

        clone.SigmaX = SigmaX + (blur.SigmaX - SigmaX) * progress;
        clone.SigmaY = SigmaY + (blur.SigmaY - SigmaY) * progress;

        return clone;
    }
}
