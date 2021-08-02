using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.ChartToImage
{
    public class ViewModel
    {
        public ISeries[] CatesianSeries { get; set; } = new ISeries[]
        {
            new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
            new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
        };

        public ISeries[] PieSeries { get; set; } = new ISeries[]
        {
            new PieSeries<int> { Values = new int[] { 10, } },
            new PieSeries<int> { Values = new int[] { 6 } },
            new PieSeries<int> { Values = new int[] { 4 } }
        };

        public Dictionary<string, double> MapValues { get; set; } = new Dictionary<string, double>
        {
            ["mex"] = 10,
            ["usa"] = 15,
            ["can"] = 8,
            ["ind"] = 12,
            ["deu"] = 13,
            ["chn"] = 14,
            ["rus"] = 11,
            ["fra"] = 8,
            ["esp"] = 7,
            ["kor"] = 10,
            ["zaf"] = 12,
            ["bra"] = 13,
            ["are"] = 13
        };
    }
}
