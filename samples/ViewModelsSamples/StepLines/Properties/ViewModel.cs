using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModelsSamples.StepLines.Properties
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly Color[] colors = ColorPalletes.FluentDesign;
        private readonly Random random = new Random();
        private StepLineSeries<double> lineSeries;
        private int currentColor = 0;
        private List<ISeries> series;

        public ViewModel()
        {
            lineSeries = new StepLineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
            };

            Series = new List<ISeries>
            {
                lineSeries
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ISeries> Series { get => series; set { series = value; OnPropertyChanged(); } }

        public void ChangeValuesInstance()
        {
            var t = 0;
            var values = new List<double>();
            for (var i = 0; i < 10; i++)
            {
                t += random.Next(-5, 10);
                values.Add(t);
            }

            lineSeries.Values = values;
        }

        public List<ISeries> ChangeSeriesInstance()
        {
            lineSeries = new StepLineSeries<double>
            {
                Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
            };

            Series = new List<ISeries>
            {
                lineSeries
            };

            return series;
        }

        public void NewStroke()
        {
            var nextColorIndex = currentColor++ % colors.Length;
            var color = colors[nextColorIndex];
            lineSeries.Stroke = new SolidColorPaintTask(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
        }

        public void NewFill()
        {
            var nextColorIndex = currentColor++ % colors.Length;
            var color = colors[nextColorIndex];

            lineSeries.Fill = new SolidColorPaintTask(new SKColor(color.R, color.G, color.B, 90));
        }

        public void NewGeometryFill()
        {
            var nextColorIndex = currentColor++ % colors.Length;
            var color = colors[nextColorIndex];

            lineSeries.GeometryFill = new SolidColorPaintTask(new SKColor(color.R, color.G, color.B));
        }

        public void NewGeometryStroke()
        {
            var nextColorIndex = currentColor++ % colors.Length;
            var color = colors[nextColorIndex];

            lineSeries.GeometryStroke = new SolidColorPaintTask(new SKColor(color.R, color.G, color.B)) { StrokeThickness = 3 };
        }

        public void IncreaseGeometrySize()
        {
            if (lineSeries.GeometrySize == 60) return;

            lineSeries.GeometrySize += 10;
        }

        public void DecreaseGeometrySize()
        {
            if (lineSeries.GeometrySize == 0) return;

            lineSeries.GeometrySize -= 10;
        }

        // The next commands are only to enable XAML bindings
        // they are not used in the WinForms sample
        public ICommand ChangeValuesInstanceCommand => new Command(o => ChangeValuesInstance());
        public ICommand ChangeSeriesInstanceCommand => new Command(o => ChangeSeriesInstance());
        public ICommand NewStrokeCommand => new Command(o => NewStroke());
        public ICommand NewFillCommand => new Command(o => NewFill());
        public ICommand NewGeometryFillCommand => new Command(o => NewGeometryFill());
        public ICommand NewGeometryStrokeCommand => new Command(o => NewGeometryStroke());
        public ICommand IncreaseGeometrySizeCommand => new Command(o => IncreaseGeometrySize());
        public ICommand DecreseGeometrySizeCommand => new Command(o => DecreaseGeometrySize());

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
