using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.Defaults;

namespace ViewModelsSamples.Pies.Gauge4;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } =
        GaugeGenerator.BuildSolidGauge(
                new GaugeItem(50, series => SetStyle("Vanessa", series)),
                new GaugeItem(80, series => SetStyle("Charles", series)),
                new GaugeItem(95, series => SetStyle("Ana", series)),
                new GaugeItem(GaugeItem.Background, series =>
                {
                    series.Fill = null;
                }));

    public static void SetStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsSize = 20;
        series.DataLabelsPosition = PolarLabelsPosition.End;
        series.DataLabelsFormatter =
                point => point.Coordinate.PrimaryValue.ToString();
        series.InnerRadius = 20;
        series.MaxRadialColumnWidth = 5;
    }
}
