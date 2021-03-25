using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.General.Axis
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 6, 7, 8, 3, 1, 4, 6 }
            }
        };
    }
}
