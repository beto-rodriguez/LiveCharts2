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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Defaults;

/// <summary>
/// Defines an object that notifies when the value property changes.
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public class ObservableValue : IChartEntity, INotifyPropertyChanged
{
    private double? _value;
    private int _entityIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValue"/> class.
    /// </summary>
    public ObservableValue()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValue"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ObservableValue(double? value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public double? Value { get => _value; set { _value = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartEntity.EntityIndex"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public int EntityIndex
    {
        get => _entityIndex;
        set
        {
            // the coordinate of this type depends on the index of element in the data collection.
            // we update the coordinate if the index changed.
            if (value == _entityIndex) return;
            _entityIndex = value;
            OnCoordinateChanged();
        }
    }

    /// <inheritdoc cref="IChartEntity.ChartPoints"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public Dictionary<IChartView, ChartPoint>? ChartPoints { get; set; }

    /// <inheritdoc cref="IChartEntity.Coordinate"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public Coordinate Coordinate { get; private set; } = Coordinate.Empty;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Called when am property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnCoordinateChanged();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the coordinate changed.
    /// </summary>
    protected virtual void OnCoordinateChanged()
    {
        Coordinate = _value is null
            ? Coordinate.Empty
            : new(EntityIndex, _value.Value);
    }
}
