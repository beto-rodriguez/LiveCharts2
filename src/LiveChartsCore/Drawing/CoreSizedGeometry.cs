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
/// Defines a geometry with width and height dimensions.
/// </summary>
public abstract class CoreSizedGeometry : CoreGeometry
{
    private readonly FloatMotionProperty _widthProperty;
    private readonly FloatMotionProperty _heightProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreSizedGeometry"/> class.
    /// </summary>
    protected CoreSizedGeometry()
    {
        _widthProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0));
        _heightProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Height), 0));
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public float Width
    {
        get => _widthProperty.GetMovement(this);
        set => _widthProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public float Height
    {
        get => _heightProperty.GetMovement(this);
        set => _heightProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="CoreGeometry.Measure()" />
    public override LvcSize Measure() =>
        new(Width, Height);
}
