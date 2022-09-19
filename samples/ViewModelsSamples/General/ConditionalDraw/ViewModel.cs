using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.ConditionalDraw;

[ObservableObject]
public partial class ViewModel
{
    public ViewModel()
    {
        var series1 = new ColumnSeries<ObservableValue>
        {
            Name = "Mary",
            Values = new ObservableValue[] { new(2), new(5), new(4), new(6), new(8), new(3), new(2), new(4), new(6) }
        }
        .UsePaint(new SolidColorPaint(SKColors.Black) { IsStroke = true, StrokeThickness = 5 })
        .When(point => point.Model?.Value > 5);

        var series2 = new ColumnSeries<City>
        {
            Name = "Mary",
            Values = new City[] { new(4), new(2), new(8), new(3), new(2), new(4), new(6), new(4), new(4) },
            Mapping = (city, point) =>
            {
                point.PrimaryValue = city.Population;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            }
        }
        .UsePaint(new SolidColorPaint(SKColors.Black) { IsStroke = true, StrokeThickness = 5 })
        .When(point => point.Model?.Population > 5);

        Series = new ISeries[]
        {
            series1,
            series2
        };

        Randomize((ISeries<ObservableValue>)Series[0]);
        Randomize((ISeries<City>)Series[1]);
    }

    public ISeries[] Series { get; set; }

    private async void Randomize<T>(ISeries<T> series)
    {
        var r = new Random();
        if (series.Values is null) return;

        while (true)
        {
            await Task.Delay(500);

            foreach (var item in series.Values)
            {
                if (item is ObservableValue observableValue)
                {
                    observableValue.Value = r.Next(0, 10);
                }

                if (item is City city)
                {
                    city.Population = r.Next(0, 10);
                }
            }
        }
    }
}
