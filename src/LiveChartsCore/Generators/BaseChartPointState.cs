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
using System.Collections.ObjectModel;
using LiveChartsCore.VisualStates;

namespace LiveChartsCore.Generators;

/// <summary>
/// Represents the base state of a chart point, including its name and associated visual state setters.
/// </summary>
/// <remarks>This class provides a foundational structure for managing the state of a chart point. It includes a
/// name to identify the state and a collection of setters that define visual properties for the state. The <see
/// cref="Setters"/> property allows customization of visual state properties, while the <see cref="Name"/> property
/// provides a descriptive identifier for the state.</remarks>
public class BaseChartPointState
{
    /// <summary>
    /// The sate name.
    /// </summary>
    public string Name
    {
        get;
        set
        {
            field = value;
            Setters.Name = value;
        }
    } = "Default";

    /// <summary>
    /// The setters.
    /// </summary>
    public SetterCollection Setters { get; set; } = new() { Name = "Default" };

    /// <summary>
    /// Represents a collection of <see cref="BaseSet"/> objects, with additional functionality for managing  visual state
    /// properties.
    /// </summary>
    /// <remarks>This class extends <see cref="ObservableCollection{T}"/> to provide a specialized collection
    /// of  <see cref="BaseSet"/> objects. It includes methods for converting the collection into a  <see
    /// cref="VisualStatesDictionary.DrawnPropertiesDictionary"/> for use in visual state management.</remarks>
    public class SetterCollection : ObservableCollection<BaseSet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetterCollection"/> class with an empty collection.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Converts the current collection of drawn property sets into a dictionary of visual states.
        /// </summary>
        /// <remarks>This method creates a dictionary where each entry corresponds to a drawn property set
        /// in the current collection. The resulting dictionary can be used to represent visual states in a structured
        /// format.</remarks>
        /// <returns>A <see cref="VisualStatesDictionary.DrawnPropertiesDictionary"/> containing the visual states represented by
        /// the current collection. Each state is keyed by the property name and includes its corresponding value.</returns>
        public VisualStatesDictionary.DrawnPropertiesDictionary AsStatesCollection()
        {
            var setters = new Dictionary<string, DrawnPropertySetter>();
            for (var i = 0; i < Count; i++)
            {
                var set = this[i];
                setters[set.PropertyName] = new DrawnPropertySetter(set.PropertyName, set.Value);
            }

            return new(setters, false);
        }
    }
}
