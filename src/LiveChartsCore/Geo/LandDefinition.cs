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

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines the data of a lane in a map.
/// </summary>
public class LandDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LandDefinition"/> class.
    /// </summary>
    /// <param name="shortName">The short name.</param>
    /// <param name="name">The name.</param>
    /// <param name="setOf">The set of.</param>
    public LandDefinition(string shortName, string name, string setOf)
    {
        Name = name;
        ShortName = shortName;
        SetOf = setOf;
    }

    /// <summary>
    /// Gets the short name.
    /// </summary>
    public string ShortName { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the set of reference.
    /// </summary>
    public string SetOf { get; set; }

    /// <summary>
    /// Gets the HSize.
    /// </summary>
    public double HSize { get; internal set; }

    /// <summary>
    /// Gets the HCenter.
    /// </summary>
    public double HCenter { get; internal set; }

    /// <summary>
    /// Gets or sets the maximum bounds.
    /// </summary>
    /// <value>
    /// The maximum bounds.
    /// </value>
    public double[] MaxBounds { get; set; } = { double.MinValue, double.MinValue };

    /// <summary>
    /// Gets or sets the minimum bounds.
    /// </summary>
    /// <value>
    /// The minimum bounds.
    /// </value>
    public double[] MinBounds { get; set; } = { double.MaxValue, double.MaxValue };

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public LandData[] Data { get; set; } = Array.Empty<LandData>();
}
