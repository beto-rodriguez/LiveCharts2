
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StackedBars.Groups;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new StackedColumnSeries<int>
        {
            Values = new List<int> { 3, 5, 3 },
            Stroke = null,
            StackGroup = 0 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = new List<int> { 4, 2, 3 },
            Stroke = null,
            StackGroup = 0 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = new List<int> { 4, 6, 6 },
            Stroke = null,
            StackGroup = 1 // mark
        },
        new StackedColumnSeries<int>
        {
            Values = new List<int> { 2, 5, 4 },
            Stroke = null,
            StackGroup = 1 // mark
        }
    };

    public Axis[] XAxis { get; set; } =
    {
        new Axis
        {
            LabelsRotation = -15,
            Labels = new[] { "Category 1", "Category 2", "Category 3" }
        }
    };
}
