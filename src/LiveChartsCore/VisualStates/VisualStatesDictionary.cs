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
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using static LiveChartsCore.VisualStates.VisualStatesDictionary;

namespace LiveChartsCore.VisualStates;

/// <summary>
/// Defines a collection of visual states.
/// </summary>
public class VisualStatesDictionary : Dictionary<string, DrawnPropertiesDictionary>
{
    internal class StatesTracker
    {
        public HashSet<string> ActiveStates = [];
        public List<string> ActiveStatesList = [];
        public Dictionary<PropertyDefinition, object?> OriginalValues = [];
    }

    /// <summary>
    /// Sets the visual state of the instance.
    /// </summary>
    /// <param name="stateName">The state name.</param>
    /// <param name="animatable">The animatable instance.</param>
    public void SetState(string stateName, Animatable animatable)
    {
        animatable._statesTracker ??= new();
        var state = this[stateName];

        foreach (var setter in state.Values)
        {
            var definition = animatable.GetPropertyDefinition(setter.PropertyName);
            var property = definition?.GetMotion(animatable);
            if (definition is null || property is null) continue;

            // if the value is already saved, it means that the original value
            // was already saved by this or another state
            if (!animatable._statesTracker.OriginalValues.ContainsKey(definition))
            {
                // save the original value
                animatable._statesTracker.OriginalValues[definition] = property.ToValue;
            }

            // set the state value
            definition.SetValue(animatable, setter.Value);
        }

        if (!animatable._statesTracker.ActiveStates.Add(stateName)) return;
        animatable._statesTracker.ActiveStatesList.Add(stateName);
    }

    /// <summary>
    /// Clears the visual state of the instance.
    /// </summary>
    /// <param name="stateName">The state name.</param>
    /// <param name="animatable">The animatable instance.</param>
    public void ClearState(string stateName, Animatable animatable)
    {
        if (animatable._statesTracker is null) return;
        var state = this[stateName];

        foreach (var setter in state.Values)
        {
            var definition = animatable.GetPropertyDefinition(setter.PropertyName);
            var property = definition?.GetMotion(animatable);
            if (definition is null || property is null) continue;

            var foundValue = false;
            object? value = null;

            // get the last setter for the property;
            for (var i = animatable._statesTracker.ActiveStatesList.Count - 1; i >= 0; i--)
            {
                var name = animatable._statesTracker.ActiveStatesList[i];
                if (stateName == name) continue;

                if (this[name].TryGetValue(setter.PropertyName, out var lastSetter))
                {
                    value = lastSetter.Value;
                    foundValue = true;
                    break;
                }
            }

            // if no value was found, then we need to restore the original value
            if (!foundValue && animatable._statesTracker.OriginalValues.TryGetValue(definition, out var originalValue))
            {
                value = originalValue;
                _ = animatable._statesTracker.OriginalValues.Remove(definition);
            }

            // set the original value
            definition.SetValue(animatable, value);
        }

        _ = animatable._statesTracker.ActiveStates.Remove(stateName);
        _ = animatable._statesTracker.ActiveStatesList.Remove(stateName);
    }

    /// <summary>
    /// Clears all the states of the instance.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    public void ClearStates(Animatable animatable)
    {
        if (animatable._statesTracker is null) return;

        foreach (var stateName in animatable._statesTracker.ActiveStatesList)
        {
            var state = this[stateName];

            foreach (var setter in state.Values)
            {
                var definition = animatable.GetPropertyDefinition(setter.PropertyName);
                var property = definition?.GetMotion(animatable);
                if (definition is null || property is null) continue;

                // set the original value
                if (animatable._statesTracker.OriginalValues.TryGetValue(definition, out var originalValue))
                {
                    definition.SetValue(animatable, originalValue);
                    _ = animatable._statesTracker.OriginalValues.Remove(definition);
                }
            }
        }

        animatable._statesTracker.ActiveStates.Clear();
        animatable._statesTracker.ActiveStatesList.Clear();
        animatable._statesTracker.OriginalValues.Clear();
    }

    /// <summary>
    /// Defines a collection of drawn properties.
    /// </summary>
    /// <param name="baseDictionary">The base dictionary.</param>
    /// <param name="isInternalSet">Indicates whether the object was created by a theme.</param>
    public class DrawnPropertiesDictionary(
        Dictionary<string, DrawnPropertySetter> baseDictionary,
        bool isInternalSet)
            : Dictionary<string, DrawnPropertySetter>(baseDictionary)
    {
        internal bool isInternalSet { get; } = isInternalSet;
    }
}
