using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace ViewModelsSamples.Heat.Basic
{
    public class ViewModel
    {
        public ObservableCollection<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new HeatSeries<WeightedPoint>
            {
                HeatMap = new[]
                {
                    new SKColor(255, 241, 118).AsLvcColor(), // the first element is the "coldest"
                    SKColors.DarkSlateGray.AsLvcColor(),
                    SKColors.Blue.AsLvcColor() // the last element is the "hottest"
                },
                Values = new ObservableCollection<WeightedPoint>
                {
                    // Charles
                    new WeightedPoint(0, 0, 150), // Jan
                    new WeightedPoint(0, 1, 150), // Feb
                    new WeightedPoint(0, 2, 150), // Mar
                    new WeightedPoint(0, 3, 150), // Apr
                    new WeightedPoint(0, 4, 150), // May
                    new WeightedPoint(0, 5, 150), // Jun

                    // Richard
                    new WeightedPoint(1, 0, 150), // Jan
                    new WeightedPoint(1, 1, 150), // Feb
                    new WeightedPoint(1, 2, 150), // Mar
                    new WeightedPoint(1, 3, 150), // Apr
                    new WeightedPoint(1, 4, 150), // May
                    new WeightedPoint(1, 5, 150), // Jun

                    // Ana
                    new WeightedPoint(2, 0, 150), // Jan
                    new WeightedPoint(2, 1, 150), // Feb
                    new WeightedPoint(2, 2, 150), // Mar
                    new WeightedPoint(2, 3, 150), // Apr
                    new WeightedPoint(2, 4, 150), // May
                    new WeightedPoint(2, 5, 150), // Jun

                    // Mari
                    new WeightedPoint(3, 0, 150), // Jan
                    new WeightedPoint(3, 1, 150), // Feb
                    new WeightedPoint(3, 2, 150), // Mar
                    new WeightedPoint(3, 3, 150), // Apr
                    new WeightedPoint(3, 4, 150), // May
                    new WeightedPoint(3, 5, 150), // Jun
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
