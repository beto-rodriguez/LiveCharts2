using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Scatter.Bubbles
{
    public class ViewModel
    {
        public ViewModel()
        {
            var r = new Random();
            var values1 = new ObservableCollection<WeightedPoint>();
            var values2 = new ObservableCollection<WeightedPoint>();

            for (var i = 0; i < 20; i++)
            {
                values1.Add(new WeightedPoint(r.Next(0, 20), r.Next(0, 20), r.Next(0, 20)));
                values2.Add(new WeightedPoint(r.Next(0, 20), r.Next(0, 20), r.Next(0, 20)));
            }

            Series = new ObservableCollection<ISeries>
            {
                new ScatterSeries<WeightedPoint, RoundedRectangleGeometry>
                {
                    Values = values1,
                    GeometrySize = 40,
                    MinGeometrySize = 15
                },

                new ScatterSeries<WeightedPoint, CircleGeometry>
                {
                    Values = values2,
                    GeometrySize = 40,
                    MinGeometrySize = 15
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }
    }
}
