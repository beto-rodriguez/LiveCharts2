using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Scatter.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint> 
                {
                    new ObservablePoint(2.2, 5.4),
                    new ObservablePoint(4.5, 2.5),
                    new ObservablePoint(4.2, 7.4),
                    new ObservablePoint(6.4, 9.9),
                    new ObservablePoint(4.2, 9.2),
                    new ObservablePoint(5.8, 3.5),
                    new ObservablePoint(7.3, 5.8),
                    new ObservablePoint(8.9, 3.9),
                    new ObservablePoint(6.1, 4.6),
                    new ObservablePoint(9.4, 7.7),
                    new ObservablePoint(8.4, 8.5),
                }
            }
        };
    }
}
