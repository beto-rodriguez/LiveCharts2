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

using LiveChartsCore.Drawing;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the color motion property class.
/// </summary>
public class ColorMotionProperty : MotionProperty<LvcColor>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public ColorMotionProperty(string propertyName)
        : base(propertyName)
    {
        fromValue = LvcColor.FromArgb(0, 0, 0, 0);
        toValue = LvcColor.FromArgb(0, 0, 0, 0);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public ColorMotionProperty(string propertyName, LvcColor value)
        : base(propertyName)
    {
        fromValue = value;
        toValue = value;
    }

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override LvcColor OnGetMovement(float progress)
    {
        return toValue == LvcColor.Empty
            ? LvcColor.Empty
            : LvcColor.FromArgb(
                (byte)(fromValue.A + progress * (toValue.A - fromValue.A)),
                (byte)(fromValue.R + progress * (toValue.R - fromValue.R)),
                (byte)(fromValue.G + progress * (toValue.G - fromValue.G)),
                (byte)(fromValue.B + progress * (toValue.B - fromValue.B)));
    }
}
