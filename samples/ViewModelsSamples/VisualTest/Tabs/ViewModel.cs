﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.VisualTest.Tabs;

[ObservableObject]
public partial class ViewModel
{
    public IEnumerable<ISeries> LineSeries { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
        }
    };

    public IEnumerable<ISeries> ColumnSeries { get; set; } = new ObservableCollection<ISeries>
    {
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 }
        }
    };
}
