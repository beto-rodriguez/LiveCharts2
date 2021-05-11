using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
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
            var ir = 100;
            var pos = PolarLabelsPosition.End;
            Func<ChartPoint, string> formatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}";
            GaugeTotal = 60;
            InitialRotation = 0 + 45 * 3;

            Series = new List<ISeries>
            {
                new PieSeries<double>
                {
                    Values = new List<double> { 10, 0, 0, 0 },
                    InnerRadius = ir,
                    RelativeInnerRadius = 0,
                    DataLabelsDrawableTask = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = pos,
                    DataLabelsFormatter = formatter,
                    DataLabelsSize = 30
                },
                new PieSeries<double>
                {
                    Values = new List<double> { 0, 25, 0, 0 },
                    InnerRadius = ir,
                    RelativeInnerRadius = 0,
                    DataLabelsDrawableTask = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = pos,
                    DataLabelsFormatter = formatter,
                    DataLabelsSize = 30
                },
                new PieSeries<double>
                {
                    Values = new List<double> { 0, 0, 40, 0 },
                    InnerRadius = ir,
                    RelativeInnerRadius = 0,
                    DataLabelsDrawableTask = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = pos,
                    DataLabelsFormatter = formatter,
                    DataLabelsSize = 30
                },
                new PieSeries<double>
                {
                    Values = new List<double> { 0, 0, 0, 50 },
                    InnerRadius = ir,
                    RelativeInnerRadius = 0,
                    DataLabelsDrawableTask = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = pos,
                    DataLabelsFormatter = formatter,
                    DataLabelsSize = 30
                },
                new PieSeries<double>
                {
                    Values = new List<double> { 0, 0, 0, 0 },
                    Fill = new SolidColorPaintTask(new SKColor(220, 220, 220, 90)),
                    InnerRadius = ir,
                    //RelativeInnerRadius = 30,
                    //RelativeOuterRadius = 30,
                    IsFillSeries = true,
                    ZIndex = -1
                },
            };
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

    public class GaugeBuilder
    {
        public double InnerRadius { get; set; }
        public PolarLabelsPosition LabelsPosition { get; set; }

        public GaugeBuilder()
        {

        }
    }
}
