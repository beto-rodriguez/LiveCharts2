using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StepLines.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new StepLineSeries<double?>
        {
            Values = [2, 1, 3, 4, 3, 4, 6],
            Fill = null
        }
    ];
}
