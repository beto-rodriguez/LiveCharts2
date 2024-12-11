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
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a candlestick geometry.
/// </summary>
public abstract class BaseCandlestickGeometry : DrawnGeometry
{
    private readonly FloatMotionProperty _wProperty;
    private readonly FloatMotionProperty _oProperty;
    private readonly FloatMotionProperty _cProperty;
    private readonly FloatMotionProperty _lProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCandlestickGeometry"/> class.
    /// </summary>
    public BaseCandlestickGeometry()
    {
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0f));
        _oProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Open), 0f));
        _cProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Close), 0f));
        _lProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Low), 0f));
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public float Width
    {
        get => _wProperty.GetMovement(this);
        set => _wProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the open.
    /// </summary>
    public float Open
    {
        get => _oProperty.GetMovement(this);
        set => _oProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the close.
    /// </summary>
    public float Close
    {
        get => _cProperty.GetMovement(this);
        set => _cProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the low.
    /// </summary>
    public float Low
    {
        get => _lProperty.GetMovement(this);
        set => _lProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure() =>
        new(Width, Math.Abs(Low - Y));
}
