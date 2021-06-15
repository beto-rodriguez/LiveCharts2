using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Heat.Basic
{
    public class ViewModel
    {
        public ObservableCollection<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new HeatSeries<WeightedPoint>
            {
                Values = new ObservableCollection<WeightedPoint>
                {
                    // Charles
                    new WeightedPoint(0, 0, 150),
                    new WeightedPoint(0, 1, 123),
                    new WeightedPoint(0, 2, 310),
                    new WeightedPoint(0, 3, 225),
                    new WeightedPoint(0, 4, 473),
                    new WeightedPoint(0, 5, 373),

                    // Richard
                    new WeightedPoint(1, 0, 432),
                    new WeightedPoint(1, 1, 312),
                    new WeightedPoint(1, 2, 135),
                    new WeightedPoint(1, 3, 78),
                    new WeightedPoint(1, 4, 124),
                    new WeightedPoint(1, 5, 423),

                    // Ana
                    new WeightedPoint(2, 0, 543),
                    new WeightedPoint(2, 1, 134),
                    new WeightedPoint(2, 2, 524),
                    new WeightedPoint(2, 3, 315),
                    new WeightedPoint(2, 4, 145),
                    new WeightedPoint(2, 5, 80),

                    // Mari
                    new WeightedPoint(3, 0, 90),
                    new WeightedPoint(3, 1, 123),
                    new WeightedPoint(3, 2, 70),
                    new WeightedPoint(3, 3, 123),
                    new WeightedPoint(3, 4, 432),
                    new WeightedPoint(3, 5, 142),
                },
            }
        };

        public ObservableCollection<Axis> XAxes { get; set; } = new ObservableCollection<Axis>
        {
            new Axis
            {
                Labels = new [] { "Charles", "Richard", "Ana", "Mari" }
            }
        };

        public ObservableCollection<Axis> YAxes { get; set; } = new ObservableCollection<Axis>
        {
            new Axis
            {
                Labels = new [] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" }
            }
        };
    }
}
