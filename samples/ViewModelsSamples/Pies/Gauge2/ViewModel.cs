using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Gauge2;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.YellowGreen);
                series.DataLabelsSize = 50;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 75;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));
}
