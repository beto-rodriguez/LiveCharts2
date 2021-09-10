using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ViewModelsSamples.Polar.Basic
{
    public class ViewModel : INotifyPropertyChanged
    {
        private double _initialRotation = 15;
        private double _innerRadius = 50;

        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new PolarLineSeries<double>
            {
                Values = new ObservableCollection<double> { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
                IsClosed = false
            }
        };

        public IEnumerable<IPolarAxis> RadialAxes { get; set; }
            = new IPolarAxis[]
            {
                new PolarAxis
                {
                    LabelsRotation = 0,
                    LabelsAngle = -60,
                    Labeler = v => (v * 10).ToString("N2")
                }
            };

        public IEnumerable<IPolarAxis> AngleAxes { get; set; }
            = new IPolarAxis[]
            {
                new PolarAxis
                {
                    //LabelsRotation = 90,
                    Labeler = v => (v * 1000).ToString("N2")
                }
            };

        public double InitialRotation
        {
            get => _initialRotation;
            set
            {
                _initialRotation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitialRotation)));
            }
        }

        public double InnerRadius
        {
            get => _innerRadius;
            set
            {
                _innerRadius = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InnerRadius)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
