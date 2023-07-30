using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Scatter.Custom;

public partial class ViewModel : ObservableObject
{
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

        Series = new ISeries[]
        {
            // use the second type parameter to specify the geometry to draw for every point
            // there are already many predefined geometries in the
            // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
            new ScatterSeries<ObservablePoint, RoundedRectangleGeometry>
            {
                Values = values1,
                Stroke = null,
                GeometrySize = 40,
            },

            // You can also use SVG paths to draw the geometry
            // LiveCharts already provides some predefined paths in the SVGPoints class.
            new ScatterSeries<ObservablePoint, SVGPathGeometry>
            {
                Values = values2,
                GeometrySvg = SVGPoints.Heart
            },

            // you can declare your own gemetry and use the SkiaSharp api to draw it
            new ScatterSeries<ObservablePoint, MyGeometry>
            {
                Values = values3,
                GeometrySize = 40,
            }
        };
    }

    public ISeries[] Series { get; set; }
}
