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
/// Defines a motions property.
/// </summary>
public interface IMotionProperty
{
#if DEBUG
    /// <summary>
    /// Gets or sets the instance that will be used to log the getter of the <see cref="IMotionProperty"/>.
    /// </summary>
    Animatable? LogGet { get; set; }

    /// <summary>
    /// Gets or sets the instance that will be used to log the setter of the <see cref="IMotionProperty"/>.
    /// </summary>
    Animatable? LogSet { get; set; }
#endif

    /// <summary>
    /// Finishes the transition.
    /// </summary>
    void Finish();

    /// <summary>
    /// Gets or sets the animation.
    /// </summary>
    /// <value>
    /// The animation.
    /// </value>
    Animation? Animation { get; set; }

    /// <summary>
    /// Copies into this instance the source property.
    /// </summary>
    /// <param name="source">The source.</param>
    void CopyFrom(IMotionProperty source);

    /// <summary>
    /// Saves the property target value.
    /// </summary>
    void Save();

    /// <summary>
    /// Restores the property target value.
    /// </summary>
    void Restore(Animatable animatable);
}
