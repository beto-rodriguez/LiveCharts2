using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModelsSamples.Bars.AutoUpdate
{
    public class ViewModel
    {
        private int index = 0;
        private Random random = new Random();
        private ObservableCollection<ObservablePoint> observableValues;

        public ViewModel()
        {
            // using an INotifyCollectionChanged as your values collection
            // will let the chart update every time a point is added, removed, replaced or the whole list was cleared
            observableValues = new ObservableCollection<ObservablePoint>
            {
                // using object that implements INotifyPropertyChanged
                // will allow the chart to update everytime a property in a point changes.

                // LiveCharts already provides the ObservableValue class
                // notice you can plot any type, but you must let LiveCharts know how to handle it
                // for more info please see:
                // https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/ViewModelsSamples/General/UserDefinedTypes/ViewModel.cs#L22

                new ObservablePoint(index++, 2),
                new ObservablePoint(index++, 5),
                new ObservablePoint(index++, 4),
                new ObservablePoint(index++, 5),
                new ObservablePoint(index++, 2),
                new ObservablePoint(index++, 6),
                new ObservablePoint(index++, 6),
                new ObservablePoint(index++, 6),
                new ObservablePoint(index++, 4),
                new ObservablePoint(index++, 2),
                new ObservablePoint(index++, 3),
                new ObservablePoint(index++, 8),
                new ObservablePoint(index++, 3)
            };

            // using a collection that implements INotifyCollectionChanged as your series collection
            // will allow the chart to update every time a series is added, removed, replaced or the whole list was cleared
            // .Net already provides the System.Collections.ObjectModel.ObservableCollection class
            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<ObservablePoint> { Values = observableValues }
            };

            // in the following series notice that the type int does not implement INotifyPropertyChanged
            // and our Series.Values collection is of type List<T>
            // List<T> does not implement INotifyCollectionChanged
            // this means the following series is not listening for changes.
            //Series.Add(new ColumnSeries<int> { Values = new List<int> { 2, 4, 6, 1, 7, -2 } });
        }

        public ObservableCollection<ISeries> Series { get; set; }

        public void AddRandomItem()
        {
            // for this sample only 50 items are supported.
            if (observableValues.Count > 50) return;

            var randomValue = random.Next(1, 10);
            observableValues.Add(new ObservablePoint(index++, randomValue));
        }

        public void RemoveFirstItem()
        {
            if (observableValues.Count < 2) return;

            observableValues.RemoveAt(0);
        }

        public void ReplaceRandomItem()
        {
            var randomValue = random.Next(1, 10);
            var randomIndex = random.Next(0, observableValues.Count - 1);
            observableValues[randomIndex] = new ObservablePoint(observableValues[randomIndex].X, randomValue);
        }

        public void AddSeries()
        {
            //  for this sample only 5 series are supported.
            if (Series.Count == 5) return;

            Series.Add(
                new ColumnSeries<int>
                {
                    Values = new List<int> { random.Next(0, 10), random.Next(0, 10), random.Next(0, 10) }
                });
        }

        public void RemoveLastSeries()
        {
            if (Series.Count == 1) return;

            Series.RemoveAt(Series.Count - 1);
        }

        // The next commands are only to enable XAML bindings
        // they are not used in the WinForms sample
        public ICommand AddItemCommand => new Command(o => AddRandomItem());
        public ICommand RemoveItemCommand => new Command(o => RemoveFirstItem());
        public ICommand ReplaceItemCommand => new Command(o => ReplaceRandomItem());
        public ICommand AddSeriesCommand => new Command(o => AddSeries());
        public ICommand RemoveSeriesCommand => new Command(o => RemoveLastSeries());
    }
}
