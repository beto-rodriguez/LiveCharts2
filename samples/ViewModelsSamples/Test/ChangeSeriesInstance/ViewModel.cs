using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Test.ChangeSeriesInstance;

public class ViewModel : INotifyPropertyChanged
{
    private readonly Random _r = new();
    private ISeries[]? _data;

    public ViewModel()
    {
        GenerateData();
    }

    public ISeries[]? Data { get => _data; set { _data = value; OnPropertyChanged(); } }

    public ICommand GenerateDataCommand => new Command(o => GenerateData());

    public event PropertyChangedEventHandler? PropertyChanged;

    public void GenerateData()
    {
        Data = new ISeries[]
        {
            new ColumnSeries<double> { Values = new double[] { _r.Next(0, 10), _r.Next(0, 10), _r.Next(0, 10) } }
        };
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
