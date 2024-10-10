using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StackedArea.StepArea;

public class ViewModel
{
    public ISeries[] Series { get; set; } =
    [
        new StackedStepAreaSeries<double>([3, 2, 3, 5, 3, 4, 6]),
        new StackedStepAreaSeries<double>([6, 5, 6, 3, 8, 5, 2]),
        new StackedStepAreaSeries<double>([4, 8, 2, 8, 9, 5, 3])
    ];
}
