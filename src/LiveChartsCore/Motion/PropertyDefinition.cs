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
using System.Collections.Generic;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the motion property metadata.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="PropertyDefinition"/>.
/// </remarks>
/// <param name="propertyName">The property name.</param>
/// <param name="propertyType">The property type.</param>
/// <param name="getter">The property getter.</param>
/// <param name="setter">The property setter.</param>
/// <param name="motionGetter">The motion property getter if exists.</param>
public class PropertyDefinition(
    string propertyName,
    Type propertyType,
    PropertyDefinition.Getter getter,
    PropertyDefinition.Setter setter,
    PropertyDefinition.MotionPropertyGetter motionGetter)
{
    internal static Dictionary<Type, Func<string, object>> Parsers { get; } = new()
    {
        { typeof(float), str => float.TryParse(str, out var parsed) ? parsed : 0 },
        { typeof(double), str => double.TryParse(str, out var parsed) ? parsed : 0 },
        { typeof(int), str => int.TryParse(str, out var parsed) ? parsed : 0 }
    };

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string PropertyName { get; } = propertyName;

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public Type Type { get; } = propertyType;

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <returns>The value.</returns>
    public object? GetValue(Animatable animatable) => getter(animatable);

    /// <summary>
    /// Sets the property value.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <param name="value">The value.</param>
    public void SetValue(Animatable animatable, object? value)
    {
        if (value is string str && Type != typeof(string))
        {
            if (!Parsers.TryGetValue(Type, out var parser))
            {
                throw new InvalidOperationException(
                    $"The type {Type} does not have a parser registered. Please register a parser for this type.");
            }

            value = parser(str);
        }

        setter(animatable, value);
    }

    /// <summary>
    /// Gets the motion property if exists.
    /// </summary>
    /// <param name="animatable"></param>
    /// <returns></returns>
    public IMotionProperty? GetMotion(Animatable animatable) => motionGetter(animatable);

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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => PropertyName;
}
