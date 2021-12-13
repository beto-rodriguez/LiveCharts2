using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Bars.Spacing;

public class ViewModel
{
    public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, 2, 4, 3, 5, 2, 5, 4, 2, 4, 3, 5 },

                // Defines the distance between every group of bars that share
                // the same secondary coordinate (normally the X coordinate)
                GroupPadding = 0,

                // Defines the max width a bar can have
                MaxBarWidth = double.PositiveInfinity
            }
        };
}
