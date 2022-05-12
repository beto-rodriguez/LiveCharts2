using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Paging;

public class ViewModel : INotifyPropertyChanged
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

        Series = new[]
        {
            new ColumnSeries<int>
            {
                Values = values
            }
        };

        XAxes = new[] { new Axis() };
    }

    public ISeries[] Series { get; set; }

    public Axis[] XAxes { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand GoToPage1Command => new Command(o => GoToPage1());
    public ICommand GoToPage2Command => new Command(o => GoToPage2());
    public ICommand GoToPage3Command => new Command(o => GoToPage3());
    public ICommand SeeAllCommand => new Command(o => SeeAll());

    public void GoToPage1()
    {
        var axis = XAxes[0];
        axis.MinLimit = -0.5;
        axis.MaxLimit = 10.5;
    }

    public void GoToPage2()
    {
        var axis = XAxes[0];
        axis.MinLimit = 9.5;
        axis.MaxLimit = 20.5;
    }

    public void GoToPage3()
    {
        var axis = XAxes[0];
        axis.MinLimit = 19.5;
        axis.MaxLimit = 30.5;
    }

    public void SeeAll()
    {
        var axis = XAxes[0];
        axis.MinLimit = null;
        axis.MaxLimit = null;
    }
}
