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
using System.Runtime.CompilerServices;
using LiveChartsCore.Kernel;

namespace LiveChartsCore.Defaults;

/// <summary>
/// Defines a time span point for the Cartesian coordinate system that implements <see cref="INotifyPropertyChanged"/>.
/// </summary>
public class TimeSpanPoint : IChartEntity, INotifyPropertyChanged
{
    private TimeSpan _timeSpan;
    private double? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanPoint"/> class.
    /// </summary>
    public TimeSpanPoint()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanPoint"/> class.
    /// </summary>
    /// <param name="timeSpan">The date time.</param>
    /// <param name="value">The value.</param>
    public TimeSpanPoint(TimeSpan timeSpan, double? value)
    {
        TimeSpan = timeSpan;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the time span.
    /// </summary>
    /// <value>
    /// The date time.
    /// </value>
    public TimeSpan TimeSpan { get => _timeSpan; set { _timeSpan = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public double? Value { get => _value; set { _value = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartEntity.MetaData"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public ChartEntityMetaData? MetaData { get; set; }

    /// <inheritdoc cref="IChartEntity.Coordinate"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public Coordinate Coordinate { get; set; } = Coordinate.Empty;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Called when a property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        Coordinate = _value is null
            ? Coordinate.Empty
            : new(_timeSpan.Ticks, _value ?? 0d);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
