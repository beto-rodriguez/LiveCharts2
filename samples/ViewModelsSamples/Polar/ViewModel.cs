using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Polar.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new PolarLineSeries<double>
            {
                Values = new ObservableCollection<double> { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }
            }
        };
    }
}
