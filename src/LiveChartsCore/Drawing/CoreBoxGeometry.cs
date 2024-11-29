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
/// Defines a box geometry.
/// </summary>
public abstract class CoreBoxGeometry : CoreGeometry
{
    private readonly FloatMotionProperty _wProperty;
    private readonly FloatMotionProperty _tProperty;
    private readonly FloatMotionProperty _fProperty;
    private readonly FloatMotionProperty _minProperty;
    private readonly FloatMotionProperty _medProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreBoxGeometry"/> class.
    /// </summary>
    public CoreBoxGeometry()
    {
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0f));
        _tProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Third), 0f));
        _fProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(First), 0f));
        _minProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Min), 0f));
        _medProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Median), 0f));
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
    /// Gets or sets the third quadrile.
    /// </summary>
    public float Third
    {
        get => _tProperty.GetMovement(this);
        set => _tProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the first quadrile.
    /// </summary>
    public float First
    {
        get => _fProperty.GetMovement(this);
        set => _fProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min.
    /// </summary>
    public float Min
    {
        get => _minProperty.GetMovement(this);
        set => _minProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the median.
    /// </summary>
    public float Median
    {
        get => _medProperty.GetMovement(this);
        set => _medProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="CoreGeometry.Measure(Paint)" />
    public override LvcSize Measure(Paint paintTasks) =>
        new(Width, Math.Abs(Min - Y));
}
