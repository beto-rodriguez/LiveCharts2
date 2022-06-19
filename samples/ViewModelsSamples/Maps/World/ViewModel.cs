﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Maps.World;

[ObservableObject]
public  partial class ViewModel
{
    private bool _isBrazilInChart = true;
    private readonly IWeigthedMapLand _brazil;
    private readonly Random _r = new();

    public ViewModel()
    {
        // every country has a unique identifier
        // check the "shortName" property in the following
        // json file to assign a value to a country in the heat map
        // https://github.com/beto-rodriguez/LiveCharts2/blob/master/docs/_assets/word-map-index.json
        var lands = new HeatLand[]
        {
            new() { Name = "bra", Value = 13 },
            new() { Name = "mex", Value = 10 },
            new() { Name = "usa", Value = 15 },
            new() { Name = "can", Value = 8 },
            new() { Name = "ind", Value = 12 },
            new() { Name = "deu", Value = 13 },
            new() { Name= "jpn", Value = 15 },
            new() { Name = "chn", Value = 14 },
            new() { Name = "rus", Value = 11 },
            new() { Name = "fra", Value = 8 },
            new() { Name = "esp", Value = 7 },
            new() { Name = "kor", Value = 10 },
            new() { Name = "zaf", Value = 12 },
            new() { Name = "are", Value = 13 }
        };

        Series = new HeatLandSeries[] { new HeatLandSeries { Lands = lands } };

        _brazil = lands.First(x => x.Name == "bra");
        DoRandomChanges();
    }

    public HeatLandSeries[] Series { get; set; }

    [ICommand]
    public void ToggleBrazil()
    {
        var lands = Series[0].Lands;
        if (lands is null) return;

        if (_isBrazilInChart)
        {
            Series[0].Lands = lands.Where(x => x != _brazil).ToArray();
            _isBrazilInChart = false;
            return;
        }

        Series[0].Lands = lands.Concat(new[] { _brazil }).ToArray();
        _isBrazilInChart = true;
    }

    private async void DoRandomChanges()
    {
        await Task.Delay(1000);

        while (true)
        {
            foreach (var shape in Series[0].Lands ?? Enumerable.Empty<IWeigthedMapLand>())
            {
                shape.Value = _r.Next(0, 20);
            }

            await Task.Delay(500);
        }
    }
}
