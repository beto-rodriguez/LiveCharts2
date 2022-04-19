﻿using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.Basic;

public class ViewModel
{
    public ViewModel()
    {
        Series = new ISeries[]
        {
            new PieSeries<double> { Values = new double[] { 2 }, Name = "Slice 1" },
            new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 2" },
            new PieSeries<double> { Values = new double[] { 1 }, Name = "Slice 3" },
            new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 4" },
            new PieSeries<double> { Values = new double[] { 3 }, Name = "Slice 5" }
        };

        // the next code is equivalent to the previous one, // mark
        // but using the AsLiveChartsPieSeries() extension.  // mark

        // you could convert any IEnumerable to a pie series collection  // mark
        // so it is easier to convert an array or collection of data to a pie chart.  // mark

        //var data = new List<double> { 2, 4, 1, 4, 3 };  // mark
        //Series = data.AsLiveChartsPieSeries(); // mark

        // and the name property? try this: // mark
        //Series = data.AsLiveChartsPieSeries((value, series) => // mark
        //{ // mark
        //    // here you can configure the series assigned to each value. // mark
        //    series.Name = $"Series for value {value}"; // mark
        //}); // mark
    }

    public IEnumerable<ISeries> Series { get; set; }
}
