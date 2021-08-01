﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Processing
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly ObservableValue _processing;
        private readonly ObservableValue _completed;
        private readonly ObservableValue _failed;

        public ViewModel()
        {
            _processing = new ObservableValue(200);
            _completed = new ObservableValue(100);
            _failed = new ObservableValue(100);

            Series = new ISeries[]
            {
                new PieSeries<ObservableValue>
                {
                    Name = "Processing",
                    Values = new []{ _processing },
                    //InnerRadius = 45
                },
                new PieSeries<ObservableValue>
                {
                    Name = "Failed",
                    Values = new []{ _failed },
                    //InnerRadius = 45
                },
                new PieSeries<ObservableValue>
                {
                    Name = "Completed",
                    Values = new []{ _completed },
                    //InnerRadius = 45
                }
            };

            ValueSeries = new ValueSeries[]
            {
                new ValueSeries{ Value = _processing, Series = Series[0] },
                new ValueSeries{ Value = _failed, Series = Series[1] },
                new ValueSeries{ Value = _completed, Series = Series[2] }
            };

            Value = _processing;

            Read();
        }

        public ISeries[] Series { get; set; }

        public ObservableValue Value { get; set; }

        public ValueSeries[] ValueSeries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void Read()
        {
            var r = new Random();
            var processed = 0;
            var ellapsed = 0;
            var isProcessing = true;

            while (isProcessing)
            {
                var succeed = 0.8 + r.NextDouble() * 0.4;
                var failed = 1 - succeed;

                var totalTask = (int)(0.05 * _processing.Value) + processed;
                var completedTasks = (int)(totalTask * succeed);
                var failedTasks = (int)(totalTask * failed);

                _completed.Value += completedTasks;
                _failed.Value += failedTasks;

                _processing.Value -= completedTasks + failedTasks;

                var newTask = r.Next(5, 20) * (1 - ellapsed / 10000);
                if (newTask < 0) newTask = 0;
                _processing.Value += newTask;

                ellapsed += 100;
                await Task.Delay(100);
                processed = newTask;

                //Value = new ObservableValue(_processing.Value ?? 0);
                ValueSeries = new ValueSeries[]
                {
                    new ValueSeries{ Value = _processing, Series = Series[0] },
                    new ValueSeries{ Value = _failed, Series = Series[1] },
                    new ValueSeries{ Value = _completed, Series = Series[2] }
                };
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueSeries)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                isProcessing = _completed.Value < 1000;
            }
        }
    }

    public class ValueSeries
    {
        public ObservableValue Value { get; set; }
        public ISeries Series { get; set; }
    }
}
