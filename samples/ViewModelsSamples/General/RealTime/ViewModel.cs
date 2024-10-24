using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.RealTime;

public class ViewModel
{
    private readonly Random _random = new();
    private readonly List<DateTimePoint> _values = [];
    private readonly DateTimeAxis _customAxis;

    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] XAxes { get; set; }

    public object Sync { get; } = new object();

    public bool IsReading { get; set; } = true;

    public ViewModel()
    {
        Series = [
            new LineSeries<DateTimePoint>
            {
                Values = _values,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        ];

        _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
        {
            CustomSeparators = GetSeparators(),
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
        };

        XAxes = [_customAxis];

        _ = ReadData();
    }

    private async Task ReadData()
    {
        // to keep this sample simple, we run the next infinite loop // mark
        // in a real application you should stop the loop/task when the view is disposed // mark

        while (IsReading)
        {
            await Task.Delay(100);

            // Because we are updating the chart from a different thread // mark
            // we need to use a lock to access the chart data. // mark
            // this is not necessary if your changes are made on the UI thread. // mark
            lock (Sync)
            {
                _values.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 10)));
                if (_values.Count > 250) _values.RemoveAt(0);

                // we need to update the separators every time we add a new point // mark
                _customAxis.CustomSeparators = GetSeparators();
            }
        }
    }

    private static double[] GetSeparators()
    {
        var now = DateTime.Now;

        return
        [
            now.AddSeconds(-25).Ticks,
            now.AddSeconds(-20).Ticks,
            now.AddSeconds(-15).Ticks,
            now.AddSeconds(-10).Ticks,
            now.AddSeconds(-5).Ticks,
            now.Ticks
        ];
    }

    private static string Formatter(DateTime date)
    {
        var secsAgo = (DateTime.Now - date).TotalSeconds;

        return secsAgo < 1
            ? "now"
            : $"{secsAgo:N0}s ago";
    }
}
