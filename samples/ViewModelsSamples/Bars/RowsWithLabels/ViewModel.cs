using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Bars.RowsWithLabels
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new RowSeries<int>
            {
                Values = new List<int> { 8, -3, 4, -3, 3, 4, -2 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.End
            },
            new RowSeries<int>
            {
                Values = new List<int> { 4, -6, 5, -9, 4, 8, -6 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(250, 250, 250)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle
            },
            new RowSeries<int>
            {
                Values = new List<int> { 6, -9, 3, -6, 8, 2, -9 },
                Stroke = null,
                DataLabelsPaint = new SolidColorPaintTask(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Start
            }
        };
    }
}
