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
            Values = new double[] { 2, 5, 4 }
        },
        new ColumnSeries<double>
        {
            Name = "Ana",
            Values = new double[] { 3, 1, 6 }
        }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labels = new string[] { "Category 1", "Category 2", "Category 3" },
            LabelsRotation = 0,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true
        }
    };
}
