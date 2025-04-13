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
/// Defines the motion property metadata.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="PropertyDefinition"/>.
/// </remarks>
/// <param name="getter">The property getter.</param>
/// <param name="setter">The property setter.</param>
/// <param name="motionGetter">The motion property getter if exists.</param>
public class PropertyDefinition(
    PropertyDefinition.Getter getter,
    PropertyDefinition.Setter setter,
    PropertyDefinition.MotionPropertyGetter? motionGetter)
{
    /// <summary>
    /// Gets the getter function.
    /// </summary>
    public Getter GetValue { get; } = getter;

    /// <summary>
    /// Gets the setter function.
    /// </summary>
    public Setter SetValue { get; } = setter;

    /// <summary>
    /// Gets the motion property.
    /// </summary>
    public MotionPropertyGetter? GetMotion { get; } = motionGetter;

    /// <summary>
    /// The property definition setter delegate.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <param name="value">The value.</param>
    public delegate void Setter(Animatable animatable, object? value);

    /// <summary>
    /// The property definition getter delegate.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <returns>The value.</returns>
    public delegate object? Getter(Animatable animatable);

    /// <summary>
    /// The property definition motion property getter delegate.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <returns>The motion property.</returns>
    public delegate IMotionProperty? MotionPropertyGetter(Animatable animatable);
}
