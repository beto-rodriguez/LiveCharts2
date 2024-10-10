using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.ChartToImage;

public partial class ViewModel
{
    public ISeries[] CatesianSeries { get; set; } = [
        new LineSeries<int> { Values = [1, 5, 4, 6] },
        new ColumnSeries<int> { Values = [4, 8, 2, 4] }
    ];

    public ISeries[] PieSeries { get; set; } = [
        new PieSeries<int> { Values = [10,] },
        new PieSeries<int> { Values = [6] },
        new PieSeries<int> { Values = [4] }
    ];

    public IGeoSeries[] GeoSeries { get; set; } = [
        new HeatLandSeries
        {
            Lands = [
                new() { Name = "bra", Value = 13 },
                new() { Name = "mex", Value = 10 },
                new() { Name = "usa", Value = 15 },
                new() { Name = "can", Value = 8 },
                new() { Name = "ind", Value = 12 },
                new() { Name = "deu", Value = 13 },
                new() { Name= "jpn", Value = 15 },
                new() { Name = "chn", Value = 14 },
                new() { Name = "rus", Value = 11 },
                new() { Name = "fra", Value = 8 },
                new() { Name = "esp", Value = 7 },
                new() { Name = "kor", Value = 10 },
                new() { Name = "zaf", Value = 12 },
                new() { Name = "are", Value = 13 }
            ]
        }
    ];
}
