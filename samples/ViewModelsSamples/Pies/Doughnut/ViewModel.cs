using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Doughnut;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } = new ISeries[]
    {
        new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50 },
        new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50 },
        new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50 },
        new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50 },
        new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50 }
    };
}
