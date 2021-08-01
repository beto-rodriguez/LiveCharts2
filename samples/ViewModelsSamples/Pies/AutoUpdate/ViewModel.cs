using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModelsSamples.Pies.AutoUpdate
{
    public class ViewModel
    {
        private Random random = new Random();

        public ViewModel()
        {
            // using a collection that implements INotifyCollectionChanged as your series collection
            // will allow the chart to update every time a series is added, removed, replaced or the whole list was cleared
            // .Net already provides the System.Collections.ObjectModel.ObservableCollection class
            Series = new ObservableCollection<ISeries>
            {
                // using object that implements INotifyPropertyChanged
                // will allow the chart to update everytime a property in a point changes.

                // LiveCharts already provides the ObservableValue class
                // notice you can plot any type, but you must let LiveCharts know how to handle it
                // for more info please see:
                // https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/ViewModelsSamples/General/UserDefinedTypes/ViewModel.cs#L22
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(2) } },
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(5) } },
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(3) } },
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(7) } },
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(4) } },
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(3) } }
            };
        }

        public ObservableCollection<ISeries> Series { get; set; }

        public void AddSeries()
        {
            //  for this sample only 15 series are supported.
            if (Series.Count == 15) return;

            Series.Add(
                new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(random.Next(1, 10)) } });
        }

        public void UpdateAll()
        {
            foreach (var series in Series)
            {
                foreach (var value in series.Values)
                {
                    var observableValue = (ObservableValue)value;
                    observableValue.Value = random.Next(1, 10);
                }
            }
        }

        public void RemoveLastSeries()
        {
            if (Series.Count == 1) return;

            Series.RemoveAt(Series.Count - 1);
        }

        // The next commands are only to enable XAML bindings
        // they are not used in the WinForms sample
        public ICommand AddSeriesCommand => new Command(o => AddSeries());
        public ICommand UpdateAllCommand => new Command(o => UpdateAll());
        public ICommand RemoveSeriesCommand => new Command(o => RemoveLastSeries());
    }
}
