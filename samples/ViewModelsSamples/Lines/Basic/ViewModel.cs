using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; }
        = {
            new LineSeries<int?>
            {
                Values = new List<int?>() { 1, 5, null , 7, 2 , 5 ,null, 8, 4 },
                Fill = null
            }
        };
}
