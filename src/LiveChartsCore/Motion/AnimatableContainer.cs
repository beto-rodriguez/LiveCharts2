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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the animatable container class.
/// </summary>
public partial class AnimatableContainer : Animatable
{
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    [MotionProperty]
    public partial LvcPoint Location { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [MotionProperty]
    public partial LvcSize Size { get; set; }

    /// <summary>
    /// Gets a valuea indicating whewhter the container have a previous state.
    /// </summary>
    public bool HasPreviousState { get; internal set; }
}
