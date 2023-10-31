using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Basic;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Name = "Mary",
            Values = new double[] { 2, 4, 5, 6, 2, 3, 7, 4, 2, 5, 7}
        },
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labels = new string[]
            {
                "Category fffffff 1", "Category fffffff 2", "Category fffffff 3",
                "Category fffffff 4", "Category fffffff 5", "Category fffffff 6",
                "Category fffffff 7", "Category fffffff 8", "Category fffffff 9",
                "Category fffffff 10", "Category fffffff 11"
            },
            LabelsRotation = 90,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true,
            // By default the axis tries to optimize the number of // mark
            // labels to fit the available space, // mark
            // when you need to force the axis to show all the labels then you must: // mark
            //ForceStepToMin = true, // mark
            //MinStep = 1 // mark
        }
    };
}
