using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ViewModelsSamples.Bars.Race
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly Random _r = new Random();
        private List<ISeries> _series;

        public ViewModel()
        {
            Series = new List<ISeries>
            {
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(50) },
                    Name = "Tsunoda",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(45) },
                    Name = "Sainz",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(52) },
                    Name = "Riccardo",
                    Stroke  = null,
                    MaxBarWidth = 52,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(55) },
                    Name = "Bottas",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(66) },
                    Name = "Perez",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(92) },
                    Name = "Verstapen",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
                new RowSeries<ObservableValue>
                {
                    Values = new []{ new ObservableValue(100) },
                    Name = "Hamilton",
                    Stroke  = null,
                    MaxBarWidth = 50,
                    DataLabelsPaint = new SolidColorPaintTask(new SKColor(40, 40, 40)),
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
                },
            };
        }

        public List<ISeries> Series { get => _series; set { _series = value; OnPropertyChanged(nameof(Series)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RandomIncrement()
        {
            foreach (var item in Series)
            {
                var i = ((ObservableValue[])item.Values)[0];
                i.Value += _r.Next(0, 30);
            }

            Series = Series.OrderBy(x => ((ObservableValue[])x.Values)[0].Value).ToList();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
