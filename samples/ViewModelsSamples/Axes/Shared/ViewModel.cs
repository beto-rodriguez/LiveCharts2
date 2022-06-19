﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Shared;

[ObservableObject]
public partial class ViewModel
{
    public ViewModel()
    {
        var values1 = new int[50];
        var values2 = new int[50];
        var r = new Random();
        var t = 0;
        var t2 = 0;

        for (var i = 0; i < 50; i++)
        {
            t += r.Next(-90, 100);
            values1[i] = t;

            t2 += r.Next(-90, 100);
            values2[i] = t2;
        }

        SeriesCollection1 = new ISeries[] { new LineSeries<int> { Values = values1 } };
        SeriesCollection2 = new ISeries[] { new ColumnSeries<int> { Values = values2 } };

        // sharing the same instance for both charts will keep the zooming and panning synced // mark
        SharedXAxis = new Axis[] { new Axis() };
    }

    public ISeries[] SeriesCollection1 { get; set; }
    public ISeries[] SeriesCollection2 { get; set; }
    public Axis[] SharedXAxis { get; set; }
}
