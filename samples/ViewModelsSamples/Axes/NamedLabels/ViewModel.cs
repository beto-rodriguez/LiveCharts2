using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Axes.NamedLabels
{
    public class ViewModel
    {
        public ViewModel()
        {
            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<int>
                {
                    Name = "Sales",
                    Values = new ObservableCollection<int> { 200, 558, 458, 249 },
                },
                new LineSeries<int>
                {
                    Name = "Projected",
                    Values = new ObservableCollection<int> { 300, 450, 400, 280 },
                    Fill = null
                }
            };

            XAxes = new List<Axis>
            {
                new Axis
                {
                    // Use the labels property to define named labels.
                    Labels = new string[] { "Anne", "Johnny", "Zac", "Rosa" }
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    // Now the Y axis we will display labels as currency
                    // LiveCharts provides some common formatters
                    // in this case we are using the currency formatter.
                    Labeler = Labelers.Currency

                    // you could also build your own currency formatter
                    // for example:
                    // Labeler = (value) => value.ToString("C")

                    // But the one that LiveCharts provides creates shorter labels when
                    // the amount is in millions or trillions
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }

        public List<Axis> XAxes { get; set; }

        public List<Axis> YAxes { get; set; }

    }
}
