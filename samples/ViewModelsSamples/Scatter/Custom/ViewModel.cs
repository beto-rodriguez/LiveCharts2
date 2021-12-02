using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Scatter.Custom
{
    public class ViewModel
    {
        public ViewModel()
        {
            var r = new Random();
            var values1 = new ObservableCollection<ObservablePoint>();
            var values2 = new ObservableCollection<ObservablePoint>();

            for (var i = 0; i < 20; i++)
            {
                values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
                values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
            }

            Series = new ObservableCollection<ISeries>
            {
                // use the second type argument to specify the geometry to draw for every point
                // there are already many predefined geometries in the
                // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
                new ScatterSeries<ObservablePoint, RoundedRectangleGeometry>
                {
                    Values = values1,
                    Stroke = null,
                    GeometrySize = 40,
                },

                // Or Define your own SVG geometry
                new ScatterSeries<ObservablePoint, MyGeometry>
                {
                    Values = values2,
                    GeometrySize = 40,
                    Stroke = null,
                    Fill = new SolidColorPaint(SKColors.DarkOliveGreen)
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }
    }
}
