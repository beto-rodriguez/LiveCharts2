using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Pies.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ViewModel()
    {
        // but 

        // you could convert any IEnumerable to a pie series collection
        // so it is easier to convert an array or collection of data to a pie chart.

        var data = new double[] { 2, 4, 1, 4, 3 };
        // Series = data.AsLiveChartsPieSeries(); this could be enough in some cases // mark
        // but you can customize the series properties using the following overload: // mark
        Series = data.AsLiveChartsPieSeries((value, series) =>
        {
            // here you can configure the series assigned to each value.
            series.Name = $"Series for value {value}";
            series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
            series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer;
            series.DataLabelsFormatter = p => $"Label: {p.Context.DataSource}";
        });

        // this is an equivalent and more verbose syntax. // mark
        // Series = new ISeries[]
        // {
        //     new PieSeries<double> { Values = new double[] { 2 }, Name = "Slice 1" },
        //     new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 2" },
        //     new PieSeries<double> { Values = new double[] { 1 }, Name = "Slice 3" },
        //     new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 4" },
        //     new PieSeries<double> { Values = new double[] { 3 }, Name = "Slice 5" }
        // };
    }

    public IEnumerable<ISeries> Series { get; set; }
}
