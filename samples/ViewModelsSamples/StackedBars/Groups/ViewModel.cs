using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StackedBars.Groups;

public class ViewModel
{
    public ISeries[] Series { get; set; } =
    [
        new StackedColumnSeries<int>
        {
            Values = [3, 5, 3],
            Stroke = null,
            StackGroup = 0 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = [4, 2, 3],
            Stroke = null,
            StackGroup = 0 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = [4, 6, 6],
            Stroke = null,
            StackGroup = 1 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = [2, 5, 4],
            Stroke = null,
            StackGroup = 1 // mark
        }
    ];

    public ICartesianAxis[] XAxis { get; set; } = [
        new Axis
        {
            LabelsRotation = -15,
            Labels = ["Category 1", "Category 2", "Category 3"]
        }
    ];
}
