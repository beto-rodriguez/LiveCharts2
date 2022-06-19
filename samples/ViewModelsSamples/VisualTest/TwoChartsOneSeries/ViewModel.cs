﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.VisualTest.TwoChartsOneSeries;

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

        Series = new ISeries[] { new StepLineSeries<int> { Values = values } };
    }

    public ISeries[] Series { get; set; }
}
