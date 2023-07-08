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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Represents additional data required by LiveCharts to draw a point.
/// </summary>
public class ChartEntityMetaData
{
    private readonly Action<int>? _entityIndexChangedCallback;
    private int _entityIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartEntityMetaData"/> class.
    /// </summary>
    /// <param name="onEntityIndexChanged">The callback to call when the entity index changes.</param>
    public ChartEntityMetaData(Action<int>? onEntityIndexChanged = null)
    {
        _entityIndexChangedCallback = onEntityIndexChanged;
    }

    /// <summary>
    /// Gets the entity index, a consecutive integer based on the position of the entity in the data collection.
    /// </summary>
    public int EntityIndex
    {
        get => _entityIndex;
        set
        {
            if (value == _entityIndex) return;
            _entityIndex = value;
            _entityIndexChangedCallback?.Invoke(value);
        }
    }

    /// <summary>
    /// Gets the chart points dictionary.
    /// </summary>
    public Dictionary<IChartView, ChartPoint> ChartPoints { get; set; } = new();
}
