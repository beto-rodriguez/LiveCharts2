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
    private readonly DoubleMotionProperty _maxDataLimitProperty;
    private readonly DoubleMotionProperty _minDataLimitProperty;
    private readonly DoubleMotionProperty _maxVisibleLimitProperty;
    private readonly DoubleMotionProperty _minVisibleLimitProperty;

    /// <summary>
    /// Intializes a new isntance of the <see cref="AnimatableAxisBounds"/> class.
    /// </summary>
    public AnimatableAxisBounds()
    {
        _maxLimitProperty = RegisterMotionProperty(new NullableDoubleMotionProperty(nameof(MaxLimit), null));
        _minLimitProperty = RegisterMotionProperty(new NullableDoubleMotionProperty(nameof(MinLimit), null));
        _maxDataLimitProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MaxDataLimit), 0d));
        _minDataLimitProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MinDataLimit), 0d));
        _maxVisibleLimitProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MaxVisibleLimit), 0d));
        _minVisibleLimitProperty = RegisterMotionProperty(new DoubleMotionProperty(nameof(MinVisibleLimit), 0d));
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
    public double MaxDataLimit
    {
        get => _maxDataLimitProperty.GetMovement(this);
        set => _maxDataLimitProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min data limit.
    /// </summary>
    public double MinDataLimit
    {
        get => _minDataLimitProperty.GetMovement(this);
        set => _minDataLimitProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the max  visible limit.
    /// </summary>
    public double MaxVisibleLimit
    {
        get => _maxVisibleLimitProperty.GetMovement(this);
        set => _maxVisibleLimitProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the min visible limit.
    /// </summary>
    public double MinVisibleLimit
    {
        get => _minVisibleLimitProperty.GetMovement(this);
        set => _minVisibleLimitProperty.SetMovement(value, this);
    }
}
