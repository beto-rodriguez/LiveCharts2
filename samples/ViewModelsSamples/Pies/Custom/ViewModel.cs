using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Custom;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new PieSeries<double> { Values = new List<double> { 4 }, MaxOuterRadius = 0.60 },
        new PieSeries<double> { Values = new List<double> { 5 }, MaxOuterRadius = 0.65 },
        new PieSeries<double> { Values = new List<double> { 3 }, MaxOuterRadius = 0.70 },
        new PieSeries<double> { Values = new List<double> { 5 }, MaxOuterRadius = 0.85 },
        new PieSeries<double> { Values = new List<double> { 7 }, MaxOuterRadius = 1.00 },
    };
}
