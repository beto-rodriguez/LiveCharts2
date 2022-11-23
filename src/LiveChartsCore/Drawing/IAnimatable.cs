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

using System.Collections.Generic;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines an animated object, the properties of this object move according to the easing and speed when a change occurs.
/// </summary>
public interface IAnimatable
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is completed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is completed; otherwise, <c>false</c>.
    /// </value>
    bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the current time.
    /// </summary>
    /// <value>
    /// The current time.
    /// </value>
    long CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the element is removed when all the transitions complete.
    /// </summary>
    /// <value>
    ///   <c>true</c> if remove on completed; otherwise, <c>false</c>.
    /// </value>
    bool RemoveOnCompleted { get; set; }

    /// <summary>
    /// Gets the motion properties.
    /// </summary>
    Dictionary<string, IMotionProperty> MotionProperties { get; }

    /// <summary>
    /// Sets a property transition for the specified property or properties.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="propertyName">Name of the property, use null to set all the animatable properties.</param>
    void SetTransition(Animation? animation, params string[]? propertyName);

    /// <summary>
    /// Removes a property or properties transitions.
    /// </summary>
    /// <param name="propertyName">Name of the property, null to remove them all.</param>
    void RemoveTransition(params string[]? propertyName);

    /// <summary>
    /// Completes the property or properties transitions.
    /// </summary>
    /// <param name="propertyName">Name of the property, null to select them all.</param>
    void CompleteTransition(params string[]? propertyName);
}
