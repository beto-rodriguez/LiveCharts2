using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.MultiThreading
{
    public class ViewModel
    {
        private readonly Random _r = new Random();
        private readonly int _delay = 100;
        private readonly ObservableCollection<ObservableValue> _values;

        public ViewModel()
        {
            var items = new List<ObservableValue>();
            for (var i = 0; i < 150; i++)
            {
                items.Add(new ObservableValue(_r.Next(0, 10)));
            }

            _values = new ObservableCollection<ObservableValue>(items);

            Series = new ISeries[]
            {
                new LineSeries<ObservableValue>
                {
                    Values = _values
                }
            };

            Sync = new object();

            _delay = 1;
            var readTasks = 1;

            // create {readTasks} parallel tasks that will add a point every {_delay} milliseconds
            for (var i = 0; i < readTasks; i++)
            {
                _ = Task.Run(ReadData);
            }
        }

        public ISeries[] Series { get; set; }

        public object Sync { get; }

        private async Task ReadData()
        {
            await Task.Delay(1000);

            while (true)
            {
                //Trace.WriteLine(
                //   $"Thread id: {Environment.CurrentManagedThreadId}");

                await Task.Delay(_delay);
                lock (Sync)
                {
                    _values.Add(new ObservableValue(_r.Next(0, 10)));
                    _values.RemoveAt(0);
                }
            }
        }
    }
}
