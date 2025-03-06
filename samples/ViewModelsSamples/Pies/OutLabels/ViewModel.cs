using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.OutLabels;

public class ViewModel
{
    private static int _index = 0;
    private static string[] _names = ["Maria", "Susan", "Charles", "Fiona", "George"];

    public IEnumerable<ISeries> Series { get; set; } =
         new[] { 8, 6, 5, 3, 3 }.AsPieSeries((value, series) =>
         {
             series.Name = _names[_index++ % _names.Length];
             series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer; // mark
             series.DataLabelsSize = 15;
             series.ShowDataLabels = true;
             series.DataLabelsFormatter =
                point =>
                    $"This slide takes {point.Coordinate.PrimaryValue} " +
                    $"out of {point.StackedValue!.Total} parts";
             series.ToolTipLabelFormatter = point => $"{point.StackedValue!.Share:P2}";
         });
}
