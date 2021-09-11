using LiveChartsCore;
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
        private double _totalAngle = 360;
        private double _labelsAngle = -60;

        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new PolarLineSeries<double>
            {
                Values = new ObservableCollection<double> { 15, 14, 13, 0, 0, 0, 0, 0, 0, 0, 5, 4, 3, 2, 1 },
                IsClosed = false
            }
        };

        public PolarAxis[] RadialAxes { get; set; }
            = new PolarAxis[]
            {
                new PolarAxis
                {
                    LabelsRotation = 0,
                    LabelsAngle = -60,
                    Labeler = v => (v * 10).ToString("N2")
                }
            };

        public PolarAxis[] AngleAxes { get; set; }
            = new PolarAxis[]
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

        public double TotalAngle
        {
            get => _totalAngle;
            set
            {
                _totalAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalAngle)));
            }
        }

        public double LabelsAngle
        {
            get => _labelsAngle;
            set
            {
                _labelsAngle = value;
                RadialAxes[0].LabelsAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelsAngle)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
