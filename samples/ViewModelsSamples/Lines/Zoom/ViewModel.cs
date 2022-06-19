﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Zoom;

[ObservableObject]
public partial class ViewModel
{
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

        SeriesCollection = new ISeries[] { new LineSeries<int> { Values = values } };
    }

    public ISeries[] SeriesCollection { get; set; }
}
