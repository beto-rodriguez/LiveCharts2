using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Bars.Basic;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
            }
        };
}
