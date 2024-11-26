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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;

namespace LiveChartsCore;

/// <summary>
/// Defines a chart data series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <seealso cref="Series{TModel, TVisual, TLabel}" />
/// <seealso cref="IChartSeries" />
/// <remarks>
/// Initializes a new instance of the <see cref="ChartSeries{TModel, TVisual, TLabel}"/> class.
/// </remarks>
/// <param name="properties">The properties.</param>
/// <param name="values">The values.</param>
public abstract class ChartSeries<TModel, TVisual, TLabel>(
    SeriesProperties properties,
    IReadOnlyCollection<TModel>? values)
        : Series<TModel, TVisual, TLabel>(properties, values), IChartSeries
            where TVisual : CoreGeometry, new()
            where TLabel : CoreLabelGeometry, new()
{
    private Paint? _dataLabelsPaint;
    private double _dataLabelsSize = 16;
    private double _dataLabelsRotation = 0;
    private Padding _dataLabelsPadding = new() { Left = 6, Top = 8, Right = 6, Bottom = 8 };
    private double _dataLabelsMaxWidth = LiveCharts.DefaultSettings.MaxTooltipsAndLegendsLabelsWidth;

    /// <inheritdoc cref="IChartSeries.DataLabelsPaint"/>
    public Paint? DataLabelsPaint
    {
        get => _dataLabelsPaint;
        set => SetPaintProperty(ref _dataLabelsPaint, value);
    }

    /// <inheritdoc cref="IChartSeries.DataLabelsSize"/>
    public double DataLabelsSize { get => _dataLabelsSize; set => SetProperty(ref _dataLabelsSize, value); }

    /// <inheritdoc cref="IChartSeries.DataLabelsRotation"/>
    public double DataLabelsRotation { get => _dataLabelsRotation; set => SetProperty(ref _dataLabelsRotation, value); }

    /// <inheritdoc cref="IChartSeries.DataLabelsPadding"/>
    public Padding DataLabelsPadding { get => _dataLabelsPadding; set => SetProperty(ref _dataLabelsPadding, value); }

    /// <inheritdoc cref="IChartSeries.DataLabelsMaxWidth"/>
    public double DataLabelsMaxWidth { get => _dataLabelsMaxWidth; set => SetProperty(ref _dataLabelsMaxWidth, value); }

    void IChartSeries.OnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points, LvcPoint pointer) =>
        OnDataPointerDown(chart, points, pointer);
}
