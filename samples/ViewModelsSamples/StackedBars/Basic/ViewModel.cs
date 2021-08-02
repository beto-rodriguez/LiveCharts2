using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.StackedBars.Basic
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new StackedColumnSeries<int>
            {
                Values = new List<int> { 3, 5, 3, 2, 5, 4, 2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            },
            new StackedColumnSeries<int>
            {
                Values = new List<int> { 4, 2, 3, 2, 3, 4, 2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            },
            new StackedColumnSeries<int>
            {
                Values = new List<int> { -4, 6, 6, 5, 4, 3 , 2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            }
        };
    }
}
