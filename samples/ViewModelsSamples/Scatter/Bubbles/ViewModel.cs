using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Scatter.Bubbles;

public class ViewModel
{
    public ISeries[] Series { get; set; }

    public ViewModel()
    {
        var r = new Random();
        var values1 = new List<WeightedPoint>();
        var values2 = new List<WeightedPoint>();

        for (var i = 0; i < 8; i++)
        {
            // the WeightedPoint has 3 properties, X, Y and Weight
            // we use the X and Y coordinates to position the point in the chart
            // the weight will be used by the library to define the size of the points

            // where the minimum weight will be the smallest point, and the max weight the biggest

            // for any weight between these limits the library
            // will interpolate lineally to determine the size of each point

            var x = r.Next(0, 20);
            var y = r.Next(0, 20);
            var w = r.Next(0, 100);

            values1.Add(new WeightedPoint(x, y, w));

            // we do the same for our seconds values collection.
            values2.Add(new WeightedPoint(r.Next(0, 20), r.Next(0, 20), r.Next(0, 100)));
        }

        Series = [
            new ScatterSeries<WeightedPoint>
            {
                Values = values1,
                GeometrySize = 100,
                MinGeometrySize = 5,
            },
            new ScatterSeries<WeightedPoint, RoundedRectangleGeometry>
            {
                Values = values2,
                GeometrySize = 100,
                MinGeometrySize = 5,
                StackGroup = 1
            },
            new ScatterSeries<WeightedPoint>
            {
                Values = [ new() { X = 10, Y = 10, Weight = 500 } ],
                GeometrySize = 100,
                MinGeometrySize = 5,
                // use the stack group to share the Weight between series. // mark
                // in this case, the previous series shares the same // mark
                // StackGroup, thus series share the Weigth bounds. // mark
                StackGroup = 1
            }
        ];
    }
}
