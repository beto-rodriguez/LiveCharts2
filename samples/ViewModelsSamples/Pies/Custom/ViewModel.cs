using System.Collections.Generic;
using LiveChartsCore;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Custom;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3 };

        // you can convert any array, list or IEnumerable<T> to a pie series collection:
        Series = data.AsPieSeries((value, series) =>
        {
            // this method is called once per element in the array, so:

            // for the series with the value 6, we set the outer radius to 1
            // for the series with the value 5, the outer radius is 0.9
            // for the series with the value 4, the outer radius is 0.8
            // for the series with the value 3, the outer radius is 0.7

            // The MaxOuterRadius property sets the maximum outer, the value goes from
            // 0 to 1, where 1 is the full available radius and 0 is none.

            series.OuterRadiusOffset = outer;
            outer -= 50;

            series.DataLabelsPaint = new SolidColorPaint(SKColors.White)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            series.ToolTipLabelFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };

            series.DataLabelsFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };
        });
    }

    public IEnumerable<ISeries> Series { get; set; }
}
