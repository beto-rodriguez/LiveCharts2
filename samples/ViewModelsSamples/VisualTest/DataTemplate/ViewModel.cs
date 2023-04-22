using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.VisualTest.DataTemplate;


public partial class ViewModel : ObservableObject
{
    public IEnumerable<IEnumerable<ISeries>> Models { get; set; } = new List<IEnumerable<ISeries>>
    {
        new ObservableCollection<ISeries>
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
            }
        },
        new ObservableCollection<ISeries>
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
            }
        },
        new ObservableCollection<ISeries>
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
            }
        }
    };
}
