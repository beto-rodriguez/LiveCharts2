using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace ViewModelsSamples.General.ConditionalDraw;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        var series1 = new ColumnSeries<ObservableValue>
        {
            Name = "Mary",
            Values = new ObservableValue[] { new(2), new(5), new(4), new(6), new(8), new(3), new(2), new(4), new(6) }
        }
        .WithConditionalPaint(new SolidColorPaint(SKColors.Black.WithAlpha(50)))
        .When(point => point.Model?.Value > 5);

        var series2 = new ColumnSeries<City>
        {
            Name = "Mary",
            Values = new City[] { new(4), new(2), new(8), new(3), new(2), new(4), new(6), new(4), new(4) },
            Mapping = (city, point) =>
            {
                point.PrimaryValue = city.Population;
                point.SecondaryValue = point.Context.Entity.MetaData!.EntityIndex;
            }
        }
        .WithConditionalPaint(new SolidColorPaint(SKColors.Black.WithAlpha(50)))
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

    public RectangularSection[] Sections { get; set; } =
    {
        new RectangularSection
        {
            Yi = 5,
            Yj = 5,
            Stroke = new SolidColorPaint
            {
                Color = SKColors.Black,
                StrokeThickness = 3,
                PathEffect = new DashEffect(new float[] { 6, 6 })
            }
        }
    };

    public Axis[] Y { get; set; } =
    {
        new Axis { MinLimit = 0 }
    };

    private async void Randomize<T>(ISeries<T> series)
    {
        var r = new Random();
        if (series.Values is null) return;

        while (true)
        {
            await Task.Delay(3000);

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
