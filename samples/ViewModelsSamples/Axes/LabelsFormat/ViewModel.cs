using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Axes.LabelsFormat
{
    public class ViewModel
    {
        public ViewModel()
        {
            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = new ObservableCollection<double> { 200, 558, 458, 249, 457, 339, 587 },
                }
            };

            XAxes = new List<Axis>
            {
                new Axis
                {
                    // Use the Label property to indicate the format of the labels in the axis
                    // The Labeler takes the value of the label as parameter and must return it as string
                    Labeler = (value) => "Day " + value,

                    // The Step property 
                    MinStep = 1
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    // Now the Y axis we will display it as currency
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
