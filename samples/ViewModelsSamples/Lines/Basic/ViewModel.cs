using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; }
        = {
           new ScatterSeries<ObservablePoint>
          {
              // Values = new List<double> { -2, -1, 3, 5, 3, 4, 6 },
              Values = new List<ObservablePoint>
              {
                  new ObservablePoint{ X = 100000, Y = 3.4},
                  new ObservablePoint{ X = 110000, Y = 2.2},
                  new ObservablePoint{ X = 120000, Y = 3.9},
                  new ObservablePoint{ X = 130000, Y = 4.1},
                  new ObservablePoint{ X = 150000, Y = 3.4},
              },
              //Fill = null,
              // use the line smoothness property to control the curve
              // it goes from 0 to 1
              // where 0 is a straight line and 1 the most curved
              //LineSmoothness = 0
          }
        };
}
