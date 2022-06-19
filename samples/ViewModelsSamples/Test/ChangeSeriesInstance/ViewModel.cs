﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Test.ChangeSeriesInstance;

[ObservableObject]
public partial class ViewModel
{
    private readonly Random _r = new();

    public ViewModel()
    {
        GenerateData();
    }

    [ObservableProperty]
    private ISeries[]? _cartesianSeries;

    [ObservableProperty]
    private IEnumerable<ISeries>? _pieSeries;

    [ObservableProperty]
    private ISeries[]? _polarSeries;

    [ObservableProperty]
    private IGeoSeries[]? _geoSeries;

    [ICommand]
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
}
