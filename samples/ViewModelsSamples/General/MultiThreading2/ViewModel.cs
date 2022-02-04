using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.MultiThreading2;

public class ViewModel
{
    private readonly Random _r = new();
    private readonly int _delay = 100;
    private readonly ObservableCollection<int> _values;
    private static int s_current;
    private readonly Action<Action> _uiThreadInvoker;

    public ViewModel(Action<Action> uiThreadInvoker)
    {
        var items = new List<int>();
        for (var i = 0; i < 1500; i++)
        {
            s_current += _r.Next(-9, 10);
            items.Add(s_current);
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

        _uiThreadInvoker = uiThreadInvoker;
        _delay = 1;
        var readTasks = 10;

        // create {readTasks} parallel tasks that will add a point every {_delay} milliseconds
        for (var i = 0; i < readTasks; i++)
        {
            ReadData();
        }
    }

    public ISeries[] Series { get; set; }

    public async void ReadData()
    {
        await Task.Delay(1000);

        while (true)
        {
            await Task.Delay(_delay);

            // force the change to happen in the UI thread.
            _uiThreadInvoker(() =>
            {
                s_current += _r.Next(-9, 10);
                _values.Add(s_current);
                _values.RemoveAt(0);
            });
        }
    }
}
