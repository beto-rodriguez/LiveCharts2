using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Race;

[ObservableObject]
public partial class ViewModel
{
    private readonly Random _r = new();
    private static readonly (string, double)[] s_initialData =
    {
        ("Tsunoda", 50),
        ("Sainz", 45),
        ("Riccardo", 52),
        ("Bottas", 55),
        ("Perez", 66),
        ("Verstapen", 92),
        ("Hamilton", 100)
    };

    [ObservableProperty]
    private ISeries[] _series =
        s_initialData
            .Select(x => new RowSeries<ObservableValue>
            {
                Values = new[] { new ObservableValue(x.Item2) },
                Name = x.Item1,
                Stroke = null,
                MaxBarWidth = 50,
                DataLabelsPaint = new SolidColorPaint(new SKColor(40, 40, 40)),
                DataLabelsPosition = DataLabelsPosition.End,
                DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
            })
            .ToArray();


    public void RandomIncrement()
    {
        foreach (var item in Series)
        {
            if (item.Values is null) continue;

            var i = ((ObservableValue[])item.Values)[0];
            i.Value += _r.Next(0, 30);
        }

        Series = Series.OrderBy(x => ((ObservableValue[])x.Values!)[0].Value).ToArray();
    }
}
