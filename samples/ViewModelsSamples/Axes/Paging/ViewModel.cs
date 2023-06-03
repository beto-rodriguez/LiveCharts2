using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Paging;

public partial class ViewModel : ObservableObject
{
    private readonly Random _random = new();

    public ViewModel()
    {
        var trend = 100;
        var values = new List<int>();

        for (var i = 0; i < 100; i++)
        {
            trend += _random.Next(-30, 50);
            values.Add(trend);
        }

        Series = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values
            }
        };

        XAxes = new[] { new Axis() };
    }

    public ISeries[] Series { get; }

    public Axis[] XAxes { get; }

    [RelayCommand]
    public void GoToPage1()
    {
        var axis = XAxes[0];
        axis.MinLimit = -0.5;
        axis.MaxLimit = 10.5;
    }

    [RelayCommand]
    public void GoToPage2()
    {
        var axis = XAxes[0];
        axis.MinLimit = 9.5;
        axis.MaxLimit = 20.5;
    }

    [RelayCommand]
    public void GoToPage3()
    {
        var axis = XAxes[0];
        axis.MinLimit = 19.5;
        axis.MaxLimit = 30.5;
    }

    [RelayCommand]
    public void SeeAll()
    {
        var axis = XAxes[0];
        axis.MinLimit = null;
        axis.MaxLimit = null;
    }
}
