using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Race;

// this class is used to store the data for each pilot // mark
public class PilotInfo : ObservableValue
{
    public PilotInfo(string name, int value, SolidColorPaint paint)
    {
        Name = name;
        Paint = paint;

        // the ObservableValue.Value property is used by the chart
        Value = value;
    }

    public string Name { get; set; }
    public SolidColorPaint Paint { get; set; }
}

public partial class ViewModel : ObservableObject
{
    private readonly Random _r = new();
    private readonly PilotInfo[] _data;

    public ViewModel()
    {
        // generate some paints for each pilot:
        var paints = Enumerable.Range(0, 7)
            .Select(i => new SolidColorPaint(ColorPalletes.MaterialDesign500[i].AsSKColor()))
            .ToArray();

        // generate some data for each pilot:
        _data =
        [
            new("Tsunoda",   500,  paints[0]),
            new("Sainz",     450,  paints[1]),
            new("Riccardo",  520,  paints[2]),
            new("Bottas",    550,  paints[3]),
            new("Perez",     660,  paints[4]),
            new("Verstapen", 920,  paints[5]),
            new("Hamilton",  1000, paints[6])
        ];

        var rowSeries = new RowSeries<PilotInfo>
        {
            Values = SortData(),
            DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
            DataLabelsPosition = DataLabelsPosition.End,
            DataLabelsTranslate = new(-1, 0),
            DataLabelsFormatter = point => $"{point.Model!.Name} {point.Coordinate.PrimaryValue}",
            MaxBarWidth = 50,
            Padding = 10,
        }
        .OnPointMeasured(point =>
        {
            // assign a different color to each point
            if (point.Visual is null) return;
            point.Visual.Fill = point.Model!.Paint;
        });

        _series = [rowSeries];

        _ = StartRace();
    }

    [ObservableProperty]
    private ISeries[] _series;

    [ObservableProperty]
    private Axis[] _xAxes = [new Axis { SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) }];

    [ObservableProperty]
    private Axis[] _yAxes = [new Axis { IsVisible = false }];

    public bool IsReading { get; set; } = true;

    public async Task StartRace()
    {
        await Task.Delay(1000);

        // to keep this sample simple, we run the next infinite loop
        // in a real application you should stop the loop/task when the view is disposed

        while (IsReading)
        {
            // do a random change to the data
            foreach (var item in _data)
                item.Value += _r.Next(0, 100);

            Series[0].Values = SortData();

            await Task.Delay(100);
        }
    }

    private PilotInfo[] SortData() => [.. _data.OrderBy(x => x.Value)];
}
