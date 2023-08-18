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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore.Kernel;

namespace LiveChartsCore.Defaults;

/// <summary>
/// Defines an error point to create error series.
/// </summary>
public class ErrorPoint : IChartEntity, INotifyPropertyChanged
{
    private double? _x;
    private double? _y;
    private double _exi;
    private double _exj;
    private double _eyi;
    private double _eyj;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorValue"/> class.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="xErrorI">The left error.</param>
    /// <param name="xErrorJ">The right error.</param>
    /// <param name="yErrorI">The top error.</param>
    /// <param name="yErrorJ">The bottom error.</param>
    public ErrorPoint(double x, double y, double xErrorI, double xErrorJ, double yErrorI, double yErrorJ)
    {
        X = x;
        Y = y;
        XErrorI = xErrorI;
        XErrorJ = xErrorJ;
        YErrorI = yErrorI;
        YErrorJ = yErrorJ;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorValue"/> class.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="xError">The error in X.</param>
    /// <param name="yError">The error in Y.</param>
    public ErrorPoint(double x, double y, double xError, double yError)
        : this(x, y, xError, xError, yError, yError)
    { }

    /// <summary>
    /// Gets or sets the X coordinate.
    /// </summary>
    /// <value>
    /// The high.
    /// </value>
    public double? X { get => _x; set { _x = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the Y coordinate.
    /// </summary>
    /// <value>
    /// The high.
    /// </value>
    public double? Y { get => _y; set { _y = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the left error in X.
    /// </summary>
    /// <value>
    /// The open.
    /// </value>
    public double XErrorI { get => _exi; set { _exi = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the right error in x.
    /// </summary>
    /// <value>
    /// The close.
    /// </value>
    public double XErrorJ { get => _exj; set { _exj = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the top error in Y.
    /// </summary>
    /// <value>
    /// The open.
    /// </value>
    public double YErrorI { get => _eyi; set { _eyi = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the bottom error in Y.
    /// </summary>
    /// <value>
    /// The close.
    /// </value>
    public double YErrorJ { get => _eyj; set { _eyj = value; OnPropertyChanged(); } }

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
        Coordinate = _y is null || _x is null
            ? Coordinate.Empty
            : new(_y.Value, _x.Value, 0, 0, 0, 0, new(_exi, _exj, _eyi, _eyj));

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
