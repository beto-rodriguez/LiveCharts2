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

using LiveChartsCore.Kernel;
using Microsoft.UI.Xaml.Data;

namespace LiveChartsCore.SkiaSharpView.WinUI.Binding;

/// <summary>
/// Defines a bindable chart point.
/// </summary>
[Bindable]
public class BindableChartPoint
{
    private readonly ChartPoint _chartPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPoint"/> class.
    /// </summary>
    /// <param name="chartpoint">The chartpoint.</param>
    public BindableChartPoint(ChartPoint chartpoint)
    {
        _chartPoint = chartpoint;

        IsNull = chartpoint.IsNull;
        PrimaryValue = chartpoint.PrimaryValue;
        SecondaryValue = chartpoint.SecondaryValue;
        TertiaryValue = chartpoint.TertiaryValue;
        QuaternaryValue = chartpoint.QuaternaryValue;
        QuinaryValue = chartpoint.QuinaryValue;

        if (chartpoint.StackedValue is not null)
            StackedValue = new BindableStackedValue
            {
                Start = chartpoint.StackedValue.Start,
                End = chartpoint.StackedValue.End,
                Total = chartpoint.StackedValue.Total,
                NegativeStart = chartpoint.StackedValue.NegativeStart,
                NegativeEnd = chartpoint.StackedValue.NegativeEnd,
                NegativeTotal = chartpoint.StackedValue.NegativeTotal,
            };

        Context = new BindableChartPointContext(chartpoint.Context);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is null.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
    /// </value>
    public bool IsNull { get; set; }

    /// <summary>
    /// Gets or sets the primary value.
    /// </summary>
    /// <value>
    /// The primary value.
    /// </value>
    public double PrimaryValue { get; set; }

    /// <summary>
    /// Gets or sets the secondary value.
    /// </summary>
    /// <value>
    /// The secondary value.
    /// </value>
    public double SecondaryValue { get; set; }

    /// <summary>
    /// Gets or sets the tertiary value.
    /// </summary>
    /// <value>
    /// The tertiary value.
    /// </value>
    public double TertiaryValue { get; set; }

    /// <summary>
    /// Gets or sets the quaternary value.
    /// </summary>
    /// <value>
    /// The quaternary value.
    /// </value>
    public double QuaternaryValue { get; set; }

    /// <summary>
    /// Gets or sets the quinary value.
    /// </summary>
    /// <value>
    /// The quinary value.
    /// </value>
    public double QuinaryValue { get; set; }

    /// <summary>
    /// Gets or sets the stacked value, if the point do not belongs to a stacked series then this property is null.
    /// </summary>
    public BindableStackedValue StackedValue { get; set; }

    /// <summary>
    /// Gets the point as data label.
    /// </summary>
    /// <value>
    /// As tooltip string.
    /// </value>
    public string AsDataLabel => _chartPoint.Context.Series.GetDataLabelText(_chartPoint);

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    public BindableChartPointContext Context { get; }
}
