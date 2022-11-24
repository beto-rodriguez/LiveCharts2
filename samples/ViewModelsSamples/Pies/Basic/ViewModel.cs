using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace ViewModelsSamples.Pies.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ViewModel()
    {
        // you could convert any IEnumerable to a pie series collection
        var data = new double[] { 2, 4, 1, 4, 3 };

        // Series = data.AsLiveChartsPieSeries(); this could be enough in some cases // mark
        // but you can customize the series properties using the following overload: // mark

        Series = data.AsLiveChartsPieSeries((value, series) =>
        {
            // here you can configure the series assigned to each value.
            series.Name = $"Series for value {value}";
            series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
            series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer;
            series.DataLabelsFormatter = p => $"{p.PrimaryValue} / {p.StackedValue!.Total} ({p.StackedValue.Share:P2})";
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

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
}
