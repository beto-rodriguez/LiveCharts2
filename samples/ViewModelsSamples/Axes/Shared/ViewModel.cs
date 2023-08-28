using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Axes.Shared;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        var values1 = new int[50];
        var values2 = new int[50];
        var r = new Random();
        var t = 0;
        var t2 = 0;

        for (var i = 0; i < 50; i++)
        {
            t += r.Next(-90, 100);
            values1[i] = t;

            t2 += r.Next(-9000, 10000);
            values2[i] = t2;
        }

        SeriesCollection1 = new ISeries[] { new LineSeries<int> { Values = values1 } };
        SeriesCollection2 = new ISeries[] { new ColumnSeries<int> { Values = values2 } };

        // ideally, when sharing an axis, you should set the initial limits for all the axes involved. // mark
        var start = 0 - 5;
        var end = 50 + 5;

        X1 = new Axis[] { new Axis { MinLimit = start, MaxLimit = end } };
        X2 = new Axis[] { new Axis { MinLimit = start, MaxLimit = end } };

        X1[0].SharedWith = X2;
        X2[0].SharedWith = X1;

        // Force the chart to use 70px margin on the left, this way we can align both charts Y axes. // mark
        DrawMargin = new Margin(70, Margin.Auto, Margin.Auto, Margin.Auto);

        // Advanced alternative:
        // you can also ask an axis its posible dimensions to determine the margin you need.

        // First you need to get a chart from the UI
        // in this sample we use the in-memory chart provided by the library.

        // var cartesianChart = new SKCartesianChart();
        // var axis = cartesianChart.YAxes.First() as Axis;
        // var size = axis.GetPossibleSize(cartesianChart.Core);

        // finally instead of using the static 70px, we can use the actual width of the axis.

        // DrawMargin = new Margin(size.Width, Margin.Auto, Margin.Auto, Margin.Auto);

        // normally you would need measure all the axes involved, and use the greater width to
        // calculate the required margin.
    }

    public ISeries[] SeriesCollection1 { get; set; }
    public ISeries[] SeriesCollection2 { get; set; }
    public Axis[] X1 { get; set; }
    public Axis[] X2 { get; set; }
    public Margin DrawMargin { get; set; }
}
