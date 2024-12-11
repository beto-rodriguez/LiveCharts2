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

using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a needle geometry.
/// </summary>
public abstract class BaseNeedleGeometry : DrawnGeometry
{
    private readonly FloatMotionProperty _rProperty;
    private readonly FloatMotionProperty _wProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNeedleGeometry"/> class.
    /// </summary>
    public BaseNeedleGeometry()
    {
        _rProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Radius), 0f));
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 20f));
    }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    public float Radius
    {
        get => _rProperty.GetMovement(this);
        set => _rProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public float Width
    {
        get => _wProperty.GetMovement(this);
        set => _wProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.Measure()"/>
    public override LvcSize Measure() =>
        new(Width, Radius);
}

