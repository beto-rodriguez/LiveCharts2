using System.Collections.Generic;
using LiveChartsCore;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Extensions;
using System;

namespace ViewModelsSamples.Pies.OutLabels;

public partial class ViewModel : ObservableObject
{
    public IEnumerable<ISeries> Series { get; set; } =
         new[] { 8, 6, 5, 3, 3 }.AsPieSeries((value, series) =>
         {
             series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer; // mark
             series.DataLabelsSize = 15;
             series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
             series.DataLabelsFormatter =
                point =>
                    $"This slide takes{Environment.NewLine}" +
                    $"{point.Coordinate.PrimaryValue} out of {point.StackedValue!.Total} parts";
         });

    // add some margin to the chart so the labels can be drawn // mark
    public LiveChartsCore.Measure.Margin DrawMargin { get; set; } =
        new LiveChartsCore.Measure.Margin(0);
}
