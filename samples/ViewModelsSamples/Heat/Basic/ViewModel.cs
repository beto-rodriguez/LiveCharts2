using System.Collections.ObjectModel;
using System.Drawing;
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
                HeatMap = new[]
                {
                    Color.FromArgb(255, 255, 241, 118), // the first element is the "coldest"
                    Color.DarkSlateGray,
                    Color.Blue // the last element is the "hottest"
                },
                Values = new ObservableCollection<WeightedPoint>
                {
                    // Charles
                    new WeightedPoint(0, 0, 150), // Jan
                    new WeightedPoint(0, 1, 123), // Feb
                    new WeightedPoint(0, 2, 310), // Mar
                    new WeightedPoint(0, 3, 225), // Apr
                    new WeightedPoint(0, 4, 473), // May
                    new WeightedPoint(0, 5, 373), // Jun

                    // Richard
                    new WeightedPoint(1, 0, 432), // Jan
                    new WeightedPoint(1, 1, 312), // Feb
                    new WeightedPoint(1, 2, 135), // Mar
                    new WeightedPoint(1, 3, 78), // Apr
                    new WeightedPoint(1, 4, 124), // May
                    new WeightedPoint(1, 5, 423), // Jun

                    // Ana
                    new WeightedPoint(2, 0, 543), // Jan
                    new WeightedPoint(2, 1, 134), // Feb
                    new WeightedPoint(2, 2, 524), // Mar
                    new WeightedPoint(2, 3, 315), // Apr
                    new WeightedPoint(2, 4, 145), // May
                    new WeightedPoint(2, 5, 80), // Jun

                    // Mari
                    new WeightedPoint(3, 0, 90), // Jan
                    new WeightedPoint(3, 1, 123), // Feb
                    new WeightedPoint(3, 2, 70), // Mar
                    new WeightedPoint(3, 3, 123), // Apr
                    new WeightedPoint(3, 4, 432), // May
                    new WeightedPoint(3, 5, 142), // Jun
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
