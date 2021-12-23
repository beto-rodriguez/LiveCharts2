using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Basic;

public class ViewModel
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }},
            new PieSeries<double> { Values = new double[] { 2 }}
            //new PieSeries<double> { Values = new double[] { 2 }, Name = "Slice 1" },
            //new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 2" },
            //new PieSeries<double> { Values = new double[] { 1 }, Name = "Slice 3" },
            //new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 4" },
            //new PieSeries<double> { Values = new double[] { 3 }, Name = "Slice 5" }
        };

        // the next code is equivalent to the previous one,
        // but using the AsLiveChartsPieSeries() extension.

        // you could convert any IEnumerable to a pie series collection
        // so it is easier to convert an array or collection of data to a pie chart.

        //var data = new List<double> { 2, 4, 1, 4, 3 };
        //Series = data.AsLiveChartsPieSeries();

        // and the name property? try this:
        //Series = data.AsLiveChartsPieSeries((value, series) =>
        //{
        //    // here you can configure the series assigned to each value.
        //    series.Name = $"Series for value {value}";
        //});
    }

    public IEnumerable<ISeries> Series { get; set; }
}
