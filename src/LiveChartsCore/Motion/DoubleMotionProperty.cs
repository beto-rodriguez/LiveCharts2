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

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the double motion property class.
/// </summary>
public class DoubleMotionProperty : MotionProperty<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public DoubleMotionProperty(string propertyName)
        : base(propertyName)
    {
        fromValue = 0;
        toValue = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public DoubleMotionProperty(string propertyName, double value)
        : base(propertyName)
    {
        fromValue = value;
        toValue = value;
    }

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override double OnGetMovement(float progress)
    {
        return fromValue + progress * (toValue - fromValue);
    }
}
