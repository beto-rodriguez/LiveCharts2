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

#pragma warning disable IDE1006 // Naming Styles

using LiveChartsCore.Drawing;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the animatable bounds class.
/// </summary>
public class AnimatableAxisBounds : Animatable
{
    private readonly NullableDoubleMotionProperty _maxLimitProperty = new(null);
    private readonly NullableDoubleMotionProperty _minLimitProperty = new(null);
    private readonly DoubleMotionProperty _maxDataBoundProperty = new(0d);
    private readonly DoubleMotionProperty _minDataBoundProperty = new(0d);
    private readonly DoubleMotionProperty _maxVisibleBoundProperty = new(0d);
    private readonly DoubleMotionProperty _minVisibleBoundProperty = new(0d);

    /// <summary>
    /// Gets or sets the max limit.
    /// </summary>
    public double? MaxLimit
    {
        get => _maxLimitProperty.GetMovement(this);
        set => _maxLimitProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min limit.
    /// </summary>
    public double? MinLimit
    {
        get => _minLimitProperty.GetMovement(this);
        set => _minLimitProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the max data limit.
    /// </summary>
    public double MaxDataBound
    {
        get => _maxDataBoundProperty.GetMovement(this);
        set => _maxDataBoundProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min data limit.
    /// </summary>
    public double MinDataBound
    {
        get => _minDataBoundProperty.GetMovement(this);
        set => _minDataBoundProperty.SetMovement(value, this);
    }


    /// <summary>
    /// Gets or sets the max visible limit.
    /// </summary>
    public double MaxVisibleBound
    {
        get => _maxVisibleBoundProperty.GetMovement(this);
        set => _maxVisibleBoundProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min visible limit.
    /// </summary>
    public double MinVisibleBound
    {
        get => _minVisibleBoundProperty.GetMovement(this);
        set => _minVisibleBoundProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets a valuea indicating whewhter the bounds have a previous state.
    /// </summary>
    public bool HasPreviousState { get; internal set; }
}
