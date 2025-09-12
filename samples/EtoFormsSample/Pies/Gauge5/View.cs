using System;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.Gauge5;

public class View : Panel
{
    private readonly PieChart pieChart;
    private readonly ObservableValue observableValue1 = new() { Value = 50 };
    private readonly ObservableValue observableValue2 = new() { Value = 80 };
    private readonly Random _random = new();

    public View()
    {
        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(observableValue1, series =>
                {
                    series.Name = "North";
                    series.DataLabelsPosition = PolarLabelsPosition.Start;
                }),
                new GaugeItem(observableValue2, series =>
                {
                    series.Name = "South";
                    series.DataLabelsPosition = PolarLabelsPosition.Start;
                })),
            InitialRotation = -90,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100,
            LegendPosition = LegendPosition.Bottom
        };

        var b1 = new Button { Text = "Update" };
        b1.Click += (sender, e) =>
        {
            observableValue1.Value = _random.Next(0, 100);
            observableValue2.Value = _random.Next(0, 100);
        };

        Content = new DynamicLayout(new StackLayout(b1), pieChart);
    }
}
