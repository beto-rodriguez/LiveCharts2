﻿// The MIT License(MIT)
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
using Windows.UI.Xaml.Data;

namespace LiveChartsCore.SkiaSharpView.UnoWinUI.Binding;

/// <summary>
/// Defines a bindable chart point context.
/// </summary>
[Bindable]
public class BindableChartPointContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chartPointContext"></param>
    public BindableChartPointContext(ChartPointContext chartPointContext)
    {
        Chart = chartPointContext.Chart;
        Series = chartPointContext.Series;
        Index = chartPointContext.Index;
        DataSource = chartPointContext.DataSource;
        Visual = chartPointContext.Visual;
        Label = chartPointContext.Label;
    }

    /// <summary>
    /// Gets the chart.
    /// </summary>
    /// <value>
    /// The chart.
    /// </value>
    public object Chart { get; }

    /// <summary>
    /// Gets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public object Series { get; }

    /// <summary>
    /// Gets the position of the point the collection that was used when the point was drawn.
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// Gets the DataSource.
    /// </summary>
    public object? DataSource { get; internal set; }

    /// <summary>
    /// Gets the visual.
    /// </summary>
    /// <value>
    /// The visual.
    /// </value>
    public object? Visual { get; internal set; }

    /// <summary>
    /// Gets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    public object? Label { get; internal set; }
}
