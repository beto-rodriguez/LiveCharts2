using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Spacing;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 20, 50, 40, 20, 40, 30, 50, 20, 50, 40 },

            // Defines the distance between every bars in the series
            Padding = 0,

            // Defines the max width a bar can have
            MaxBarWidth = double.PositiveInfinity
        }
    };
}
