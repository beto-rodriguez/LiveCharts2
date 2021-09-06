using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.VisualTest.DataTemplate
{
    public class ViewModel
    {
        public IEnumerable<IEnumerable<ISeries>> Models { get; set; }
            = new List<IEnumerable<ISeries>>
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
}
