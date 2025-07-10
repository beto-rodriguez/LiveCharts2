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

using System.Linq;
using LiveChartsCore.Kernel;

namespace LiveChartsCore.VisualStates;

/// <summary>
/// Defines the setter extensions.
/// </summary>
public static class SetterExtensions
{
    /// <summary>
    /// Adds a visual state to the series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="stateName">The state name.</param>
    /// <param name="setters">The setters collection.</param>
    /// <returns>The series.</returns>
    public static ISeries HasState(
        this ISeries series,
        string stateName,
        (string, object)[] setters)
    {
        var statesDictionary = setters.ToDictionary(
            x => x.Item1,
            x => new DrawnPropertySetter(x.Item1, x.Item2));

        var isInternalSet = ((ChartElement)series)._isInternalSet;

        if (series.VisualStates.TryGetValue(stateName, out var propertiesDictionary))
        {
            // only override the state if:

            if (isInternalSet == propertiesDictionary.isInternalSet)
            {
                // if both are defined by the user or both are defined by the theme

                series.VisualStates[stateName] =
                    new VisualStatesDictionary.DrawnPropertiesDictionary(statesDictionary, isInternalSet);
            }
            else
            {
                // only the user can override a theme state, a theme can never override a state defined by the user

                if (propertiesDictionary.isInternalSet && !isInternalSet)
                {
                    series.VisualStates[stateName] =
                        new VisualStatesDictionary.DrawnPropertiesDictionary(statesDictionary, isInternalSet);
                }
            }
        }
        else
        {
            // if the state is not defined, we can add it

            series.VisualStates[stateName] =
                new VisualStatesDictionary.DrawnPropertiesDictionary(statesDictionary, isInternalSet);
        }

        return series;
    }
}
