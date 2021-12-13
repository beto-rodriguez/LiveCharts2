using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
    };
}
