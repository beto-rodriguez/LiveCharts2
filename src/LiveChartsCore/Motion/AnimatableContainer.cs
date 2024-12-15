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
/// Defines the animatable container class.
/// </summary>
public class AnimatableContainer : Animatable
{
    private readonly PointMotionProperty _locationProperty;
    private readonly SizeMotionProperty _sizeMotionProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatableContainer"/> class.
    /// </summary>
    public AnimatableContainer()
    {
        _locationProperty = RegisterMotionProperty(new PointMotionProperty(nameof(Location), new LvcPoint()));
        _sizeMotionProperty = RegisterMotionProperty(new SizeMotionProperty(nameof(Size), new LvcSize()));
    }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public LvcPoint Location
    {
        get => _locationProperty.GetMovement(this);
        set => _locationProperty.SetMovement(value, this);
    }


    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public LvcSize Size
    {
        get => _sizeMotionProperty.GetMovement(this);
        set => _sizeMotionProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets a valuea indicating whewhter the container have a previous state.
    /// </summary>
    public bool HasPreviousState { get; internal set; }
}
