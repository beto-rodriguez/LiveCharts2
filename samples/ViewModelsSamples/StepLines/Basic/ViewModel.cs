using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.StepLines.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new StepLineSeries<double?>
            {
                Values = new ObservableCollection<double?> { 2, 1, 3, 4, 3, 4, 6 },
                Fill = null
            }
        };
    }
}
