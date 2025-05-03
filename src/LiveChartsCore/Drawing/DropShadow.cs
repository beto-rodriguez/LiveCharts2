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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a drop shadow.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LvcDropShadow"/> class.
/// </remarks>
public class LvcDropShadow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LvcDropShadow"/> class.
    /// </summary>
    public LvcDropShadow()
    {
        Color = LvcColor.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LvcDropShadow"/> class.
    /// </summary>
    /// <param name="x">The horizonal offset.</param>
    /// <param name="y">The vertical offset.</param>
    /// <param name="sigmaX">The blur radius in the X direction.</param>
    /// <param name="sigmaY">The blur radius in the Y direction.</param>
    /// <param name="color">The color of the shadow.</param>
    public LvcDropShadow(float x, float y, float sigmaX, float sigmaY, LvcColor color)
    {
        Dx = x;
        Dy = y;
        SigmaX = sigmaX;
        SigmaY = sigmaY;
        Color = color;
    }

    /// <summary>
    /// Gets an empty drop shadow.
    /// </summary>
    public static LvcDropShadow Empty => new(0, 0, 0, 0, new(0, 0, 0, 0));

    /// <summary>
    /// Gets or sets the horizontal offset.
    /// </summary>
    public float Dx { get; set; }

    /// <summary>
    /// Gets or sets the vertical offset.
    /// </summary>
    public float Dy { get; set; }

    /// <summary>
    /// Gets or sets the blur radius in the x direction.
    /// </summary>
    public float SigmaX { get; set; }

    /// <summary>
    /// Gets or sets the blur radius in the y direction.
    /// </summary>
    public float SigmaY { get; set; }

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    public LvcColor Color { get; set; }
}
