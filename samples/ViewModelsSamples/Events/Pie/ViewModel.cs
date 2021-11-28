using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace ViewModelsSamples.Events.Pie
{
    public class ViewModel
    {
        public ViewModel()
        {
            var data = new[]
            {
                new City { Name = "Tokyo", Population = 4 },
                new City { Name = "New York", Population = 6 },
                new City { Name = "Seoul", Population = 2 },
                new City { Name = "Moscow", Population = 8 },
                new City { Name = "Shanghai", Population = 3 },
                new City { Name = "Guadalajara", Population = 4 }
            };

            // the parameter in the AsLiveChartsSeries() function is optional
            // and is usefull to customize each series
            // it is a function that takes the city and the series assigned to the city as parameters
            var seriesCollection = data.AsLiveChartsPieSeries(
                (city, series) =>
                {
                    series.Name = city.Name;
                    series.Mapping = (cityMapper, point) =>
                    {
                        point.PrimaryValue = cityMapper.Population; // use the population property in this series // mark
                        point.SecondaryValue = point.Context.Index;
                    };
                    series.DataPointerDown += Series_DataPointerDown;
                });

            Series = seriesCollection;
        }

        private void Series_DataPointerDown(
            IChartView chart,
            IEnumerable<ChartPoint<City, DoughnutGeometry, LabelGeometry>> points)
        {
            // the event passes a collection of the points that were triggered by the pointer down event.
            foreach (var point in points)
            {
                Trace.WriteLine($"[series.dataPointerDownEvent] clicked on {point.Model.Name}");
            }
        }

        public IEnumerable<ISeries> Series { get; set; }

        // XAML platforms also support ICommands
        public ICommand DataPointerDownCommand { get; set; } = new RelayCommand(); // mark
    }
}
