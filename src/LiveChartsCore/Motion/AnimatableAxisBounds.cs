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
/// Defines the animatable bounds class.
/// </summary>
public class AnimatableAxisBounds : Animatable
{
    private readonly NullableDoubleMotionProperty _maxLimitProperty;
    private readonly NullableDoubleMotionProperty _minLimitProperty;
    private readonly DoubleMotionProperty _maxDataBoundProperty;
    private readonly DoubleMotionProperty _minDataBoundProperty;
    private readonly DoubleMotionProperty _maxVisibleBoundProperty;
    private readonly DoubleMotionProperty _minVisibleBoundProperty;

    /// <summary>
    /// Intializes a new isntance of the <see cref="AnimatableAxisBounds"/> class.
    /// </summary>
    public AnimatableAxisBounds()
    {
        _maxLimitProperty = RegisterMotionProperty(new NullableDoubleMotionProperty(nameof(MaxLimit), null));
        _minLimitProperty = RegisterMotionProperty(new NullableDoubleMotionProperty(nameof(MinLimit), null));
        _maxDataBoundProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MaxDataBound), 0d));
        _minDataBoundProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MinDataBound), 0d));
        _maxVisibleBoundProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MaxVisibleBound), 0d));
        _minVisibleBoundProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MinVisibleBound), 0d));
    }

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
