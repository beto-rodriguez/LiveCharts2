using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.General.ChartToImage;

public partial class ViewModel : ObservableObject
{
    public ISeries[] CatesianSeries { get; set; } =
    {
        new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
        new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
    };

    public ISeries[] PieSeries { get; set; } =
    {
        new PieSeries<int> { Values = new int[] { 10, } },
        new PieSeries<int> { Values = new int[] { 6 } },
        new PieSeries<int> { Values = new int[] { 4 } }
    };

    public IGeoSeries[] GeoSeries { get; set; } = new HeatLandSeries[]
    {
        new()
        {
            Lands = new HeatLand[]
            {
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
            }
        }
    };
}
