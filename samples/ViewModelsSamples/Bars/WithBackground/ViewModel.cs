using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples.Bars.WithBackground
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 10, 10, 10, 10, 10, 10, 10 },
                Stroke = null,
                Fill = new SolidColorPaintTask(new SKColor(30, 30, 30, 30)),
                IgnoresBarPosition = true
            },
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 3, 10, 5, 3, 7, 3, 8 },
                Stroke = null,
                Fill = new SolidColorPaintTask(SKColors.CornflowerBlue),
                IgnoresBarPosition = true
            }
        };

        public List<Axis> YAxes { get; set; } = new List<Axis>
        {
            new Axis { MinLimit = 0, MaxLimit = 10 }
        };
    }
}
