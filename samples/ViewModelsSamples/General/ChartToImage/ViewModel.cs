using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.ChartToImage
{
    public class ViewModel
    {
        public ISeries[] Series { get; set; } = new ISeries[]
        {
            new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
            new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
        };
    }
}
