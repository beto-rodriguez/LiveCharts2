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
        private double _initialRotation;

        public ViewModel()
        {
            GaugeTotal = 60;
            InitialRotation = 135;
            Series = new GaugeBuilder
            {
                Background = new SolidColorPaintTask(new SKColor(0, 0, 0, 10)),
                BackgroundInnerRadius = 100,
                BackgroundOffsetRadius = 20,
                InnerRadius = 100,
                OffsetRadius = 10,
                LabelFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}",
                LabelsPosition = PolarLabelsPosition.Start,
                LabelsSize = 30
            }
            .AddValue(new ObservableValue(10))
            .AddValue(new ObservableValue(25))
            .AddValue(new ObservableValue(40))
            .AddValue(new ObservableValue(50))
            .BuildSeries();
        }

        public IEnumerable<ISeries> Series { get; set; }

        public double GaugeTotal { get; set; }

        public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
