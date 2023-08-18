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
/// Defines a point for box plots.
/// </summary>
public class BoxValue : IChartEntity, INotifyPropertyChanged
{
    private double _max;
    private double _third;
    private double _first;
    private double _min;
    private double _median;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialPoint"/> class.
    /// </summary>
    public BoxValue()
    {
        MetaData = new ChartEntityMetaData(OnCoordinateChanged);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialPoint"/> class.
    /// </summary>
    /// <param name="max">The max.</param>
    /// <param name="thirdQuartile">The third quartile.</param>
    /// <param name="firstQuartile">The first quartile.</param>
    /// <param name="min">The min.</param>
    /// <param name="median">The median.</param>
    public BoxValue(
        double max, double thirdQuartile, double firstQuartile, double min, double median)
            : this()
    {
        Max = max;
        ThirdQuartile = thirdQuartile;
        FirtQuartile = firstQuartile;
        Min = min;
        Median = median;
    }

    /// <summary>
    /// Gets or sets the high.
    /// </summary>
    /// <value>
    /// The high.
    /// </value>
    public double Max { get => _max; set { _max = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the max quiadrile value.
    /// </summary>
    /// <value>
    /// The open.
    /// </value>
    public double ThirdQuartile { get => _third; set { _third = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the min quadrile value.
    /// </summary>
    /// <value>
    /// The close.
    /// </value>
    public double FirtQuartile { get => _first; set { _first = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the min value.
    /// </summary>
    /// <value>
    /// The low.
    /// </value>
    public double Min { get => _min; set { _min = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the median value.
    /// </summary>
    /// <value>
    /// The low.
    /// </value>
    public double Median { get => _median; set { _median = value; OnPropertyChanged(); } }

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
        if (MetaData is not null) OnCoordinateChanged(MetaData.EntityIndex);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the coordinate changed.
    /// </summary>
    protected virtual void OnCoordinateChanged(int index)
    {
        Coordinate = new(index, _max, _third, _first, _min, _median);
    }
}
