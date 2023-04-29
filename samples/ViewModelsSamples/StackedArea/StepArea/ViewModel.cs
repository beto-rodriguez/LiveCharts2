using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StackedArea.StepArea;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new StackedStepAreaSeries<double>
        {
            Values = new List<double> { 3, 2, 3, 5, 3, 4, 6 }
        },
        new StackedStepAreaSeries<double>
        {
            Values = new List<double> { 6, 5, 6, 3, 8, 5, 2 }
        },
        new StackedStepAreaSeries<double>
        {
            Values = new List<double> { 4, 8, 2, 8, 9, 5, 3 }
        }
    };
}
