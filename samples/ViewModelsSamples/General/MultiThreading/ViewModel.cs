using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.MultiThreading;

public class ViewModel
{
    private readonly Random _r = new();
    private readonly int _delay = 100;
    private readonly ObservableCollection<ObservableValue> _values;
    private volatile object _sync = new { ImNotAString = "no you are not." };
    private int _current;

    public ViewModel()
    {
        var items = new List<ObservableValue>();
        for (var i = 0; i < 1500; i++)
        {
            _current += _r.Next(-9, 10);
            items.Add(new ObservableValue(_current));
        }

        _values = new ObservableCollection<ObservableValue>(items);

        Series = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = _values,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Blue, 1)
            }
        };

        _delay = 1;
        var readTasks = 100;

        // create {readTasks} parallel tasks that will add a point every {_delay} milliseconds
        for (var i = 0; i < readTasks; i++)
        {
            _ = Task.Run(ReadData);
        }
    }

    public ISeries[] Series { get; set; }

    public object Sync => _sync;

    private async Task ReadData()
    {
        await Task.Delay(1000);

        while (true)
        {
            await Task.Delay(_delay);

            _current = Interlocked.Add(ref _current, _r.Next(-9, 10));

            lock (Sync)
            {
                _values.Add(new ObservableValue(_current));
                _values.RemoveAt(0);
            }
        }
    }
}
