using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Scatter.Custom;

public class ViewModel
{
    public ISeries[] Series { get; set; }

    public ViewModel()
    {
        var r = new Random();
        var values1 = new ObservableCollection<ObservablePoint>();
        var values2 = new ObservableCollection<ObservablePoint>();
        var values3 = new ObservableCollection<ObservablePoint>();

        for (var i = 0; i < 20; i++)
        {
            values1.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
            values2.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
            values3.Add(new ObservablePoint(r.Next(0, 20), r.Next(0, 20)));
        }

        Series = [
            // use the second generic parameter to define the geometry to draw
            // there are many predefined geometries in the LiveChartsCore.Drawing namespace
            // for example, the StarGeometry, CrossGeometry, RectangleGeometry and DiamondGeometry
            new ScatterSeries<ObservablePoint, HeartGeometry>
            {
                Values = values1,
                Stroke = null,
                GeometrySize = 40
            },

            // You can also use SVG paths to draw the geometry
            // the VariableSVGPathGeometry can change the drawn path at runtime
            new ScatterSeries<ObservablePoint, VariableSVGPathGeometry>
            {
                Values = values2,
                GeometrySvg = SVGPoints.Pin
            },

            // finally you can also use SkiaSharp to draw your own geometry
            new ScatterSeries<ObservablePoint, MyGeometry>
            {
                Values = values3,
                GeometrySize = 40,
            }
        ];
    }
}
