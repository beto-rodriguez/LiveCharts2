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
using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a line geometry.
/// </summary>
public abstract class BaseLineGeometry : DrawnGeometry
{
    private readonly FloatMotionProperty _x1;
    private readonly FloatMotionProperty _y1;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseLineGeometry"/> class.
    /// </summary>
    public BaseLineGeometry()
    {
        _x1 = RegisterMotionProperty(new FloatMotionProperty(nameof(X1), 0f));
        _y1 = RegisterMotionProperty(new FloatMotionProperty(nameof(Y1), 0f));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="X1"/> and <see cref="Y1"/> properties are relative to the location
    /// (<see cref="DrawnGeometry.X"/>, <see cref="DrawnGeometry.Y"/>).
    /// </summary>
    public bool IsRelativeToLocation { get; set; }

    /// <summary>
    /// Gets or sets the x1.
    /// </summary>
    public float X1
    {
        get => IsRelativeToLocation
            ? _x1.GetMovement(this) + X
            : _x1.GetMovement(this);
        set => _x1.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the y1.
    /// </summary>
    public float Y1
    {
        get => IsRelativeToLocation
            ? _y1.GetMovement(this) + Y
            : _y1.GetMovement(this);
        set => _y1.SetMovement(value, this);
    }

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure() =>
        new(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
}
