using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace ViewModelsSamples.General.Events
{
    public class ViewModel
    {
        public ViewModel()
        {
            var data = new[]
            {
                new City { Name = "Tokyo", Population = 4, JustAnotherProperty = 6 },
                new City { Name = "New York", Population = 6, JustAnotherProperty = 8 },
                new City { Name = "Seoul", Population = 2, JustAnotherProperty = 3 },
                new City { Name = "Moscow", Population = 8, JustAnotherProperty = 6 },
                new City { Name = "Shanghai", Population = 3, JustAnotherProperty = 5 },
                new City { Name = "Guadalajara", Population = 4, JustAnotherProperty = 2 }
            };

            var columnSeries = new ColumnSeries<City>
            {
                Values = data,
                TooltipLabelFormatter = point => $"{point.Model.Name} {point.Model.Population} Million",
                Mapping = (city, point) =>
                {
                    point.PrimaryValue = city.Population;
                    point.SecondaryValue = point.Context.Index;
                }
            };

            var columnSeries2 = new ColumnSeries<City>
            {
                Values = data,
                TooltipLabelFormatter = point => $"{point.Model.Name} {point.Model.JustAnotherProperty} Anothers",
                Mapping = (city, point) =>
                {
                    point.PrimaryValue = city.JustAnotherProperty;
                    point.SecondaryValue = point.Context.Index;
                }
            };

            columnSeries.DataPointerDown += ColumnSeries_DataPointerDown;

            Series = new ISeries[]
            {
                columnSeries,
                columnSeries2,
                new LineSeries<int> { Values = new[] { 6, 7, 2, 9, 6, 2 } },
                new ScatterSeries<int, RectangleGeometry> { Values = new[] { 1, 5, 3, 8, 4, 2 } },
                new StackedColumnSeries<int> { Values = new[] { 1, 5, 3, 8, 4, 2 } },
                new StackedColumnSeries<int> { Values = new[] { 1, 5, 3, 8, 4, 2 } }
            };
        }

        private void ColumnSeries_DataPointerDown(
            IChartView chart,
            IEnumerable<ChartPoint<City, RoundedRectangleGeometry, LabelGeometry>> points)
        {
            // the event passes a collection of the point that were triggered by the pointer down event.
            foreach (var point in points)
            {
                Trace.WriteLine(point.Model.Name);
            }
        }

        public IEnumerable<ISeries> Series { get; set; }

        // XAML platforms also support ICommands
        public ICommand DataPointerDownCommand { get; set; } = new RelayCommand(); // mark

        public Axis[] X { get; set; } = new Axis[] { new Axis { SeparatorsPaint = new SolidColorPaint(SKColors.Red, 2) } };
    }
}
