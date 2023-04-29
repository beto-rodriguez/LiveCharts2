using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Pushout;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
        new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
        new PieSeries<double> { Values = new List<double> { 3 }, Pushout = 4 },
        new PieSeries<double> { Values = new List<double> { 2 }, Pushout = 4 },
        new PieSeries<double> { Values = new List<double> { 5 }, Pushout = 30 }
    };
}
