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
/// <remarks>
/// Initializes a new instance of the <see cref="DropShadow"/> class.
/// </remarks>
/// <param name="dx">The dx.</param>
/// <param name="dy">The dy.</param>
/// <param name="sigmaX">The sigma x.</param>
/// <param name="sigmaY">The sigma y.</param>
/// <param name="color">The color.</param>
public class DropShadow(
    float dx,
    float dy,
    float sigmaX,
    float sigmaY,
    SKColor color)
        : ImageFilter
{
    private float Dx { get; set; } = dx;
    private float Dy { get; set; } = dy;
    private float SigmaX { get; set; } = sigmaX;
    private float SigmaY { get; set; } = sigmaY;
    private SKColor Color { get; set; } = color;

    /// <inheritdoc cref="ImageFilter.Clone"/>
    public override ImageFilter Clone() =>
        new DropShadow(Dx, Dy, SigmaX, SigmaY, Color);

    /// <inheritdoc cref="ImageFilter.CreateFilter(SkiaSharpDrawingContext)"/>
    public override void CreateFilter(SkiaSharpDrawingContext drawingContext) =>
        SKImageFilter = SKImageFilter.CreateDropShadow(Dx, Dy, SigmaX, SigmaY, Color);

    /// <inheritdoc cref="ImageFilter.Transitionate(float, ImageFilter)"/>
    public override ImageFilter? Transitionate(float progress, ImageFilter? target)
    {
        if (target is not DropShadow shadow) return this;

        var clone = (DropShadow)Clone();

        clone.Dx = Dx + (shadow.Dx - Dx) * progress;
        clone.Dy = Dy + (shadow.Dy - Dy) * progress;
        clone.SigmaX = SigmaX + (shadow.SigmaX - SigmaX) * progress;
        clone.SigmaY = SigmaY + (shadow.SigmaY - SigmaY) * progress;
        clone.Color = new SKColor(
            (byte)(Color.Red + (shadow.Color.Red - Color.Red) * progress),
            (byte)(Color.Green + (shadow.Color.Green - Color.Green) * progress),
            (byte)(Color.Blue + (shadow.Color.Blue - Color.Blue) * progress),
            (byte)(Color.Alpha + (shadow.Color.Alpha - Color.Alpha) * progress));

        return clone;
    }
}
