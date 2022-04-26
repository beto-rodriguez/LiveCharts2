
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; }
        = {
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(3, 1),
                    new ObservablePoint(4, 1),
                    new ObservablePoint(4, 1.25),
                    new ObservablePoint(4, 1.5),
                    new ObservablePoint(4, 1.75),
                    new ObservablePoint(4, 2),
                    new ObservablePoint(5, 2)
                },
                Fill = null
            },
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(3, 0.5),
                    new ObservablePoint(4, 2.5),
                    new ObservablePoint(5, 0.5)
                },
                Fill = null
            }
        };
}
