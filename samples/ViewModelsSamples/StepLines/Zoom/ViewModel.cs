using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StepLines.Zoom;

public class ViewModel
{
    public ISeries[] SeriesCollection { get; set; }

    public ViewModel()
    {
        var values = new int[100];
        var r = new Random();
        var t = 0;

        for (var i = 0; i < 100; i++)
        {
            t += r.Next(-90, 100);
            values[i] = t;
        }

        SeriesCollection = [new StepLineSeries<int> { Values = values }];
    }
}
