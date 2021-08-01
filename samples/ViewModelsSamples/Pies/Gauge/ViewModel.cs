using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModelsSamples.Pies.Gauge
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly GaugeBuilder _gaugeBuilder;
        private double _initialRotation;
        private double _maxAngle;
        private double _innerRadius = 100;
        private double _offsetRadius = 10;
        private double _backgroundInnerRadius = 100;
        private double _backgroundOffsetRadius = 20;

        public ViewModel()
        {
            _maxAngle = 270;
            GaugeTotal = 60;
            _initialRotation = 135;

            _gaugeBuilder = new GaugeBuilder
            {
                InnerRadius = _innerRadius,
                OffsetRadius = _offsetRadius,
                Background = new SolidColorPaintTask(new SKColor(0, 0, 0, 10)),
                BackgroundInnerRadius = _backgroundInnerRadius,
                BackgroundOffsetRadius = _backgroundOffsetRadius,
                LabelFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}",
                LabelsPosition = PolarLabelsPosition.Start,
                LabelsSize = 30
            }
            .AddValue(new ObservableValue(10))
            .AddValue(new ObservableValue(25))
            .AddValue(new ObservableValue(40))
            .AddValue(new ObservableValue(50));

            Series = _gaugeBuilder.BuildSeries();
        }

        public IEnumerable<ISeries> Series { get; set; }

        public double GaugeTotal { get; set; }

        public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

        public double MaxAngle { get => _maxAngle; set { _maxAngle = value; OnPropertyChanged(); } }

        public double BackgroundInnerRadius
        {
            get => _backgroundInnerRadius;
            set
            {
                _backgroundInnerRadius = value;
                _gaugeBuilder.BackgroundInnerRadius = value;
                OnPropertyChanged();
            }
        }

        public double BackgroundOffsetRadius
        {
            get => _backgroundOffsetRadius;
            set
            {
                _backgroundOffsetRadius = value;
                _gaugeBuilder.BackgroundOffsetRadius = value;
                OnPropertyChanged();
            }
        }

        public double InnerRadius
        {
            get => _innerRadius;
            set
            {
                _innerRadius = value;
                _gaugeBuilder.InnerRadius = value;
                OnPropertyChanged();
            }
        }

        public double OffsetRadius
        {
            get => _offsetRadius;
            set
            {
                _offsetRadius = value;
                _gaugeBuilder.OffsetRadius = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
