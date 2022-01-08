using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Test.Dispose;

public class ViewModel
{
    private readonly Random _r = new();

    public ViewModel()
    {
        var data = new List<double>();

        for (var i = 0; i < 1000; i++)
        {
            data.Add(_r.Next(0, 10));
        }

        Series = new ISeries[]
        {
            new ColumnSeries<double> { Values = data }
        };
    }

    public ISeries[]? Series { get; }
}
