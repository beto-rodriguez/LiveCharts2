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
using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <inheritdoc cref="Animatable" />
public abstract class Animatable
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is valid, the instance is valid when all the
    /// motion properties in the object finished their animations.
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets or sets the current time, this property is used by the motion engine to calculate the progress of the animations.
    /// </summary>
    public long CurrentTime { get; set; } = long.MinValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance should be removed from the canvas when all the animations are completed.
    /// </summary>
    public bool RemoveOnCompleted { get; set; }

    /// <summary>
    /// Gets the motion properties.
    /// </summary>
    public Dictionary<string, IMotionProperty> MotionProperties { get; } = [];

    /// <summary>
    /// Sets the transition for the specified properties.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="propertyName">The property name, null to select all properties.</param>
    public void SetTransition(Animation? animation, params string[]? propertyName)
    {
        var a = animation?.Duration == 0 ? null : animation;
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var name in propertyName)
            MotionProperties[name].Animation = a;
    }

    /// <summary>
    /// Removes the transition for the specified properties.
    /// </summary>
    /// <param name="propertyName">The properties to remove, null to select all properties.</param>
    public void RemoveTransition(params string[]? propertyName)
    {
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var name in propertyName)
        {
            MotionProperties[name].Animation = null;
        }
    }

    /// <summary>
    /// Completes the transition for the specified properties.
    /// </summary>
    /// <param name="propertyName">The property name, null to select all properties.</param>
    public virtual void CompleteTransition(params string[]? propertyName)
    {
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var property in propertyName)
        {
            if (!MotionProperties.TryGetValue(property, out var transitionProperty))
                throw new Exception(
                    $"The property {property} is not a transition property of this instance.");

            if (transitionProperty.Animation is null) continue;
            transitionProperty.IsCompleted = true;
        }
    }

    /// <summary>
    /// Registers a motion property.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="motionProperty">The transition.</param>
    /// <returns></returns>
    protected T RegisterMotionProperty<T>(T motionProperty)
        where T : IMotionProperty
    {
        MotionProperties[motionProperty.PropertyName] = motionProperty;
        return motionProperty;
    }
}
