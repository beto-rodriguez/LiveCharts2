using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Race;

public partial class ViewModel : ObservableObject
{
    private readonly Random _r = new();
    private static readonly (string, double)[] s_initialData =
    {
        ("Tsunoda", 500),
        ("Sainz", 450),
        ("Riccardo", 520),
        ("Bottas", 550),
        ("Perez", 660),
        ("Verstapen", 920),
        ("Hamilton", 1000)
    };

    public ViewModel()
    {
        _ = StartRace();
    }

    public bool IsReading { get; set; } = true;

    [ObservableProperty]
    private ISeries[] _series =
        s_initialData
            .Select(x => new RowSeries<ObservableValue>
            {
                Values = new[] { new ObservableValue(x.Item2) },
                Name = x.Item1,
                Stroke = null,
                MaxBarWidth = 25,
                DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
                DataLabelsPosition = DataLabelsPosition.End,
                DataLabelsTranslate = new LvcPoint(-1, 0),
                DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.Coordinate.PrimaryValue}"
            })
            .OrderByDescending(x => ((ObservableValue[])x.Values!)[0].Value)
            .ToArray();

    [ObservableProperty]
    private Axis[] _xAxes = { new Axis { SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) } };

    [ObservableProperty]
    private Axis[] _yAxes = { new Axis { IsVisible = false } };

    public async Task StartRace()
    {
        await Task.Delay(1000);

        // to keep this sample simple, we run the next infinite loop
        // in a real application you should stop the loop/task when the view is disposed

        while (IsReading)
        {
            foreach (var item in Series)
            {
                if (item.Values is null) continue;

                var i = ((ObservableValue[])item.Values)[0];
                i.Value += _r.Next(0, 100);
            }

            Series = Series
                .OrderByDescending(x => ((ObservableValue[])x.Values!)[0].Value)
                .ToArray();

            await Task.Delay(100);
        }
    }
}
