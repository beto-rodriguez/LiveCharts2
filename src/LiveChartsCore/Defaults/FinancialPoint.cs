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
/// Defines a point with financial data.
/// </summary>
public class FinancialPoint : IChartEntity, INotifyPropertyChanged
{
    private double? _high;
    private double? _open;
    private double? _close;
    private double? _low;
    private DateTime _date;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialPoint"/> class.
    /// </summary>
    public FinancialPoint()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialPoint"/> class.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="high">The high.</param>
    /// <param name="open">The open.</param>
    /// <param name="close">The close.</param>
    /// <param name="low">The low.</param>
    public FinancialPoint(DateTime date, double? high, double? open, double? close, double? low)
    {
        Date = date;
        High = high;
        Open = open;
        Close = close;
        Low = low;
    }

    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    /// <value>
    /// The date.
    /// </value>
    public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the high.
    /// </summary>
    /// <value>
    /// The high.
    /// </value>
    public double? High { get => _high; set { _high = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the open.
    /// </summary>
    /// <value>
    /// The open.
    /// </value>
    public double? Open { get => _open; set { _open = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the close.
    /// </summary>
    /// <value>
    /// The close.
    /// </value>
    public double? Close { get => _close; set { _close = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the low.
    /// </summary>
    /// <value>
    /// The low.
    /// </value>
    public double? Low { get => _low; set { _low = value; OnPropertyChanged(); } }

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
        Coordinate = _open is null || _high is null || _low is null || _close is null
            ? Coordinate.Empty
            : new(_date.Ticks, _high.Value, _open.Value, _close.Value, _low.Value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
