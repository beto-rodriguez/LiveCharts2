using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.NightingaleRose;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50, MaxOuterRadius = 1.0 },
        new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, MaxOuterRadius = 0.9 },
        new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50, MaxOuterRadius = 0.8 },
        new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, MaxOuterRadius = 0.7 },
        new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50, MaxOuterRadius = 0.6 }
    };
}
