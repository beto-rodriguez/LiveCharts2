using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Polar.Basic
{
    public class ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new PolarLineSeries<double>
            {
                Values = new ObservableCollection<double> { 1500, 1400, 1300, 1200, 1100, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100 }
            }
        };

        public IEnumerable<IPolarAxis> RadialAxes { get; set; }
            = new IPolarAxis[]
            {
                new PolarAxis
                {
                    LabelsRotation = 45,
                    Labeler = v => (v * 10).ToString("N2")
                }
            };

        public IEnumerable<IPolarAxis> AngleAxes { get; set; }
            = new IPolarAxis[]
            {
                new PolarAxis
                {
                    LabelsRotation = 90,
                    Labeler = v => (v * 1000).ToString("N2")
                }
            };
    }
}
