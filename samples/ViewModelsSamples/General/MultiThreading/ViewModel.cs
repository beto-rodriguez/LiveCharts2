using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.MultiThreading;

[ObservableObject]
public partial class ViewModel
{
    private readonly Random _r = new();
    private readonly int _delay = 100;
    private readonly ObservableCollection<int> _values;
    private int _current;

    public ViewModel()
    {
        // notice this case is not working in Avalonia, use the invoke on UI thread alternative (MultiThreading2).

        var items = new List<int>();
        for (var i = 0; i < 1500; i++)
        {
            _current += _r.Next(-9, 10);
            items.Add(_current);
        }

        _values = new ObservableCollection<int>(items);

        Series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = _values,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Blue, 1)
            }
        };

        _delay = 1;
        var readTasks = 10;

        // create {readTasks} parallel tasks that will add a point every {_delay} milliseconds
        for (var i = 0; i < readTasks; i++)
        {
            _ = Task.Run(ReadData);
        }
    }

    public ISeries[] Series { get; set; }

    public object Sync { get; } = new object();

    private async void ReadData()
    {
        await Task.Delay(1000);

        while (true)
        {
            await Task.Delay(_delay);

            _current = Interlocked.Add(ref _current, _r.Next(-9, 10));

            lock (Sync)
            {
                _values.Add(_current);
                _values.RemoveAt(0);
            }
        }
    }
}
