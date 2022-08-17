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
using System.ComponentModel;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines the <see cref="ChartEntityMetadata"/> class.
/// </summary>
public class ChartEntityMetadata
{
    private readonly Func<Coordinate> _coordinateBuilder;
    private int _entityIndex = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartEntityMetadata"/> class.
    /// </summary>
    /// <param name="coordinateBuilder">The coordinate builder.</param>
    public ChartEntityMetadata(Func<Coordinate> coordinateBuilder)
    {
        _coordinateBuilder = coordinateBuilder;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartEntityMetadata"/> class.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="coordinateBuilder">The coordinate builder.</param>
    public ChartEntityMetadata(INotifyPropertyChanged entity, Func<Coordinate> coordinateBuilder)
    {
        _coordinateBuilder = coordinateBuilder;
        entity.PropertyChanged += OnEntityPropertyChanged;
    }

    /// <summary>
    /// Gets the entity index, a consecutive integer based on the position of the entity in the data collection.
    /// </summary>
    public int EntityIndex
    {
        get => _entityIndex;
        internal set
        {
            var changed = _entityIndex != value;
            _entityIndex = value;
            if (changed) OnCoordinateChanged();
        }
    }

    /// <summary>
    /// Gets the chart point.
    /// </summary>
    public ChartPoint? ChartPoint { get; internal set; } = null;

    /// <summary>
    /// Gets the coordinate.
    /// </summary>
    public Coordinate Coordinate { get; private set; } = Coordinate.Empty;

    private void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnCoordinateChanged();
    }

    private void OnCoordinateChanged()
    {
        Coordinate = _coordinateBuilder();
    }
}
