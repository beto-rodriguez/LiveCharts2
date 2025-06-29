using System;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Pies.AngularGauge;

public class View : Panel
{
    private readonly PieChart pieChart;
    private readonly NeedleVisual needle;
    private readonly Random _random = new();

    public View()
    {
        var sectionsOuter = 130;
        var sectionsWidth = 20;

        needle = new NeedleVisual
        {
            Value = 45
        };

        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildAngularGaugeSections(
                new GaugeItem(60, s => SetStyle(sectionsOuter, sectionsWidth, s)),
                new GaugeItem(30, s => SetStyle(sectionsOuter, sectionsWidth, s)),
                new GaugeItem(10, s => SetStyle(sectionsOuter, sectionsWidth, s))),
            VisualElements = [
                new AngularTicksVisual
                {
                    Labeler = value => value.ToString("N1"),
                    LabelsSize = 16,
                    LabelsOuterOffset = 15,
                    OuterOffset = 65,
                    TicksLength = 20
                },
                needle
            ],
            InitialRotation = -225,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100
        };

        var b1 = new Button { Text = "Update" };
        b1.Click += (sender, e) => needle.Value = _random.Next(0, 100);

        Content = new DynamicLayout(new StackLayout(b1), pieChart);
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
        series.OuterRadiusOffset = sectionsOuter;
        series.MaxRadialColumnWidth = sectionsWidth;
        series.CornerRadius = 0;
    }
}
