using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Test.Dispose;

public partial class ViewModel : ObservableObject
{
    private readonly Random _r = new();

    public ViewModel()
    {
        var data = new List<double>();

        for (var i = 0; i < 100; i++)
        {
            data.Add(_r.Next(0, 100));
        }

        CartesianSeries =
        [
            new LineSeries<double> { Values = new ObservableCollection<double>(data) },
            new ColumnSeries<double> { Values = data },
            new StackedAreaSeries<double> { Values = data },
            new StackedColumnSeries<double> { Values = data },
            new ScatterSeries<double> { Values= data },
            new RowSeries<double> { Values = data }
        ];

        PieSeries = data.AsPieSeries();

        PolarSeries =
        [
            new PolarLineSeries<double> { Values = new ObservableCollection<double>(data) }
        ];

        GeoSeries =
        [
            new HeatLandSeries
            {
                Lands =
                [
                    new() { Name = "bra", Value = 13 },
                    new() { Name = "mex", Value = 10 },
                    new() { Name = "usa", Value = 15 },
                    new() { Name = "can", Value = 8 }
                ]
            }
        ];
    }

    public IEnumerable<ISeries> CartesianSeries { get; }

    public IEnumerable<ISeries> PieSeries { get; }

    public IEnumerable<ISeries> PolarSeries { get; }

    public IEnumerable<IGeoSeries> GeoSeries { get; }
}
