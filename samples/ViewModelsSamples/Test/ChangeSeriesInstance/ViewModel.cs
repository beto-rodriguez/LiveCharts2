using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Test.ChangeSeriesInstance;

public class ViewModel : INotifyPropertyChanged
{
    private readonly Random _r = new();
    private ISeries[]? _cartesianSeries;
    private IEnumerable<ISeries>? _pieSeries;
    private ISeries[]? _polarSeries;
    private IGeoSeries[]? _geoSeries;

    public ViewModel()
    {
        GenerateData();
    }

    public ISeries[]? CartesianSeries { get => _cartesianSeries; set { _cartesianSeries = value; OnPropertyChanged(); } }
    public IEnumerable<ISeries>? PieSeries { get => _pieSeries; set { _pieSeries = value; OnPropertyChanged(); } }
    public ISeries[]? PolarSeries { get => _polarSeries; set { _polarSeries = value; OnPropertyChanged(); } }
    public IGeoSeries[]? GeoSeries { get => _geoSeries; set { _geoSeries = value; OnPropertyChanged(); } }

    public ICommand GenerateDataCommand => new Command(o => GenerateData());

    public event PropertyChangedEventHandler? PropertyChanged;

    public void GenerateData()
    {
        var data = new double[] { _r.Next(0, 10), _r.Next(0, 10), _r.Next(0, 10) };

        CartesianSeries = new ISeries[]
        {
            new LineSeries<double> { Values = new ObservableCollection<double>(data) },
            new ColumnSeries<double> { Values = data },
            new StackedAreaSeries<double> { Values = data },
            new StackedColumnSeries<double> { Values = data },
            new ScatterSeries<double> { Values= data },
            new RowSeries<double> { Values = data }
        };

        PieSeries = data.AsLiveChartsPieSeries();

        PolarSeries = new ISeries[]
        {
            new PolarLineSeries<double> { Values = new ObservableCollection<double>(data) }
        };

        GeoSeries = new IGeoSeries[]
        {
            new HeatLandSeries
            {
                Lands = new HeatLand[]
                {
                    new() { Name = "bra", Value = 13 },
                    new() { Name = "mex", Value = 10 },
                    new() { Name = "usa", Value = 15 },
                    new() { Name = "can", Value = 8 }
                }
            }
        };
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
