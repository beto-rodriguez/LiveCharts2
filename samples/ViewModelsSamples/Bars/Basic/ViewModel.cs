using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace ViewModelsSamples.Bars.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } =
    [
        new ColumnSeries<double>
        {
            Name = "Mary",
            Values = [2, 5, 4]
        },
        new ColumnSeries<double>
        {
            Name = "Ana",
            Values = [3, 1, 6]
        }
    ];

    public Axis[] XAxes { get; set; } =
    [
        new Axis
        {
            Labels = ["Category 1", "Category 2", "Category 3"],
            LabelsRotation = 0,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true,
            // By default the axis tries to optimize the number of // mark
            // labels to fit the available space, // mark
            // when you need to force the axis to show all the labels then you must: // mark
            ForceStepToMin = true, // mark
            MinStep = 1 // mark
        }
    ];
}
