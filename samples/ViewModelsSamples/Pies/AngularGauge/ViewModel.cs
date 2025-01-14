using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.VisualElements;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.Defaults;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ViewModelsSamples.Pies.AngularGauge;

public partial class ViewModel
{
    private readonly Random _random = new();

    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement> VisualElements { get; set; }
    public NeedleVisual Needle { get; set; }

    public ViewModel()
    {
        var sectionsOuter = 130;
        var sectionsWidth = 20;

        Needle = new NeedleVisual
        {
            Value = 45
        };

        Series = GaugeGenerator.BuildAngularGaugeSections(
            new GaugeItem(60, s => SetStyle(sectionsOuter, sectionsWidth, s)),
            new GaugeItem(30, s => SetStyle(sectionsOuter, sectionsWidth, s)),
            new GaugeItem(10, s => SetStyle(sectionsOuter, sectionsWidth, s)));

        VisualElements =
        [
            new AngularTicksVisual
            {
                Labeler = value => value.ToString("N1"),
                LabelsSize = 16,
                LabelsOuterOffset = 15,
                OuterOffset = 65,
                TicksLength = 20
            },
            Needle
        ];
    }

    [RelayCommand]
    public void DoRandomChange()
    {
        // modifying the Value property updates and animates the chart automatically
        Needle.Value = _random.Next(0, 100);
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
        series.OuterRadiusOffset = sectionsOuter;
        series.MaxRadialColumnWidth = sectionsWidth;
        series.CornerRadius = 0;
    }
}
