using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System.Collections.Generic;

namespace ViewModelsSamples.General.Events
{
    public class ViewModel
    {
        public ViewModel()
        {
            var columnSeries = new ColumnSeries<City>
            {
                Values = new[]
                    {
                    new City { Name = "Tokyo", Population = 4 },
                    new City { Name = "New York", Population = 6 },
                    new City { Name = "Seoul", Population = 2 },
                    new City { Name = "Moscow", Population = 8 },
                    new City { Name = "Shanghai", Population = 3 },
                    new City { Name = "Guadalajara", Population = 4 }
                },
                TooltipLabelFormatter = point => $"{point.Model.Name} {point.Model.Population} Million",
                Mapping = (city, point) =>
                {
                    point.PrimaryValue = city.Population;
                    point.SecondaryValue = point.Context.Index;
                }
            };

            columnSeries.DataPointHover += ColumnSeries_DataPointerDown; ;

            Series = new ISeries[] { columnSeries };
        }

        private void ColumnSeries_DataPointerDown(
            IEnumerable<ChartPoint<City, RoundedRectangleGeometry, LabelGeometry>> points)
        {
            // the event passes a collection of the point that were triggered by the pointer down event.
            foreach (var point in points)
            {
                var bingo = point;
            }
        }

        public IEnumerable<ISeries> Series { get; set; }
    }
}
