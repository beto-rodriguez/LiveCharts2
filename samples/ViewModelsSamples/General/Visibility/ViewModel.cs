﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Visibility;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 5, 4, 3 },
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 6, 3, 2, 8 },
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 4, 2, 8, 7 },
            IsVisible = true
        }
    };

    [ICommand]
    public void ToggleSeries0()
    {
        Series[0].IsVisible = !Series[0].IsVisible;
    }

    [ICommand]
    public void ToggleSeries1()
    {
        Series[1].IsVisible = !Series[1].IsVisible;
    }

    [ICommand]
    public void ToggleSeries2()
    {
        Series[2].IsVisible = !Series[2].IsVisible;
    }
}
