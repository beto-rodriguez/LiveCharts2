using System;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Pies.Processing;

public partial class ViewModel : ObservableObject
{
    private readonly ObservableValue _processing;
    private readonly ObservableValue _completed;
    private readonly ObservableValue _failed;

    public ViewModel()
    {
        _processing = new ObservableValue(200);
        _completed = new ObservableValue(100);
        _failed = new ObservableValue(100);

        Series =
        [
                new PieSeries<ObservableValue>
                {
                    Name = "Processing",
                    Values = [_processing],
                    //InnerRadius = 45
                },
                new PieSeries<ObservableValue>
                {
                    Name = "Failed",
                    Values = [_failed],
                    //InnerRadius = 45
                },
                new PieSeries<ObservableValue>
                {
                    Name = "Completed",
                    Values = [_completed],
                    //InnerRadius = 45
                }
        ];

        // the ValueSeries property is a workaround for WPF only.
        ValueSeries =
        [
            new() { Value = _processing, Series = Series[0] },
            new() { Value = _failed, Series = Series[1] },
            new() { Value = _completed, Series = Series[2] }
        ];

        Value = _processing;

        Read();
    }

    public ISeries[] Series { get; set; }

    public ObservableValue Value { get; set; }

    public ValueSeries[] ValueSeries { get; set; }

    public async void Read()
    {
        if (_processing.Value is null) throw new InvalidOperationException();

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
            ValueSeries =
            [
                    new() { Value = _processing, Series = Series[0] },
                    new() { Value = _failed, Series = Series[1] },
                    new() { Value = _completed, Series = Series[2] }
            ];
            OnPropertyChanged(nameof(ValueSeries));
            OnPropertyChanged(nameof(Value));
            isProcessing = _completed.Value < 1000;
        }
    }
}
